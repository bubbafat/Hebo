#region License, Terms and Conditions
//
// Jayrock - JSON and JSON-RPC for Microsoft .NET Framework and Mono
// Written by Atif Aziz (atif.aziz@skybow.com)
// Copyright (c) 2005 Atif Aziz. All rights reserved.
//
// This library is free software; you can redistribute it and/or modify it under
// the terms of the GNU Lesser General Public License as published by the Free
// Software Foundation; either version 2.1 of the License, or (at your option)
// any later version.
//
// This library is distributed in the hope that it will be useful, but WITHOUT
// ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS
// FOR A PARTICULAR PURPOSE. See the GNU Lesser General Public License for more
// details.
//
// You should have received a copy of the GNU Lesser General Public License
// along with this library; if not, write to the Free Software Foundation, Inc.,
// 59 Temple Place, Suite 330, Boston, MA 02111-1307 USA 
//
#endregion

namespace Jayrock.JsonRpc
{
    #region Imports

    using System;
    using System.Collections;
    using System.ComponentModel.Design;
    using System.Diagnostics;
    using System.IO;
    using Jayrock.Json;
    using Jayrock.Json.Conversion;
    using Jayrock.Services;

    #endregion

    /// <summary>
    /// The workhorse of the JSON-RPC implementation to parse the request,
    /// invoke methods on a service and write out the response.
    /// </summary>

    // TODO: Add async processing.

    public class JsonRpcDispatcher
    {
        private readonly IService _service;
        private readonly IServiceProvider _serviceProvider;
        private string _serviceName;
        private bool _localExecution;
        private bool _requireIdempotency;

        public JsonRpcDispatcher(IService service) :
            this(service, null) {}

        public JsonRpcDispatcher(IService service, IServiceProvider serviceProvider)
        {
            if (service == null)
                throw new ArgumentNullException("service");

            _service = service;

            if (serviceProvider == null)
            {
                //
                // No service provider supplied so check if the RPC service
                // itself is our service provider.
                //

                serviceProvider = service as IServiceProvider;    

                //
                // If no service provider found so far, then create a default
                // one.
                //

                if (serviceProvider == null)
                    serviceProvider = new ServiceContainer();
            }

            _serviceProvider = serviceProvider;
        }

        internal void SetLocalExecution()
        {
            // TODO: Need to make this public but through a more generic set of options.

            _localExecution = true;
        }

        public bool RequireIdempotency
        {
            get { return _requireIdempotency; }
            set { _requireIdempotency = value; }
        }

        private string ServiceName
        {
            get
            {
                if (_serviceName == null)
                    _serviceName = JsonRpcServices.GetServiceName(_service);

                return _serviceName;
            }
        }

        public virtual string Process(string request)
        {
            StringWriter writer = new StringWriter();
            Process(new StringReader(request), writer);
            return writer.ToString();
        }

        public virtual void Process(TextReader input, TextWriter output)
        {
            if (input == null)
                throw new ArgumentNullException("input");

            if (output == null)
                throw new ArgumentNullException("output");

            IDictionary response;
            
            try
            {
                IDictionary request = (IDictionary) ParseRequest(input);
                response = Invoke(request);
            }
            catch (MethodNotFoundException e)
            {
                response = CreateResponse(null, null, OnError(e));
            }
            catch (JsonException e)
            {
                response = CreateResponse(null, null, OnError(e));
            }
            
            WriteResponse(response, output);
        }

        public virtual IDictionary Invoke(IDictionary request)
        {
            if (request == null)
                throw new ArgumentNullException();

            //
            // Get the ID of the request.
            //

            object id = request["id"];

            //
            // If the ID is not there or was not set then this is a notification
            // request from the client that does not expect any response. Right
            // now, we don't support this.
            //

            bool isNotification = JsonNull.LogicallyEquals(id);
            
            if (isNotification)
                throw new NotSupportedException("Notification are not yet supported.");

            if (JsonRpcTrace.TraceInfo)
                JsonRpcTrace.Info("Received request with the ID {0}.", id.ToString());

            //
            // Get the method name and arguments.
            //
    
            string methodName = Mask.NullString((string) request["method"]);

            if (methodName.Length == 0)
                throw new JsonRpcException("No method name supplied for this request.");

            if (JsonRpcTrace.Switch.TraceInfo)
                JsonRpcTrace.Info("Invoking method {1} on service {0}.", ServiceName, methodName);

            //
            // Invoke the method on the service and handle errors.
            //
    
            object error = null;
            object result = null;

            try
            {
                Method method = _service.GetClass().GetMethodByName(methodName);
                
                if (RequireIdempotency && !method.Idempotent)
                    throw new JsonRpcException(string.Format("Method {1} on service {0} is not allowed for idempotent type of requests.", ServiceName, methodName));
                        
                object[] args;
                string[] names = null;

                object argsObject = request["params"];
                IDictionary argByName = argsObject as IDictionary;
                
                if (argByName != null)
                {
                    names = new string[argByName.Count];
                    argByName.Keys.CopyTo(names, 0);
                    args = new object[argByName.Count];
                    argByName.Values.CopyTo(args, 0);
                }
                else
                {
                    args = CollectionHelper.ToArray((ICollection) argsObject);
                }
                
                result = method.Invoke(_service, names, args);
            }
            catch (MethodNotFoundException e)
            {
                error = OnError(e);
            }
            catch (InvocationException e)
            {
                error = OnError(e);
            }
            catch (TargetMethodException e)
            {
                error = OnError(e.InnerException);
            }
            catch (Exception e)
            {
                if (JsonRpcTrace.Switch.TraceError)
                    JsonRpcTrace.Error(e);

                throw;
            }

            //
            // Setup and return the response object.
            //

            return CreateResponse(id, result, error);
        }

