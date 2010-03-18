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
    using System.Diagnostics;
    using System.Reflection;
    using Jayrock.Services;
    using NUnit.Framework;

    #endregion

    [ TestFixture ]
    public class TestJsonRpcServiceReflector
    {
        [ Test, ExpectedException(typeof(ArgumentNullException)) ]
        public void NullType()
        {
            JsonRpcServices.GetClassFromType(null);
        }

        [ Test ]
        public void ServiceNameIsTypeName()
        {
            ServiceClass clazz = JsonRpcServices.GetClassFromType(typeof(EmptyService));
            Assert.AreEqual("EmptyService", clazz.Name);
        }

        [ Test ]
        public void UntaggedMethodsNotExported()
        {
            ServiceClass clazz = JsonRpcServices.GetClassFromType(typeof(EmptyService));
            Assert.AreEqual(0, clazz.GetMethods().Length);
        }

        [ Test ]
        public void TaggedMethodsExported()
        {
            ServiceClass clazz = JsonRpcServices.GetClassFromType(typeof(TestService));
            Assert.AreEqual(4, clazz.GetMethods().Length);
        }

        [ Test ]
        public void CustomServiceName()
        {
            ServiceClass clazz = JsonRpcServices.GetClassFromType(typeof(TestService));
            Assert.AreEqual("MyService", clazz.Name);
        }
        
        [ Test, ExpectedException(typeof(ArgumentNullException)) ]
        public void NullService()
        {
            JsonRpcServices.GetClassFromType(null);
        }

        [ Test ]
        public void DefaultNameIsMethodName()
        {
            ServiceClass clazz = JsonRpcServices.GetClassFromType(typeof(TestService));
            Assert.AreEqual("Foo", clazz.FindMethodByName("Foo").Name);
        }

        [ Test ]
        public void AffliatedWithService()
        {
            ServiceClass clazz = JsonRpcServices.GetClassFromType(typeof(TestService));
            foreach (Method method in clazz.GetMethods())
                Assert.AreSame(clazz, method.ServiceClass);
        }

        [ Test ]
        public void CustomNameViaAttribute()
        {
            ServiceClass clazz = JsonRpcServices.GetClassFromType(typeof(TestService));
            Method method = clazz.FindMethodByName("Foo");
            Assert.AreEqual("Foo", method.Name);
            Assert.AreEqual("Foo", method.InternalName);
        }

        [ Test ]
        public void AttributeFromMethod()
        {
            ServiceClass clazz = JsonRpcServices.GetClassFromType(typeof(TestService));
            Method method = clazz.FindMethodByName("Baz");
            Assert.AreEqual("Baz", method.Name);
            Assert.AreEqual("Bar", method.InternalName);
        }

        [ Test ]
        public void ResultTypeIsMethodReturnType()
        {
            ServiceClass clazz = JsonRpcServices.GetClassFromType(typeof(TestService));
            Assert.AreEqual(typeof(void), clazz.GetMethodByName("Foo").ResultType);
            Assert.AreEqual(typeof(void), clazz.GetMethodByName("Baz").ResultType);
            Assert.AreEqual(typeof(int), clazz.GetMethodByName("Sum").ResultType);
            Assert.AreEqual(typeof(string), clazz.GetMethodByName("Format").ResultType);
        }

        [ Test ]
        public void Parameters()
        {
            ServiceClass clazz = JsonRpcServices.GetClassFromType(typeof(TestService));
            Parameter[] parameters = clazz.GetMethodByName("Sum").GetParameters();
            Assert.AreEqual(2, parameters.Length);

            Parameter parameter; 
            
            parameter = parameters[0];
            Assert.AreEqual("a", parameter.Name);
            Assert.AreEqual(typeof(int), parameter.ParameterType);
            Assert.AreEqual(0, parameter.Position);
            
            parameter = parameters[1];
            Assert.AreEqual("b", parameters[1].Name);
            Assert.AreEqual(typeof(int), parameter.ParameterType);
            Assert.AreEqual(1, parameter.Position);
        }

        [ Test ]
        public void ParamArray()
        {
            ServiceClass clazz = JsonRpcServices.GetClassFromType(typeof(TestService));

            Assert.IsFalse(clazz.GetMethodByName("Foo").HasParamArray);
            Assert.IsFalse(clazz.GetMethodByName("Baz").HasParamArray);

            Method method;
            Parameter[] parameters;

            method = clazz.GetMethodByName("Sum");
            parameters = method.GetParameters();
            Assert.IsFalse(method.HasParamArray);
            Assert.IsFalse(parameters[0].IsParamArray);
            Assert.IsFalse(parameters[1].IsParamArray);

            method = clazz.GetMethodByName("Format");
            parameters = method.GetParameters();
            Assert.IsTrue(method.HasParamArray);
            Assert.IsFalse(parameters[0].IsParamArray);
            Assert.IsTrue(parameters[1].IsParamArray);
        }
        
        [ Test ]
        public void ObsoletedMethods()
        {
            ServiceClass clazz = JsonRpcServices.GetClassFromType(typeof(TestService));
            
            Method method;
            
            method = clazz.GetMethodByName("Foo");
            Assert.IsNull(method.FindFirstCustomAttribute(typeof(ObsoleteAttribute)));
            
            method = clazz.GetMethodByName("Baz");
            Assert.IsNull(method.FindFirstCustomAttribute(typeof(ObsoleteAttribute)));

            method = clazz.GetMethodByName("Sum");
            JsonRpcObsoleteAttribute attribute = (JsonRpcObsoleteAttribute) method.FindFirstCustomAttribute(typeof(JsonRpcObsoleteAttribute));
            Assert.IsNotNull(attribute);
            Assert.AreEqual("Obsoleted.", attribute.Message);
            
            method = clazz.GetMethodByName("Format");
            Assert.IsNull(method.FindFirstCustomAttribute(typeof(ObsoleteAttribute)));
        }

        [ Test ]
        public void Invocation()
        {
            ServiceClass clazz = JsonRpcServices.GetClassFromType(typeof(TestService));
            TestService service = new TestService();
            object result = clazz.GetMethodByName("Sum").Invoke(service, null, new object[] { 2, 3 });
            Assert.AreEqual(5, result);
        }
    
        [ Test ]
        public void MethodDescriptions()
        {
            ServiceClass clazz = JsonRpcServices.GetClassFromType(typeof(TestService));
            Assert.AreEqual(0, clazz.GetMethodByName("Foo").Description.Length);
            Assert.AreEqual(0, clazz.GetMethodByName("Baz").Description.Length);
            Assert.AreEqual(0, clazz.GetMethodByName("Sum").Description.Length);
            Assert.AreEqual("Formats a string.", clazz.GetMethodByName("Format").Description);
        }
        
        [ Test ]
        public void ServiceDescription()
        {
            Assert.AreEqual("A test service.", JsonRpcServices.GetClassFromType(typeof(TestService)).Description);
        }
        
        [ Test ]
        public void CustomAttributes()
        {
            ArrayList expectedValues = new ArrayList(new int[] { 12, 34, 56 });
            Attribute[] attributes = JsonRpcServices.GetClassFromType(typeof(TestService)).GetMethodByName("Foo").GetCustomAttributes();
            Assert.AreEqual(3, attributes.Length);
            foreach (MyAttribute attribute in attributes)
                expectedValues.Remove(attribute.TestValue);
            Assert.AreEqual(0, expectedValues.Count);
        }

        [ Test ]
        public void CustomAttributesAreCopied()
        {
            Method method = JsonRpcServices.GetClassFromType(typeof(TestService)).GetMethodByName("Foo");
            Assert.AreNotSame(method.GetCustomAttributes()[0], method.GetCustomAttributes()[0]);
        }

        [ Test ]
        public void FindFirstCustomAttribute()
        {
            ArrayList expectedValues = new ArrayList(new int[] { 12, 34, 56 });
            Method method = JsonRpcServices.GetClassFromType(typeof(TestService)).GetMethodByName("Foo");
            MyAttribute attribute = (MyAttribute) method.FindFirstCustomAttribute(typeof(MyAttribute));
            expectedValues.Remove(attribute.TestValue);
            Assert.AreEqual(2, expectedValues.Count);
        }

        [ Test ]
        public void FindFirstCustomAttributeYieldsCopy()
        {
            Method method = JsonRpcServices.GetClassFromType(typeof(TestService)).GetMethodByName("Foo");
            Assert.AreNotSame(method.FindFirstCustomAttribute(typeof(MyAttribute)), method.FindFirstCustomAttribute(typeof(MyAttribute)));
        }

        [ Test ]
        public void MethodIdempotency()
        {
            ServiceClass clazz = JsonRpcServices.GetClassFromType(typeof(IdempotencyTestService));
            Assert.IsFalse(clazz.GetMethodByName("NonIdempotentMethod").Idempotent);
            Assert.IsTrue(clazz.GetMethodByName("IdempotentMethod").Idempotent);
        }
        
        private sealed class EmptyService
        {
        }

        [ JsonRpcService("MyService") ]
        [ JsonRpcHelp("A test service.") ]
        private sealed class TestService : IService
        {
            [ JsonRpcMethod ]
            [ MyAttribute(12), MyAttribute(56), MyAttribute(34) ]
            public void Foo() { throw new NotImplementedException(); }

            [ JsonRpcMethod("Baz") ]
            public void Bar() { throw new NotImplementedException(); }

            [ JsonRpcMethod, JsonRpcObsolete("Obsoleted.") ]
            public int Sum(int a, int b)
            {
                return a + b;
            }

            [ JsonRpcMethod, JsonRpcHelp("Formats a string.") ]
            public string Format(string format, params object[] args)
            {
                throw new NotImplementedException();
            }

            public ServiceClass GetClass()
            {
                throw new NotImplementedException();
            }
        }
        
        private sealed class IdempotencyTestService
        {
            [ JsonRpcMethod ]
            public void NonIdempotentMethod() {}

            [ JsonRpcMethod(Idempotent = true) ]
            public void IdempotentMethod() {}
        }

        [ AttributeUsage(AttributeTargets.All, AllowMultiple = true) ]
        private class MyAttribute : Attribute, ICloneable
        {
            private int _testValue;

            public MyAttribute(int testValue)
            {
                _testValue = testValue;
            }

            public int TestValue
            {
                get { return _testValue; }
            }

            public object Clone()
            {
                return MemberwiseClone();
            }
        }
    }
}
