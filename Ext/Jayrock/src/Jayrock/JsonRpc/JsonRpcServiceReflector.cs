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
    using System.Diagnostics;
    using System.Reflection;
    using Jayrock.Services;

    #endregion

    internal sealed class JsonRpcServiceReflector
    {
        private static readonly Hashtable _classByTypeCache = Hashtable.Synchronized(new Hashtable());
        
        public static ServiceClass FromType(Type type)
        {
            if (type == null)
                throw new ArgumentNullException("type");

            ServiceClass clazz = (ServiceClass) _classByTypeCache[type];

            if (clazz == null)
            {
                clazz = BuildFromType(type);
                _classByTypeCache[type] = clazz;
            }

            return clazz;
        }

        private static ServiceClass BuildFromType(Type type)
        {
            ServiceClassBuilder builder = new ServiceClassBuilder();
            BuildClass(builder, type);
            return builder.CreateClass();
        }

        private static void BuildClass(ServiceClassBuilder builder, Type type)
        {
            //
            // Build via attributes.
            //

            object[] attributes = type.GetCustomAttributes(typeof(IServiceClassReflector), true);
            foreach (IServiceClassReflector reflector in attributes)
                reflector.Build(builder, type);
            
            //
            // Fault in the type name if still without name.
            //

            if (builder.Name.Length == 0)
                builder.Name = type.Name;

            //
            // Get all the public instance methods on the type and create a
            // filtered table of those to expose from the service.
            //
            
            MethodInfo[] publicMethods = type.GetMethods(BindingFlags.Public | BindingFlags.Instance);

            foreach (MethodInfo method in publicMethods)
            {
                if (ShouldBuild(method))
                    BuildMethod(builder.DefineMethod(), method);
            }
        }

        private static bool ShouldBuild(MethodInfo method)
        {
            Debug.Assert(method != null);
            
            return !method.IsAbstract && Attribute.IsDefined(method, typeof(JsonRpcMethodAttribute));
        }

        private static void BuildMethod(MethodBuilder builder, MethodInfo method)
        {
            Debug.Assert(method != null);
            Debug.Assert(builder != null);

            builder.InternalName = method.Name;
            builder.ResultType = method.ReturnType;
            builder.Handler = new TypeMethodImpl(method);
            
            //
            // Build via attributes.
            //
            
            object[] attributes = method.GetCustomAttributes(typeof(Attribute), true);
            foreach (Attribute attribute in attributes)
            {
                IMethodReflector reflector = attribute as IMethodReflector;
                
                if (reflector != null)
                    reflector.Build(builder, method);
                else 
                    builder.AddCustomAttribute(attribute);
            }
            
            //
            // Fault in the method name if still without name.
            //
            
            if (builder.Name.Length == 0)
                builder.Name = method.Name;

            //
            // Build the method parameters.
            //

            foreach (ParameterInfo parameter in method.GetParameters())
                BuildParameter(builder.DefineParameter(), parameter);
        }

        private static void BuildParameter(ParameterBuilder builder, ParameterInfo parameter)
        {
            Debug.Assert(parameter != null);
            Debug.Assert(builder != null);
            
            builder.Name = parameter.Name;
            builder.ParameterType = parameter.ParameterType;
            builder.Position = parameter.Position;
            builder.IsParamArray = parameter.IsDefined(typeof(ParamArrayAttribute), true);

            //
            // Build via attributes.
            //
            
            object[] attributes = parameter.GetCustomAttributes(typeof(IParameterReflector), true);
            foreach (IParameterReflector reflector in attributes)
                reflector.Build(builder, parameter);
        }

        private JsonRpcServiceReflector()
        {
            throw new NotSupportedException();
        }
    }

    internal interface IServiceClassReflector
    {
        void Build(ServiceClassBuilder builder, Type type);
    }

    internal interface IMethodReflector
    {
        void Build(MethodBuilder builder, MethodInfo method);
    }

    internal interface IParameterReflector
    {
        void Build(ParameterBuilder builder, ParameterInfo parameter);
    }
}

