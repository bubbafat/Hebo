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

namespace Jayrock.Json
{
    #region Imports

    using System;
    using Jayrock.Json.Conversion;
    using NUnit.Framework;

    #endregion

    [ TestFixture ]
    public class TestJsonNumber
    {
        [ Test ]
        public void DefaultConstructionEqualsZero()
        {
            Assert.AreEqual("0", (new JsonNumber()).ToString());
        }

        [ Test ]
        public void One()
        {
            Assert.AreEqual("1", Number("1").ToString());
        }

        [ Test ]
        public void Float()
        {
            Assert.AreEqual("1.2345", Number("1.2345").ToString());
        }

        [ Test ]
        public void NullMeansZero()
        {
            Assert.AreEqual("0", Number(null).ToString());
        }

        [ Test, ExpectedException(typeof(ArgumentException)) ]
        public void CannotInitWithBadNumber()
        {
            Number("one");
        }
        
        [ Test ]
        public void ToInt32()
        {
            Assert.AreEqual(123456789, Number("123456789").ToInt32());
        }
        
        [ Test ]
        public void ToInt16()
        {
            Assert.AreEqual(12345, Number("12345").ToInt16());
        }

        [ Test ]
        public void ToByte()
        {
            Assert.AreEqual(123, Number("123").ToByte());
        }

        [ Test ]
        public void ToChar()
        {
            Assert.AreEqual('\x2126', Number("8486").ToChar());
        }

        [ Test ]
        public void ToBoolean()
        {
            Assert.AreEqual(false, Number(null).ToBoolean());
            Assert.AreEqual(false, Number("0").ToBoolean());
            Assert.AreEqual(true, Number("1").ToBoolean());
            Assert.AreEqual(true, Number("-123").ToBoolean());
            Assert.AreEqual(true, Number("123").ToBoolean());
        }

        [ Test ]
        public void ToInt64()
        {
            Assert.AreEqual(123456789012345, Number("123456789012345").ToInt64());
        }

        [ Test ]
        public void ToSingle()
        {
            Assert.AreEqual(1.5f, Number("1.5").ToSingle());
        }

        [ Test ]
        public void ToDouble()
        {
            Assert.AreEqual(1.5, Number("1.5").ToDouble());
        }

        [ Test ]
        public void ToDecimal()
        {
            Assert.AreEqual(1.5m, Number("1.5").ToDecimal());
        }

        [ Test ]
        public void ToDateTime()
        {
            Assert.AreEqual(new DateTime(2006, 7, 17, 10, 56, 56), Number("1153133816").ToDateTime().ToUniversalTime());
        }

        [ Test ]
        public void LogicalEquality()
        {
            Assert.IsFalse(Number(null).LogicallyEquals(null), "null");
            Assert.IsTrue(Number("123").LogicallyEquals(123), "integer");
            Assert.IsTrue(Number("123.5").LogicallyEquals(123.5m), "decimal");
        }
        
        [ Test ]
        public void TypeCodeIsObject()
        {
            IConvertible n = new JsonNumber();
            Assert.AreEqual(TypeCode.Object, n.GetTypeCode());
        }
        
        [ Test ]
        public void Export()
        {
            TestJsonWriter writer = new TestJsonWriter();
            IJsonExportable number = Number("123");
            number.Export(new ExportContext(), writer);
            Assert.AreEqual("123", writer.WrittenValue);
        }

        private static JsonNumber Number(string s)
        {
            return new JsonNumber(s);
        }

        private sealed class TestJsonWriter : JsonWriterBase
        {
            public string WrittenValue;
            
            protected override void WriteStartObjectImpl()
            {
                throw new NotImplementedException();
            }

            protected override void WriteEndObjectImpl()
            {
                throw new NotImplementedException();
            }

            protected override void WriteMemberImpl(string name)
            {
                throw new NotImplementedException();
            }

            protected override void WriteStartArrayImpl()
            {
            }

            protected override void WriteEndArrayImpl()
            {
            }

            protected override void WriteStringImpl(string value)
            {
                throw new NotImplementedException();
            }

            protected override void WriteNumberImpl(string value)
            {
                WrittenValue = value;
            }

            protected override void WriteBooleanImpl(bool value)
            {
                throw new NotImplementedException();
            }

            protected override void WriteNullImpl()
            {
                throw new NotImplementedException();
            }
        }
    }
}