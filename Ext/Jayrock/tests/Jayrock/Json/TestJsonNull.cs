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
    using System.Data.SqlTypes;
    using System.IO;
    using System.Runtime.Serialization.Formatters.Binary;
    using Jayrock.Json.Conversion;
    using NUnit.Framework;

    #endregion

    [ TestFixture ]
    public class TestJsonNull
    {
        [ Test ]
        public void Equality()
        {
            Assert.IsTrue(JsonNull.Value.Equals(JsonNull.Value));
        }

        [ Test ]
        public void ReferenceEqualityPostDeserialization()
        {
            MemoryStream stream = new MemoryStream();
            BinaryFormatter formatter = new BinaryFormatter();
            formatter.Serialize(stream, JsonNull.Value);
            stream.Position = 0;
            object o = formatter.Deserialize(stream);
            Assert.IsTrue(JsonNull.Value.Equals(o));
        }

        [ Test ]
        public void LogicalEquality()
        {
            Assert.IsTrue(JsonNull.LogicallyEquals(JsonNull.Value), "Equals self?");
            Assert.IsFalse(JsonNull.LogicallyEquals(new object()), "Equals non-nullable?");
            Assert.IsTrue(JsonNull.LogicallyEquals(null), "Equals null reference?");
            Assert.IsTrue(JsonNull.LogicallyEquals(DBNull.Value), "Equals DBNull?");
        }

        [ Test ]
        public void Export()
        {
            JsonRecorder writer = new JsonRecorder();
            ((IJsonExportable) JsonNull.Value).Export(new ExportContext(), writer);
            JsonReader reader = writer.CreatePlayer();
            reader.ReadNull();
            Assert.IsTrue(reader.EOF);
        }

        [ Test ]
        public void StringRepresentation()
        {
            Assert.AreEqual("null", JsonNull.Value.ToString());
        }
    }
}