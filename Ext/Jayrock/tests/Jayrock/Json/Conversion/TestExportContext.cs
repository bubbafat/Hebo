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

    using System;
    using System.Collections;
    using System.Collections.Specialized;
    using System.Data;
    using System.Web.UI;
    using System.Web.UI.HtmlControls;
    using Jayrock.Json.Conversion.Converters;
    using NUnit.Framework;

    #endregion

    [ TestFixture ]
    public class TestExportContext
    {
        [ Test ]
        public void StockExporters()
        {
            AssertInStock(typeof(ByteExporter), typeof(byte));
            AssertInStock(typeof(Int16Exporter), typeof(short));
            AssertInStock(typeof(Int32Exporter), typeof(int));
            AssertInStock(typeof(Int64Exporter), typeof(long));
            AssertInStock(typeof(SingleExporter), typeof(float));
            AssertInStock(typeof(DoubleExporter), typeof(double));
            AssertInStock(typeof(DateTimeExporter), typeof(DateTime));
            AssertInStock(typeof(StringExporter), typeof(string));
            AssertInStock(typeof(BooleanExporter), typeof(bool));
            AssertInStock(typeof(ComponentExporter), typeof(object));
            AssertInStock(typeof(EnumerableExporter), typeof(object[]));
            AssertInStock(typeof(NameValueCollectionExporter), typeof(NameValueCollection));
            AssertInStock(typeof(StringExporter), typeof(System.Globalization.UnicodeCategory));
            AssertInStock(typeof(ExportAwareExporter), typeof(JsonObject));
            AssertInStock(typeof(DictionaryExporter), typeof(Hashtable));
            AssertInStock(typeof(ExportAwareExporter), typeof(JsonArray));
            AssertInStock(typeof(EnumerableExporter), typeof(ArrayList));
            AssertInStock(typeof(ExportAwareExporter), typeof(ExportableThing));
            AssertInStock(typeof(ControlExporter), typeof(Control));
            AssertInStock(typeof(ControlExporter), typeof(HtmlControl));
            AssertInStock(typeof(ControlExporter), typeof(HtmlImage));
            AssertInStock(typeof(DataSetExporter), typeof(DataSet));
            AssertInStock(typeof(DataSetExporter), typeof(MyDataSet));
            AssertInStock(typeof(DataTableExporter), typeof(DataTable));
            AssertInStock(typeof(DataTableExporter), typeof(MyDataTable));
            AssertInStock(typeof(DataRowExporter), typeof(DataRow));
            AssertInStock(typeof(DataRowExporter), typeof(MyDataRow));
            AssertInStock(typeof(DataRowViewExporter), typeof(DataRowView));
            AssertInStock(typeof(StringExporter), typeof(Guid));
            AssertInStock(typeof(ByteArrayExporter), typeof(byte[]));
        }

        [ Test ]
        public void HasItems()
        {
            Assert.IsNotNull((new ExportContext()).Items);
        }

        [ Test ]
        public void Registration()
        {
            ExportContext context = new ExportContext();
            ThingExporter exporter = new ThingExporter();
            context.Register(exporter);
            Assert.AreSame(exporter, context.FindExporter(typeof(Thing)));
        }

        [ Test ]
        public void RegistrationIsPerContext()
        {
            ExportContext context = new ExportContext();
            ThingExporter exporter = new ThingExporter();
            context.Register(exporter);
            context = new ExportContext();
            Assert.AreNotSame(exporter, context.FindExporter(typeof(Thing)));
        }

        private static void AssertInStock(Type expected, Type type)
        {
            ExportContext context = new ExportContext();
            IExporter exporter = context.FindExporter(type);
            Assert.IsNotNull(exporter, "No exporter found for {0}", type.FullName);
            Assert.AreSame(type, exporter.InputType, "{0} reported {1} when expecting {2}.", exporter, exporter.InputType, type);
            Assert.IsInstanceOfType(expected, exporter, type.FullName);
        }
        
        private sealed class ExportableThing : IJsonExportable
        {
            public void Export(ExportContext context, JsonWriter writer)
            {
                throw new NotImplementedException();
            }
        }

        private sealed class MyDataSet : DataSet
        {
        }
        
        private sealed class MyDataTable : DataTable
        {
        }
        
        private sealed class MyDataRow : DataRow
        {
            public MyDataRow(DataRowBuilder builder) : 
                base(builder) {}
        }        
        
        private sealed class Thing {}

        private sealed class ThingExporter : IExporter
        {
            public Type InputType
            {
                get { return typeof(Thing); }
            }

            public void Export(ExportContext context, object value, JsonWriter writer)
            {
                throw new NotImplementedException();
            }
        }
    }
}