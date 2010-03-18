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

namespace Jayrock.JsonRpc.Web
{
    #region Imports

    using System.Collections.Specialized;
    using System.IO;
    using System.Web;
    using Jayrock.Json;
    using Jayrock.Json.Conversion;
    using Jayrock.Services;

    #endregion

    public sealed class JsonRpcGetProtocol : JsonRpcServiceFeature
        // TODO: Add IHttpAsyncHandler as soon as JsonRpcWorker supports 
        //       async processing.
    {
        public JsonRpcGetProtocol(IService service) : 
            base(service) {}

        protected override void ProcessRequest()
        {
            string httpMethod = Request.RequestType;

            if (!CaselessString.Equals(httpMethod, "GET") &&
                !CaselessString.Equals(httpMethod, "HEAD"))
            {
                throw new JsonRpcException(string.Format("HTTP {0} is not supported for RPC execution. Use HTTP GET or HEAD only.", httpMethod));
            }

            //
            // Response will be plain text, though it would have been nice to 
            // be more specific, like text/json.
            //

            Response.ContentType = "text/plain";
            
            //
            // Convert the query string into a call object.
            //

            JsonWriter writer = new JsonTextWriter();
            
            writer.WriteStartObject();
            
            writer.WriteMember("id");
            writer.WriteNumber(-1);
            
            writer.WriteMember("method");
            
            string methodName = Mask.NullString(Request.PathInfo);
            
            if (methodName.Length == 0)
            {
                writer.WriteNull();
            }
            else
            {
                //
                // If the method name contains periods then we replace it
                // with dashes to mean the one and same thing. This is
                // done to provide dashes as an alternative to some periods
                // since some web servers may block requests (for security
                // reasons) if a path component of the URL contains more
                // than one period.
                //
                
                writer.WriteString(methodName.Substring(1).Replace('-', '.'));
            }
            
            writer.WriteMember("params");
            NameValueCollection query = new NameValueCollection(Request.QueryString);
            query.Remove(string.Empty);
            JsonConvert.Export(Request.QueryString, writer);
            
            writer.WriteEndObject();
            
            //
            // Delegate rest of the work to JsonRpcDispatcher.
            //

            JsonRpcDispatcher dispatcher = new JsonRpcDispatcher(Service);
            
            dispatcher.RequireIdempotency = true;
            
            if (HttpRequestSecurity.IsLocal(Request))
                dispatcher.SetLocalExecution();
            
            dispatcher.Process(new StringReader(writer.ToString()), Response.Output);
        }
    }
}