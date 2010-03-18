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

    using System;
    using System.IO;
    using System.Web;
    using Jayrock.Services;

    #endregion

    public sealed class JsonRpcExecutive : JsonRpcServiceFeature
        // TODO: Add IHttpAsyncHandler as soon as JsonRpcWorker supports 
        //       async processing.
    {
        public JsonRpcExecutive(IService service) : 
            base(service) {}

        protected override void ProcessRequest()
        {
            if (!CaselessString.Equals(Request.RequestType, "POST"))
            {
                throw new JsonRpcException(string.Format("HTTP {0} is not supported for RPC execution. Use HTTP POST only.", Request.RequestType));
            }

            //
            // Sets the "Cache-Control" header value to "no-cache".
            // NOTE: It does not send the common HTTP 1.0 request directive
            // "Pragma" with the value "no-cache".
            //

            Response.Cache.SetCacheability(HttpCacheability.NoCache);

            //
            // Response will be plain text, though it would have been nice to 
            // be more specific, like text/json.
            //

            Response.ContentType = "text/plain";

            //
            // Delegate rest of the work to JsonRpcServer.
            //

            JsonRpcDispatcher dispatcher = new JsonRpcDispatcher(Service);
            
            if (HttpRequestSecurity.IsLocal(Request))
                dispatcher.SetLocalExecution();

            using (TextReader reader = GetRequestReader())
                dispatcher.Process(reader, Response.Output);
        }

        private TextReader GetRequestReader()
        {
            if (CaselessString.Equals(Request.ContentType, "application/x-www-form-urlencoded"))
            {
                string request = Request.Form.Count == 1 ? Request.Form[0] : Request.Form["JSON-RPC"];
                return new StringReader(request);
            }
            else
            {
                return new StreamReader(Request.InputStream, Request.ContentEncoding);
            }
        }

        /*
        public IAsyncResult BeginProcessRequest(HttpContext context, AsyncCallback cb, object extraData)
        {
            throw new NotImplementedException();
        }

        public void EndProcessRequest(IAsyncResult result)
        {
            throw new NotImplementedException();
        }
        */
    }
}
