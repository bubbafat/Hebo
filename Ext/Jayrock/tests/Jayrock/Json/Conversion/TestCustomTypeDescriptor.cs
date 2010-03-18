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

namespace Jayrock.Json.Conversion
{
    #region Imports

    using NUnit.Framework;

    #endregion

    [ TestFixture ]
    public class TestCustomTypeDescriptor
    {
        [ Test ]
        public void MembersWithIgnoreAttributeExcluded()
        {
            CustomTypeDescriptor thingType = new CustomTypeDescriptor(typeof(Thing));
            Assert.AreEqual(4, thingType.GetProperties().Count);
            Assert.IsNull(thingType.GetProperties().Find("Field2", true));
            Assert.IsNull(thingType.GetProperties().Find("Property2", true));
        }
        
        public sealed class Thing
        {
            public object Field1;
            [ JsonIgnore ] public object Field2;
            public object Field3;

            public object Property1 { get { return null; } set { } }
            [ JsonIgnore ] public object Property2 { get { return null; } set { } }
            public object Property3 { get { return null; } set { } }
        }
   }
}