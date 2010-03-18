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

namespace Jayrock.Services
{
    #region Imports

    using System;
    using System.Reflection;

    #endregion

    [ Serializable ]
    public sealed class TypeMethodImpl : IMethodImpl
    {
        private readonly MethodInfo _method;

        public TypeMethodImpl(MethodInfo method)
        {
            if (method == null)
                throw new ArgumentNullException("method");
                
            _method = method;
        }

        public MethodInfo Method
        {
            get { return _method; }
        }

        public object Invoke(IService service, object[] args)
        {
            if (service == null)
                throw new ArgumentNullException("service");

            try
            {
                return _method.Invoke(service, args);
            }
            catch (ArgumentException e)
            {
                throw TranslateException(e);
            }
            catch (TargetParameterCountException e)
            {
                throw TranslateException(e);
            }
            catch (TargetInvocationException e)
            {
                throw TranslateException(e);
            }
        }

        public IAsyncResult BeginInvoke(IService service, object[] args, AsyncCallback callback, object asyncState)
        {
            if (service == null)
                throw new ArgumentNullException("service");

            SynchronousAsyncResult asyncResult;

            try
            {
                object result = Invoke(service, args);
                asyncResult = SynchronousAsyncResult.Success(asyncState, result);
            }
            catch (Exception e)
            {
                asyncResult = SynchronousAsyncResult.Failure(asyncState, e);
            }

            if (callback != null)
                callback(asyncResult);
                
            return asyncResult;
        }

        public object EndInvoke(IAsyncResult asyncResult)
        {
            if (asyncResult == null)
                throw new ArgumentNullException("asyncResult");

            SynchronousAsyncResult ar = asyncResult as SynchronousAsyncResult;

            if (ar == null)
                throw new ArgumentException("asyncResult", "IAsyncResult object did not come from the corresponding async method on this type.");

            try
            {
                //
                // IMPORTANT! The End method on SynchronousAsyncResult will 
                // throw an exception if that's what Invoke did when 
                // BeginInvoke called it. The unforunate side effect of this is
                // the stack trace information for the exception is lost and 
                // reset to this point. There seems to be a basic failure in the 
                // framework to accommodate for this case more generally. One 
                // could handle this through a custom exception that wraps the 
                // original exception, but this assumes that an invocation will 
                // only throw an exception of that custom type. We need to 
                // think more about this.
                //

                return ar.End("Invoke");
            }
            catch (ArgumentException e)
            {
                throw TranslateException(e);
            }
            catch (TargetParameterCountException e)
            {
                throw TranslateException(e);
            }
            catch (TargetInvocationException e)
            {
                throw TranslateException(e);
            }
        }

        private Exception TranslateException(ArgumentException e)
        {
            //
            // The type of the parameter does not match the signature
            // of the method or constructor reflected by this
            // instance.
            //

            return new InvocationException(e);
        }

        private static Exception TranslateException(TargetParameterCountException e)
        {
            //
            // The parameters array does not have the correct number of
            // arguments.
            //

            return new InvocationException(e.Message, e);
        }

        private static Exception TranslateException(TargetInvocationException e)
        {
            return new TargetMethodException(e.InnerException);
        }
    }
}