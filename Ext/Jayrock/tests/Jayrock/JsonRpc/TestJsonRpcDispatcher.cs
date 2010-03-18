#region License, Terms and Conditions
//
// Jayrock - A JSON-RPC implementation for the Microsoft .NET Framework
// Written by Atif Aziz (atif.aziz@skybow.com)
// Copyright (c) Atif Aziz. All rights reserved.
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
    using System.IO;
    using System.Reflection;
    using Jayrock.Json;
    using Jayrock.Json.Conversion;
    using Jayrock.Services;
    using NUnit.Framework;

    #endregion

    [ TestFixture ]
    public class TestJsonRpcDispatcher
    {
        [ SetUp ]
        public void Init()
        {
        }

        [ TearDown ]
        public void Dispose()
        {
        }

        [ Test, ExpectedException(typeof(NotSupportedException)) ]
        public void NotificationNotSupported()
        {
            JsonRpcDispatcher server = new JsonRpcDispatcher(new TestService());
            server.Process("{ id : null, method : 'Dummy' }");
        }

        [ Test ]
        public void SimpleCall()
        {
            JsonRpcDispatcher server = new JsonRpcDispatcher(new TestService());
            string responseString = server.Process("{ id : 1, method : 'Say', params : [ 'Hello' ] }");
            IDictionary response = (IDictionary) Parse(responseString);
            Assert.AreEqual(1, (int) (JsonNumber) response["id"]);
            Assert.AreEqual("Hello", response["result"]);
        }

        [ Test ]
        public void CallWithArrayResult()
        {
            JsonRpcDispatcher server = new JsonRpcDispatcher(new TestService());
            string responseString = server.Process("{ id : 'F9A2CC85-79A2-489f-AE61-84348654008C', method : 'replicate', params : [ 'Hello', 3 ] }");
            IDictionary response = (IDictionary) Parse(responseString);
            Assert.AreEqual("F9A2CC85-79A2-489f-AE61-84348654008C", response["id"]);
            object[] result = ((JsonArray) response["result"]).ToArray();
            Assert.AreEqual(new string[] { "Hello", "Hello", "Hello" }, result);
        }

        [ Test ]
        public void ProcWithArrayArg()
        {
            JsonRpcDispatcher server = new JsonRpcDispatcher(new TestService());
            string responseString = server.Process("{ id : 42, method : 'rev', params : [ [ 1, 'two', 3 ] ] }");
            IDictionary response = (IDictionary) Parse(responseString);
            object[] result = ((JsonArray) response["result"]).ToArray();
            Assert.AreEqual(3, (int) (JsonNumber) result[0]);
            Assert.AreEqual("two", result[1]);
            Assert.AreEqual(1, (int) (JsonNumber) result[2]);
        }

        [ Test ]
        public void CallWithNamedArgs()
        {
            JsonRpcDispatcher server = new JsonRpcDispatcher(new TestService());
            string responseString = server.Process("{ id : 42, method : 'replicate', params : { count : 3, text : 'Hello' } }");
            IDictionary response = (IDictionary) Parse(responseString);
            object[] result = ((JsonArray) JsonRpcServices.GetResult(response)).ToArray();
            Assert.AreEqual(new string[] { "Hello", "Hello", "Hello" }, result);
        }
        
        [ Test ]
        public void CallWithIntArray()
        {
            JsonRpcDispatcher server = new JsonRpcDispatcher(new TestService());
            string responseString = server.Process("{ id : 42, method : 'sum', params : [ [ 1, 2, 3, 4, 5 ] ] }");
            IDictionary response = (IDictionary) Parse(responseString);
            Assert.AreEqual(15, (int) (JsonNumber) JsonRpcServices.GetResult(response));
        }
        
        [ Test ]
        public void Bug8320()
        {
            //
            // Bug #8320: Parameter at Dispatcher without method are not handeld
            // http://developer.berlios.de/bugs/?func=detailbug&bug_id=8320&group_id=4638
            //

            JsonRpcDispatcher server = new JsonRpcDispatcher(new TestService());
            string responseString = server.Process("{ id : 42, params : [ [ 1, 2, 3, 4, 5 ] ], method : 'sum' }");
            IDictionary response = (IDictionary) Parse(responseString);
            Assert.AreEqual(15, (int) (JsonNumber) JsonRpcServices.GetResult(response));
        }
        
        [ Test ]
        public void CallWithTooManyArgsHarmless()
        {
            JsonRpcDispatcher server = new JsonRpcDispatcher(new TestService());
            string responseString = server.Process("{ id : 1, method : 'Say', params : [ 'Hello', 'World' ] }");
            IDictionary response = (IDictionary) Parse(responseString);
            Assert.AreEqual("Hello", JsonRpcServices.GetResult(response));
        }

        [ Test ]
        public void CallWithUnknownArgsHarmless()
        {
            JsonRpcDispatcher server = new JsonRpcDispatcher(new TestService());
            string responseString = server.Process("{ id : 1, method : 'Say', params : { message : 'Hello', bad : 'World' } }");
            IDictionary response = (IDictionary) Parse(responseString);
            Assert.AreEqual("Hello", JsonRpcServices.GetResult(response));
        }

        [ Test ]
        public void ArgWithOneOrTwoCharNameDroppedBug()
        {
            JsonRpcDispatcher server = new JsonRpcDispatcher(new TestService2());
            string responseString = server.Process("{ id : 1, method : 'Echo', params : { o : 123 } }");
            IDictionary response = (IDictionary) Parse(responseString);
            Assert.AreEqual(123, Convert.ToInt32(JsonRpcServices.GetResult(response)));
        }

        [ Test ]
        public void PositionNineArgDroppedBug()
        {
            JsonRpcDispatcher server = new JsonRpcDispatcher(new TestService2());
            string responseString = server.Process("{ id : 1, method : 'Echo', params : { o : 123 } }");
            IDictionary response = (IDictionary) Parse(responseString);
            Assert.AreEqual(123, Convert.ToInt32(JsonRpcServices.GetResult(response)));
        }

        [ Test ]
        public void CallWithPositionalArgs()
        {
            JsonRpcDispatcher server = new JsonRpcDispatcher(new TestService2());
            string responseString = server.Process("{ id : 1, method : 'EchoMany', params : { 0:11,1:12,2:13,3:14,4:15,5:16,6:17,7:18,8:19,9:20,10:21,11:22,12:23,13:24,14:25,15:26,16:27,17:28,18:29,19:30,20:31,21:32,22:33,23:34,24:35,25:36 } }");
            IDictionary response = (IDictionary) Parse(responseString);
            Assert.AreEqual(new int[] { 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 25, 26, 27, 28, 29, 30, 31, 32, 33, 34, 35, 36 }, JsonRpcServices.GetResult(response, typeof(int[])));
        }

        [ Test, ExpectedException(typeof(JsonRpcException)) ]
        public void CannotCallNonIdempotentMethodWhenIdempotencyRequired()
        {
            JsonRpcDispatcher server = new JsonRpcDispatcher(new TestService());
            server.RequireIdempotency = true;
            JsonRpcServices.GetResult((IDictionary) Parse(server.Process("{ id : 1, method : 'Dummy', params : [] }")));
        }

        [ Test ]
        public void CallIdempotentMethodWithIdempotencyRequired()
        {
            JsonRpcDispatcher server = new JsonRpcDispatcher(new TestService());
            server.RequireIdempotency = true;
            JsonRpcServices.GetResult((IDictionary) Parse(server.Process("{ id : 1, method : 'Idem', params : [] }")));
        }

        [ Test ]
        public void UnknownRequestMembersSkipped()
        {
            JsonRpcDispatcher server = new JsonRpcDispatcher(new TestService());
            JsonRpcServices.GetResult((IDictionary) Parse(server.Process("{ id : 1, foo : [bar], method : 'Dummy', params : [] }")));
        }

        private object Parse(string source)
        {
            return JsonConvert.Import(source);
        }

        private sealed class TestService : JsonRpcService
        {
            [ JsonRpcMethod ]
            public void Dummy()
            {                
            }
            
            [ JsonRpcMethod ]
            public string Say(string message)
            {
                return message;
            }

            [ JsonRpcMethod("replicate") ]
            public string[] Replicate(string text, int count)
            {
                return (string[]) ArrayList.Repeat(text, count).ToArray(typeof(string));
            }

            [ JsonRpcMethod("rev") ]
            public object[] Reverse(object[] values)
            {
                Array.Reverse(values);
                return values;
            }
            
            [ JsonRpcMethod("sum") ]
            public int Sum(int[] ints)
            {
                int sum = 0;
                foreach (int i in ints)
                    sum += i;
                return sum;
            }

            [ JsonRpcMethod(Idempotent = true) ]
            public void Idem()
            {                
            }
        }

        private class TestService2 : IService
        {
            public ServiceClass GetClass()
            {
                return JsonRpcServices.GetClassFromType(GetType());
            }
            
            [ JsonRpcMethod ]
            public object Echo(object o)
            {                
                return o;
            }

            [ JsonRpcMethod ] // FIXME: CallWithPositionalArgs test breaks when parameters are int.
            public object[] EchoMany(object a, object b, object c, object d, object e, object f, object g, object h, object i, object j, object k, object l, object m, object n, object o, object p, object q, object r, object s, object t, object u, object v, object w, object x, object y, object z)
            {                
                return new object[] { a, b, c, d, e, f, g, h, i, j, k, l, m, n, o, p, q, r, s, t, u, v, w, x, y, z };
            }
        }
    }
}
