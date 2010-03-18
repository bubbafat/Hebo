#region License, Terms and Conditions
//
// The MIT License
// Copyright (c) 2006, Atif Aziz. All rights reserved.
//
// Permission is hereby granted, free of charge, to any person obtaining a 
// copy of this software and associated documentation files 
// (the "Software"), to deal in the Software without restriction, 
// including without limitation the rights to use, copy, modify, merge, 
// publish, distribute, sublicense, and/or sell copies of the Software, 
// and to permit persons to whom the Software is furnished to do so, subject 
// to the following conditions:
//
// The above copyright notice and this permission notice shall be included 
// in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS 
// OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF 
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. 
// IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY 
// CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, 
// TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE 
// SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//
//
// Author(s):
//  Atif Aziz (http://www.raboof.com)
//
#endregion

namespace TidyJson
{
    #region Imports

    using System;

    #endregion

    /// <summary>
    /// Provides masking services where one value masks another given a test.
    /// </summary>

    internal sealed class Mask
    {
        public static string NullString(string actual) 
        {
            return actual == null ? string.Empty : actual;
        }

        public static string NullString(string actual, string mask) 
        {
            return actual == null ? mask : actual;
        }

        public static string EmptyString(string actual, string emptyValue) 
        {
            return Mask.NullString(actual).Length == 0 ? emptyValue : actual;
        }

        private Mask()
        {
            throw new NotSupportedException();
        }
    }
}
