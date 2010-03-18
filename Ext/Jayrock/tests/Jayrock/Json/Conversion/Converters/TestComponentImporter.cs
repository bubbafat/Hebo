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

namespace Jayrock.Json.Conversion.Converters
{
    #region Imports

    using System;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.IO;
    using Jayrock.Json.Conversion;
    using NUnit.Framework;

    #endregion

    [ TestFixture ]
    public class TestComponentImporter
    {
        [ Test ]
        public void ImportNull()
        {
            ComponentImporter importer = new ComponentImporter(typeof(object));
            Assert.IsNull(importer.Import(new ImportContext(), CreateReader("null")));
        }

        [ Test ]
        public void ImportEmptyObject()
        {
            Marriage m = (Marriage) Import(typeof(Marriage), "{}");
            Assert.IsNull(m.Husband, "Husband");
            Assert.IsNull(m.Wife, "Wife");
        }
                
        [ Test ]
        public void ImportObject()
        {
            Person p = (Person) Import(typeof(Person), "{ Id : 42, FullName : 'Charles Dickens' }");
            Assert.AreEqual(42, p.Id, "Id");
            Assert.AreEqual("Charles Dickens", p.FullName, "FullName");
        }

        [ Test ]
        public void ImportEmbeddedObjects()
        {
            Marriage m = (Marriage) Import(typeof(Marriage), @"{
                Husband : {
                    Id : 42,
                    FullName : 'Bob'
                },
                Wife : { 
                    FullName : 'Alice', 
                    Id       : 43
                } 
            }");
            Assert.AreEqual(42, m.Husband.Id, "Husband.Id");
            Assert.AreEqual("Bob", m.Husband.FullName, "Husband.FullName");
            Assert.AreEqual(43, m.Wife.Id, "Wife.Id");
            Assert.AreEqual("Alice", m.Wife.FullName, "Wife.FullName");
        }
        
