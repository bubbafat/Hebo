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

namespace Jayrock
{
    #region Imports

    using System;
    using System.Collections;
    using System.Diagnostics;
    using System.Globalization;

    #endregion

    /// <summary> 
    /// Helper methods for array containing culturally-invariant strings.
    /// The main reason for this helper is to help with po
    /// </summary>
 
    internal sealed class InvariantStringArray
    {
        public static void Sort(string[] keys, Array items)
        {
            Debug.Assert(keys != null);
            Debug.Assert(items != null);

            Array.Sort(keys, items, InvariantComparer);
        }

        public static int BinarySearch(string[] values, string sought)
        {
            Debug.Assert(values != null);

            return Array.BinarySearch(values, sought, InvariantComparer);
        }
        
        private static IComparer InvariantComparer
        {
            get
            {
#if NET_1_0
                return StringComparer.DefaultInvariant;
#else
                return Comparer.DefaultInvariant;
#endif
            }
        }

#if NET_1_0
        
        [ Serializable ]
        private sealed class StringComparer : IComparer
        {
            private CompareInfo _compareInfo;
            
            public static readonly StringComparer DefaultInvariant = new StringComparer(CultureInfo.InvariantCulture);

            private StringComparer(CultureInfo culture)
            {
                Debug.Assert(culture != null);
                
                _compareInfo = culture.CompareInfo;
            }

            public int Compare(object x, object y)
            {
                if (x == y) 
                    return 0;
                else if (x == null) 
                    return -1;
                else if (y == null) 
                    return 1;
                else
                    return _compareInfo.Compare((string) x, (string) y);
            }
        }

#endif
        
        private InvariantStringArray() {}
    }
}