        private static IDictionary CreateResponse(object id, object result, object error)
        {
            JsonObject response = new JsonObject();
            
            response["id"] = id;

            if (error != null)
                response["error"] = error;
            else
                response["result"] = result;

            return response;
        }

        protected virtual object ParseRequest(TextReader input)
        {
            if (input == null)
                throw new ArgumentNullException("input");

            JsonReader reader = (JsonReader) _serviceProvider.GetService(typeof(JsonReader));

            if (reader == null)
                reader = new JsonTextReader(input);
            
            ImportContext importContext = new ImportContext();
            
            JsonObject request = new JsonObject();
            Method method = null;
            JsonReader paramsReader = null;
            object args = null;
            
            reader.ReadToken(JsonTokenClass.Object);
            
            while (reader.TokenClass != JsonTokenClass.EndObject)
            {
                string memberName = reader.ReadMember();
                
                switch (memberName)
                {
                    case "id" :
                    {
                        request["id"] = importContext.Import(reader);
                        break;
                    }
                    
                    case "method" :
                    {
                        string methodName = reader.ReadString();
                        request["method"] = methodName;
                        method = _service.GetClass().GetMethodByName(methodName);
                        
                        if (paramsReader != null)
                        {
                            //
                            // If the parameters were already read in and
                            // buffer, then deserialize them now that we know
                            // the method we're dealing with.
                            //
                            
                            args = ReadParameters(method, paramsReader, importContext);
                            paramsReader = null;
                        }
                        
                        break;
                    }
                    
                    case "params" :
                    {
                        //
                        // Is the method already known? If so, then we can
                        // deserialize the parameters right away. Otherwise
                        // we record them until hopefully the method is
                        // encountered.
                        //
                        
                        if (method != null)
                        {
                            args = ReadParameters(method, reader, importContext);
                        }
                        else
                        {
                            JsonRecorder recorder = new JsonRecorder();
                            recorder.WriteFromReader(reader);
                            paramsReader = recorder.CreatePlayer();
                        }

                        break;
                    }
                        
                    default:
                    {
                        reader.Skip();
                        break;
                    }
                }
            }
            
            reader.Read();

            if (args != null)
                request["params"] = args;
            
            return request;
        }

        protected virtual void WriteResponse(object response, TextWriter output)
        {
            if (response == null)
                throw new ArgumentNullException("response");

            if (output == null)
                throw new ArgumentNullException("output");
            
            JsonWriter writer = (JsonWriter) _serviceProvider.GetService(typeof(JsonWriter));
            
            if (writer == null)
                writer = new JsonTextWriter(output);

            ExportContext exportContext = new ExportContext();
            exportContext.Export(response, writer);
        }

        protected virtual object OnError(Exception e)
        {
            if (JsonRpcTrace.Switch.TraceError)
                JsonRpcTrace.Error(e);

            return JsonRpcError.FromException(e, _localExecution);
        }

        private static object ReadParameters(Method method, JsonReader reader, ImportContext importContext)
        {
            Debug.Assert(method != null);
            Debug.Assert(reader != null);
            Debug.Assert(importContext != null);
            
            reader.MoveToContent();
            
            Parameter[] parameters = method.GetParameters();
                            
            if (reader.TokenClass == JsonTokenClass.Array)
            {
                reader.Read();
                ArrayList argList = new ArrayList(parameters.Length);
                                
                // TODO: This loop could bomb when more args are supplied that parameters available.
                                                        
                for (int i = 0; i < parameters.Length && reader.TokenClass != JsonTokenClass.EndArray; i++)
                    argList.Add(importContext.Import(parameters[i].ParameterType, reader));

                reader.StepOut();
                return argList.ToArray();
            }
            else if (reader.TokenClass == JsonTokenClass.Object)
            {
                reader.Read();
                JsonObject argByName = new JsonObject();
                                
                while (reader.TokenClass != JsonTokenClass.EndObject)
                {
                    // TODO: Imporve this lookup.
                    // FIXME: Does not work when argument is positional.
                                    
                    Type parameterType = AnyType.Value;
                    string name = reader.ReadMember();

                    foreach (Parameter parameter in parameters)
                    {
                        if (parameter.Name.Equals(name))
                        {
                            parameterType = parameter.ParameterType;
                            break;
                        }
                    }
                                    
                    argByName.Put(name, importContext.Import(parameterType, reader));
                }
                                
                reader.Read();
                return argByName;
            }
            else
            {
                return importContext.Import(reader);
            }
        }
    }
}