        [ Test ]
        public void YahooNewsSearch()
        {
            string text = @"
            /* Source: http://api.search.yahoo.com/NewsSearchService/V1/newsSearch?appid=YahooDemo&query=yahoo&results=3&language=en&output=json */
            {
                'ResultSet': {
                    'totalResultsAvailable': '2393',
                    'totalResultsReturned': 3,
                    'firstResultPosition': '1',
                    'Result': [
                        {
                            'Title': 'Yahoo invites its users to shoot ads',
                            'Summary': ' Yahoo first encouraged consumers to create blogs and photo pages with text and pictures. Now, the Internet portal wants them to make advertisements, too. On Monday, Yahoo touts a new look for its front page by asking people to pull out the video camera, open up the editing software and create 12-second spot for Yahoo.',
                            'Url': 'http://news.yahoo.com/s/usatoday/20060717/tc_usatoday/yahooinvitesitsuserstoshootads',
                            'ClickUrl': 'http://news.yahoo.com/s/usatoday/20060717/tc_usatoday/yahooinvitesitsuserstoshootads',
                            'NewsSource': 'USATODAY.com via Yahoo! News',
                            'NewsSourceUrl': 'http://news.yahoo.com/',
                            'Language': 'en',
                            'PublishDate': '1153133816',
                            'ModificationDate': '1153134044'
                        },
                        {
                            'Title': 'Yahoo to launch new finance features',
                            'Summary': ' Yahoo Inc. is beefing up the finance section of its Web site with more interactive stock charts and other features to help it maintain its longtime lead over rival financial information sites.',
                            'Url': 'http://news.yahoo.com/s/ap/20060717/ap_on_hi_te/yahoo_finance_2',
                            'ClickUrl': 'http://news.yahoo.com/s/ap/20060717/ap_on_hi_te/yahoo_finance_2',
                            'NewsSource': 'AP via Yahoo! News',
                            'NewsSourceUrl': 'http://news.yahoo.com/',
                            'Language': 'en',
                            'PublishDate': '1153134777',
                            'ModificationDate': '1153134920',
                            'Thumbnail': {
                                'Url': 'http://us.news2.yimg.com/us.yimg.com/p/ap/20060714/vsthumb.8b1161b66b564adba0a5bbd6339c9379.media_summit_idet125.jpg',
                                'Height': '82',
                                'Width': '76'
                            }
                        }, 
                        {
                            'Title': 'Yahoo Finance revises charts, chat, other features',
                            'Summary': ' Yahoo Inc. on Monday will unveil an upgraded version of its top-ranked financial information site that features new stock charting tools, improved investor chat rooms and financial video news.',
                            'Url': 'http://news.yahoo.com/s/nm/20060717/wr_nm/media_yahoo_finance_dc_2',
                            'ClickUrl': 'http://news.yahoo.com/s/nm/20060717/wr_nm/media_yahoo_finance_dc_2',
                            'NewsSource': 'Reuters via Yahoo! News',
                            'NewsSourceUrl': 'http://news.yahoo.com/',
                            'Language': 'en',
                            'PublishDate': '1153113288',
                            'ModificationDate': '1153113674'
                        }
                    ]
                }
            }";
            
            JsonTextReader reader = new JsonTextReader(new StringReader(text));
            ImportContext context = new ImportContext();
            YahooResponse response = (YahooResponse) context.Import(typeof(YahooResponse), reader);
            Assert.IsNotNull(response);
            
            YahooResultSet resultSet = response.ResultSet;
            Assert.IsNotNull(resultSet);
            Assert.AreEqual(2393,  resultSet.totalResultsAvailable);
            Assert.AreEqual(3,  resultSet.totalResultsReturned);
            Assert.AreEqual(1,  resultSet.firstResultPosition);
            Assert.AreEqual(3,  resultSet.Result.Length);
            
            YahooResult result = resultSet.Result[0];
            
            Assert.IsNotNull(result);
            Assert.AreEqual("Yahoo invites its users to shoot ads", result.Title);
            Assert.AreEqual(" Yahoo first encouraged consumers to create blogs and photo pages with text and pictures. Now, the Internet portal wants them to make advertisements, too. On Monday, Yahoo touts a new look for its front page by asking people to pull out the video camera, open up the editing software and create 12-second spot for Yahoo.", result.Summary);
            Assert.AreEqual("http://news.yahoo.com/s/usatoday/20060717/tc_usatoday/yahooinvitesitsuserstoshootads", result.Url);
            Assert.AreEqual("http://news.yahoo.com/s/usatoday/20060717/tc_usatoday/yahooinvitesitsuserstoshootads", result.ClickUrl);
            Assert.AreEqual("USATODAY.com via Yahoo! News", result.NewsSource);
            Assert.AreEqual("http://news.yahoo.com/", result.NewsSourceUrl);
            Assert.AreEqual("en", result.Language);
            Assert.AreEqual(1153133816, result.PublishDate);
            Assert.AreEqual(1153134044, result.ModificationDate);
            
            result = resultSet.Result[1];

            Assert.AreEqual("Yahoo to launch new finance features", result.Title);
            Assert.AreEqual(" Yahoo Inc. is beefing up the finance section of its Web site with more interactive stock charts and other features to help it maintain its longtime lead over rival financial information sites.", result.Summary);
            Assert.AreEqual("http://news.yahoo.com/s/ap/20060717/ap_on_hi_te/yahoo_finance_2", result.Url);
            Assert.AreEqual("http://news.yahoo.com/s/ap/20060717/ap_on_hi_te/yahoo_finance_2", result.ClickUrl);
            Assert.AreEqual("AP via Yahoo! News", result.NewsSource);
            Assert.AreEqual("http://news.yahoo.com/", result.NewsSourceUrl);
            Assert.AreEqual("en", result.Language);
            Assert.AreEqual(1153134777, result.PublishDate);
            Assert.AreEqual(1153134920, result.ModificationDate);
            Assert.AreEqual("http://us.news2.yimg.com/us.yimg.com/p/ap/20060714/vsthumb.8b1161b66b564adba0a5bbd6339c9379.media_summit_idet125.jpg", result.Thumbnail.Url);
            Assert.AreEqual(82, result.Thumbnail.Height);
            Assert.AreEqual(76, result.Thumbnail.Width);

            result = resultSet.Result[2];

            Assert.AreEqual("Yahoo Finance revises charts, chat, other features", result.Title);
            Assert.AreEqual(" Yahoo Inc. on Monday will unveil an upgraded version of its top-ranked financial information site that features new stock charting tools, improved investor chat rooms and financial video news.", result.Summary);
            Assert.AreEqual("http://news.yahoo.com/s/nm/20060717/wr_nm/media_yahoo_finance_dc_2", result.Url);
            Assert.AreEqual("http://news.yahoo.com/s/nm/20060717/wr_nm/media_yahoo_finance_dc_2", result.ClickUrl);
            Assert.AreEqual("Reuters via Yahoo! News", result.NewsSource);
            Assert.AreEqual("http://news.yahoo.com/", result.NewsSourceUrl);
            Assert.AreEqual("en", result.Language);
            Assert.AreEqual(1153113288, result.PublishDate);
            Assert.AreEqual(1153113674, result.ModificationDate);
        }
        
        
        [ Test ]
        public void SkipsReadOnlyProperty()
        {
            Type thingType = typeof(Thing);
            JsonTextReader reader = new JsonTextReader(new StringReader("{ Id : 42 }"));
            TestTypeDescriptor descriptor = new TestTypeDescriptor();
            descriptor.AddReadOnlyProperty("Id");
            ComponentImporter importer = new ComponentImporter(thingType, descriptor);
            importer.Import(new ImportContext(), reader);
            Assert.IsFalse(descriptor.GetProperty("Id").SetValueCalled);
        }

