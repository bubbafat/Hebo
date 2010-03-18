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

namespace Jayrock
{
    #region Imports

    using System;
    using System.Reflection;
    using NUnit.Framework;

    #endregion

    [ TestFixture ]
    public class TestUnixTime
    {
        [ Test ]
        public void CannotBeCreated()
        {
            try
            {
                Activator.CreateInstance(typeof(UnixTime), true);
                Assert.Fail();
            }
            catch (TargetInvocationException e)
            {
                Assert.IsTrue(e.InnerException is NotSupportedException);
            }
        }
        
        [ Test ]
        public void Conversions()
        {
            DateTime t1 = new DateTime(2006, 7, 17, 10, 56, 56);
            const long u1 = 1153133816;
            
            DateTime t2 = new DateTime(2006, 7, 17, 11, 00, 44);
            const long u2 = 1153134044;

            Assert.AreEqual(t1, UnixTime.ToDateTime(u1).ToUniversalTime());
            Assert.AreEqual(t2, UnixTime.ToDateTime(u2).ToUniversalTime());
            
            Assert.AreEqual(u1, UnixTime.ToInt64(t1.ToLocalTime()));
            Assert.AreEqual(u2, UnixTime.ToInt64(t2.ToLocalTime()));
        }
    }
}