        private sealed class Thing
        {
        }

        private sealed class TestTypeDescriptor : ICustomTypeDescriptor
        {
            private static readonly PropertyDescriptorCollection _properties = new PropertyDescriptorCollection(null);

            public PropertyDescriptorCollection GetProperties()
            {
                return _properties;
            }
            
            public void AddReadOnlyProperty(string name)
            {
                _properties.Add(new ReadOnlyPropertyDescriptor(name));
            }

            public sealed class ReadOnlyPropertyDescriptor : PropertyDescriptor
            {
                public bool SetValueCalled;

                public ReadOnlyPropertyDescriptor(string name) : 
                    base(name, null) {}

                public override void SetValue(object component, object value)
                {
                    SetValueCalled = true;
                }

                public override bool IsReadOnly
                {
                    get { return true; }
                }

                #region Unused members

                public override bool CanResetValue(object component)
                {
                    throw new NotImplementedException();
                }

                public override object GetValue(object component)
                {
                    throw new NotImplementedException();
                }

                public override void ResetValue(object component)
                {
                    throw new NotImplementedException();
                }

                public override bool ShouldSerializeValue(object component)
                {
                    throw new NotImplementedException();
                }

                public override Type ComponentType
                {
                    get { throw new NotImplementedException(); }
                }

                public override Type PropertyType
                {
                    get { throw new NotImplementedException(); }
                }

                #endregion
            }

            #region Unused members

            public AttributeCollection GetAttributes()
            {
                throw new NotImplementedException();
            }

            public string GetClassName()
            {
                throw new NotImplementedException();
            }

            public string GetComponentName()
            {
                throw new NotImplementedException();
            }

            public TypeConverter GetConverter()
            {
                throw new NotImplementedException();
            }

            public EventDescriptor GetDefaultEvent()
            {
                throw new NotImplementedException();
            }

            public PropertyDescriptor GetDefaultProperty()
            {
                throw new NotImplementedException();
            }

            public object GetEditor(Type editorBaseType)
            {
                throw new NotImplementedException();
            }

            public EventDescriptorCollection GetEvents()
            {
                throw new NotImplementedException();
            }

            public EventDescriptorCollection GetEvents(Attribute[] attributes)
            {
                throw new NotImplementedException();
            }

            public PropertyDescriptorCollection GetProperties(Attribute[] attributes)
            {
                throw new NotImplementedException();
            }

            public object GetPropertyOwner(PropertyDescriptor pd)
            {
                throw new NotImplementedException();
            }

            #endregion

            public ReadOnlyPropertyDescriptor GetProperty(string name)
            {
                return (ReadOnlyPropertyDescriptor) _properties[name];
            }
        }

        private static object Import(Type expectedType, string s)
        {
            JsonReader reader = CreateReader(s);
            ImportContext context = new ImportContext();
            object o = context.Import(expectedType, reader);
            Assert.IsTrue(reader.EOF, "Reader must be at EOF.");
            Assert.IsNotNull(o);
            Assert.IsInstanceOfType(expectedType, o);
            return o;
        }

        private static JsonReader CreateReader(string s)
        {
            return new JsonTextReader(new StringReader(s));
        }
        
        public sealed class Marriage
        {
            private Person _husband;
            private Person _wife;

            public Person Husband
            {
                get { return _husband; }
                set { _husband = value; }
            }

            public Person Wife
            {
                get { return _wife; }
                set { _wife = value; }
            }
        }

        public sealed class Person
        {
            private int _id;
            private string _fullName;

            public int Id
            {
                get { return _id; }
                set { _id = value; }
            }

            public string FullName
            {
                get { return _fullName; }
                set { _fullName = value; }
            }
        }

        //
        // NOTE: The default assignments on the following fields is
        // to maily shut off the following warning from the compiler:
        //
        // warning CS0649: Field '...' is never assigned to, and will always have its default value
        //
        // This warning is harmless. Since the C# 1.x compiler does
        // not support #pragma warning disable, we have to resort
        // to a more brute force method.
        //

        public class YahooResponse
        {
            public YahooResultSet ResultSet = null;
        }

        public class YahooResultSet
        {
            public int totalResultsAvailable = 0;
            public int totalResultsReturned = 0;
            public int firstResultPosition = 0;
            public YahooResult[] Result = null;
        }

        public class YahooResult
        {
            public string Title = null;
            public string Summary = null;
            public string Url = null;
            public string ClickUrl = null;
            public string NewsSource = null;
            public string NewsSourceUrl = null;
            public string Language = null;
            public long PublishDate = 0;
            public long ModificationDate = 0;
            public int Height = 0;
            public int Width = 0;
            public YahooThumbnail Thumbnail = null;
        }

        public class YahooThumbnail
        {
            public string Url = null;
            public int Height = 0;
            public int Width = 0;
        }
    }
}