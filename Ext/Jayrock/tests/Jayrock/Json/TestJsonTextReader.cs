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
    using System.Collections;
    using System.IO;
    using NUnit.Framework;

    #endregion

    [ TestFixture ]
    public class TestJsonTextReader
    {
        private JsonTextReader _reader;

        [ TearDown ]
        public void Dispose()
        {
            _reader = null;
        }

        [ Test, ExpectedException(typeof(JsonException)) ]
        public void Blank()
        {
            JsonTextReader reader = new JsonTextReader(new StringReader(string.Empty));
            reader.Read();
        }

        [ Test ]
        public void BOF()
        {
            JsonTextReader reader = new JsonTextReader(new StringReader(string.Empty));
            Assert.AreEqual(JsonTokenClass.BOF, reader.TokenClass);
        }

        [ Test ]
        public void String()
        {
            CreateReader("'Hello World'");
            AssertTokenText(JsonTokenClass.String, "Hello World");
            AssertEOF();
        }

        [ Test ]
        public void Number()
        {
            CreateReader("123");
            AssertTokenText(JsonTokenClass.Number, "123");
            AssertEOF();
        }

        [ Test ]
        public void Null()
        {
            CreateReader("null");
            AssertTokenText(JsonTokenClass.Null, "null");
            AssertEOF();
        }

        [ Test ]
        public void BooleanTrue()
        {
            CreateReader("true");
            AssertTokenText(JsonTokenClass.Boolean, "true");
            AssertEOF();
        }
        
        [ Test ]
        public void BooleanFalse()
        {
            CreateReader("false");
            AssertTokenText(JsonTokenClass.Boolean, "false");
            AssertEOF();
        }

        [ Test ]
        public void EmptyArray()
        {
            CreateReader("[]");
            AssertToken(JsonTokenClass.Array);
            AssertToken(JsonTokenClass.EndArray);
            AssertEOF();
        }

        [ Test ]
        public void ArrayWithOneNumber()
        {
            CreateReader("[ 123 ]");
            AssertToken(JsonTokenClass.Array);
            AssertTokenText(JsonTokenClass.Number, "123");
            AssertToken(JsonTokenClass.EndArray);
            AssertEOF();
        }

        [ Test ]
        public void ArrayWithPrimitives()
        {
            CreateReader("[ 123, 'string', true, false, null ]");
            AssertToken(JsonTokenClass.Array);
            AssertTokenText(JsonTokenClass.Number, "123");
            AssertTokenText(JsonTokenClass.String, "string");
            AssertTokenText(JsonTokenClass.Boolean, "true");
            AssertTokenText(JsonTokenClass.Boolean, "false");
            AssertToken(JsonTokenClass.Null);
            AssertToken(JsonTokenClass.EndArray);
            AssertEOF();
        }

        [ Test ]
        public void EmptyObject()
        {
            CreateReader("{}");
            AssertToken(JsonTokenClass.Object);
            AssertToken(JsonTokenClass.EndObject);
            AssertEOF();
        }

        [ Test ]
        public void ObjectWithOneMember()
        {
            CreateReader("{ 'num' : 123 }");
            AssertToken(JsonTokenClass.Object);
            AssertTokenText(JsonTokenClass.Member, "num");
            AssertTokenText(JsonTokenClass.Number, "123");
            AssertToken(JsonTokenClass.EndObject);
            AssertEOF();
        }

        [ Test ]
        public void ObjectWithPrimitiveMembers()
        {
            CreateReader("{ m1 : 123, m2 : 'string', m3 : true, m4 : false, m5 : null }");
            AssertToken(JsonTokenClass.Object);
            AssertMember("m1", JsonTokenClass.Number, "123");
            AssertMember("m2", "string");
            AssertMember("m3", JsonTokenClass.Boolean, "true");
            AssertMember("m4", JsonTokenClass.Boolean, "false");
            AssertMember("m5", JsonTokenClass.Null);
            AssertToken(JsonTokenClass.EndObject);
            AssertEOF();
        }

        [ Test ]
        public void Complex()
        {
            CreateReader(@"
                {'menu': {
                    'header': 'SVG Viewer',
                    'items': [
                        {'id': 'Open'},
                        {'id': 'OpenNew', 'label': 'Open New'},
                        null,
                        {'id': 'ZoomIn', 'label': 'Zoom In'},
                        {'id': 'ZoomOut', 'label': 'Zoom Out'},
                        {'id': 'OriginalView', 'label': 'Original View'},
                        null,
                        {'id': 'Quality'},
                        {'id': 'Pause'},
                        {'id': 'Mute'}
                    ]
                }}");

            AssertToken(JsonTokenClass.Object);
            AssertMember("menu", JsonTokenClass.Object);
            AssertMember("header", "SVG Viewer");
            AssertMember("items", JsonTokenClass.Array);
            
            AssertToken(JsonTokenClass.Object);
            AssertMember("id", "Open");
            AssertToken(JsonTokenClass.EndObject);
            
            AssertToken(JsonTokenClass.Object);
            AssertMember("id", "OpenNew");
            AssertMember("label", "Open New");
            AssertToken(JsonTokenClass.EndObject);

            AssertToken(JsonTokenClass.Null);

            AssertToken(JsonTokenClass.Object);
            AssertMember("id", "ZoomIn");
            AssertMember("label", "Zoom In");
            AssertToken(JsonTokenClass.EndObject);

            AssertToken(JsonTokenClass.Object);
            AssertMember("id", "ZoomOut");
            AssertMember("label", "Zoom Out");
            AssertToken(JsonTokenClass.EndObject);

            AssertToken(JsonTokenClass.Object);
            AssertMember("id", "OriginalView");
            AssertMember("label", "Original View");
            AssertToken(JsonTokenClass.EndObject);

            AssertToken(JsonTokenClass.Null);
            
            AssertToken(JsonTokenClass.Object);
            AssertMember("id", "Quality");
            AssertToken(JsonTokenClass.EndObject);

            AssertToken(JsonTokenClass.Object);
            AssertMember("id", "Pause");
            AssertToken(JsonTokenClass.EndObject);

            AssertToken(JsonTokenClass.Object);
            AssertMember("id", "Mute");
            AssertToken(JsonTokenClass.EndObject);
            
            AssertToken(JsonTokenClass.EndArray);
            AssertToken(JsonTokenClass.EndObject);
            AssertToken(JsonTokenClass.EndObject);
            AssertEOF();
        }

        [ Test ]
        public void OneLevelDepth()
        {
         
            CreateReader("[]");
            Assert.AreEqual(0, _reader.Depth);
            AssertToken(JsonTokenClass.Array);
            Assert.AreEqual(1, _reader.Depth);
            AssertToken(JsonTokenClass.EndArray);
            Assert.AreEqual(1, _reader.Depth);
            AssertEOF();
            Assert.AreEqual(0, _reader.Depth);
        }

        [ Test ]
        public void TwoLevelDepth()
        {         
            CreateReader("[{}]");
            Assert.AreEqual(0, _reader.Depth);

            AssertToken(JsonTokenClass.Array);
            Assert.AreEqual(1, _reader.Depth);

            AssertToken(JsonTokenClass.Object);
            Assert.AreEqual(2, _reader.Depth);
            AssertToken(JsonTokenClass.EndObject);
            Assert.AreEqual(2, _reader.Depth);
            
            AssertToken(JsonTokenClass.EndArray);
            Assert.AreEqual(1, _reader.Depth);

            AssertEOF();
            Assert.AreEqual(0, _reader.Depth);
        }
        
        [ Test ]
        public void NestedDepths()
        {
            CreateReader("[{a:[{}]}]");

            const int maxDepth = 4;
            for (int i = 0; i < maxDepth; i++)
            {
                Assert.AreEqual(i, _reader.Depth);
                
                if (i % 2 == 0)
                {
                    AssertToken(JsonTokenClass.Array);
                }
                else
                {
                    AssertToken(JsonTokenClass.Object);
                    if (i < (maxDepth - 1))
                        AssertToken(JsonTokenClass.Member);
                }

                Assert.AreEqual(i + 1, _reader.Depth);
            }

            for (int i = 0; i < maxDepth; i++)
            {
                AssertToken(i % 2 == 0 ? JsonTokenClass.EndObject : JsonTokenClass.EndArray);
                Assert.AreEqual(maxDepth - i, _reader.Depth);
            }

            AssertEOF();
            Assert.AreEqual(0, _reader.Depth);
        }

        [ Test, ExpectedException(typeof(JsonException)) ]
        public void UnclosedComment()
        {
            Read(@"/* This is an unclosed comment");
        }

        [ Test, ExpectedException(typeof(JsonException)) ]
        public void UnterminatedString()
        {
            Read("\"Hello World'");
        }

        [ Test, ExpectedException(typeof(JsonException)) ]
        public void EmptyInput()
        {
            Read(string.Empty);
        }

        [ Test, ExpectedException(typeof(JsonException)) ]
        public void BadNumber()
        {
            Read("1234.S6");
        }

        [ Test ]
        public void UnquotedStringsMayWork()
        {
            CreateReader("hello");
            Assert.AreEqual("hello", _reader.ReadString());
        }

        [ Test ]
        public void Shred()
        {
            JsonReader reader = CreateReader(@"
                {'web-app': {
                'servlet': [   
                    {
                    'servlet-name': 'cofaxCDS',
                    'servlet-class': 'org.cofax.cds.CDSServlet',
                    'init-param': {
                        'configGlossary:installationAt': 'Philadelphia, PA',
                        'configGlossary:adminEmail': 'ksm@pobox.com',
                        'configGlossary:poweredBy': 'Cofax',
                        'configGlossary:poweredByIcon': '/images/cofax.gif',
                        'configGlossary:staticPath': '/content/static',
                        'templateProcessorClass': 'org.cofax.WysiwygTemplate',
                        'templateLoaderClass': 'org.cofax.FilesTemplateLoader',
                        'templatePath': 'templates',
                        'templateOverridePath': '',
                        'defaultListTemplate': 'listTemplate.htm',
                        'defaultFileTemplate': 'articleTemplate.htm',
                        'useJSP': false,
                        'jspListTemplate': 'listTemplate.jsp',
                        'jspFileTemplate': 'articleTemplate.jsp',
                        'cachePackageTagsTrack': 200,
                        'cachePackageTagsStore': 200,
                        'cachePackageTagsRefresh': 60,
                        'cacheTemplatesTrack': 100,
                        'cacheTemplatesStore': 50,
                        'cacheTemplatesRefresh': 15,
                        'cachePagesTrack': 200,
                        'cachePagesStore': 100,
                        'cachePagesRefresh': 10,
                        'cachePagesDirtyRead': 10,
                        'searchEngineListTemplate': 'forSearchEnginesList.htm',
                        'searchEngineFileTemplate': 'forSearchEngines.htm',
                        'searchEngineRobotsDb': 'WEB-INF/robots.db',
                        'useDataStore': true,
                        'dataStoreClass': 'org.cofax.SqlDataStore',
                        'redirectionClass': 'org.cofax.SqlRedirection',
                        'dataStoreName': 'cofax',
                        'dataStoreDriver': 'com.microsoft.jdbc.sqlserver.SQLServerDriver',
                        'dataStoreUrl': 'jdbc:microsoft:sqlserver://LOCALHOST:1433;DatabaseName=goon',
                        'dataStoreUser': 'sa',
                        'dataStorePassword': 'dataStoreTestQuery',
                        'dataStoreTestQuery': 'SET NOCOUNT ON;select test=\'test\';',
                        'dataStoreLogFile': '/usr/local/tomcat/logs/datastore.log',
                        'dataStoreInitConns': 10,
                        'dataStoreMaxConns': 100,
                        'dataStoreConnUsageLimit': 100,
                        'dataStoreLogLevel': 'debug',
                        'maxUrlLength': 500}},
                    {
                    'servlet-name': 'cofaxEmail',
                    'servlet-class': 'org.cofax.cds.EmailServlet',
                    'init-param': {
                    'mailHost': 'mail1',
                    'mailHostOverride': 'mail2'}},
                    {
                    'servlet-name': 'cofaxAdmin',
                    'servlet-class': 'org.cofax.cds.AdminServlet'},
                 
                    {
                    'servlet-name': 'fileServlet',
                    'servlet-class': 'org.cofax.cds.FileServlet'},
                    {
                    'servlet-name': 'cofaxTools',
                    'servlet-class': 'org.cofax.cms.CofaxToolsServlet',
                    'init-param': {
                        'templatePath': 'toolstemplates/',
                        'log': 1,
                        'logLocation': '/usr/local/tomcat/logs/CofaxTools.log',
                        'logMaxSize': '',
                        'dataLog': 1,
                        'dataLogLocation': '/usr/local/tomcat/logs/dataLog.log',
                        'dataLogMaxSize': '',
                        'removePageCache': '/content/admin/remove?cache=pages&id=',
                        'removeTemplateCache': '/content/admin/remove?cache=templates&id=',
                        'fileTransferFolder': '/usr/local/tomcat/webapps/content/fileTransferFolder',
                        'lookInContext': 1,
                        'adminGroupID': 4,
                        'betaServer': true}}],
                'servlet-mapping': {
                    'cofaxCDS': '/',
                    'cofaxEmail': '/cofaxutil/aemail/*',
                    'cofaxAdmin': '/admin/*',
                    'fileServlet': '/static/*',
                    'cofaxTools': '/tools/*'},
                 
                'taglib': {
                    'taglib-uri': 'cofax.tld',
                    'taglib-location': '/WEB-INF/tlds/cofax.tld'}}}");

            ArrayList items = new ArrayList();

            while (reader.Read())
            {
                if (reader.TokenClass == JsonTokenClass.Member && reader.Text == "servlet-name")
                {
                    reader.Read();
                    items.Add(reader.ReadString());
                }
            }

            Assert.AreEqual(new string[] { "cofaxCDS", "cofaxEmail", "cofaxAdmin", "fileServlet", "cofaxTools" }, items.ToArray(typeof(string)));
        }

        private void AssertToken(JsonTokenClass token)
        {
            AssertTokenText(token, null);
        }

        [ Test, ExpectedException(typeof(JsonException)) ]
        public void UnterminatedObject()
        {
            JsonReader reader = CreateReader("{x:1,y:2");
            reader.MoveToContent();
            reader.StepOut();
        }

        [ Test, ExpectedException(typeof(JsonException)) ]
        public void UnterminatedArray()
        {
            JsonReader reader = CreateReader("[1,2");
            reader.MoveToContent();
            reader.StepOut();
        }
        
        [ Test, ExpectedException(typeof(JsonException)) ]
        public void MissingObjectMember()
        {
            JsonReader reader = CreateReader("{x:1,/*y:2*/,z:3}");
            reader.MoveToContent();
            reader.StepOut();
        }

        [ Test, ExpectedException(typeof(JsonException)) ]
        public void MissingObjectMemberNameValueDelimiter()
        {
            JsonReader reader = CreateReader("{x 1}");
            reader.MoveToContent();
            reader.StepOut();
        }

        [ Test ]
        public void NullMemberNameHarmless()
        {
            JsonReader reader = CreateReader("{null:null}");
            reader.MoveToContent();
            reader.ReadToken(JsonTokenClass.Object);
            Assert.AreEqual("null", reader.ReadMember());
            reader.ReadNull();
            Assert.AreSame(JsonTokenClass.EndObject, reader.TokenClass);
            Assert.IsFalse(reader.Read());
        }

        [ Test ]
        public void ExtraCommaAfterLastObjectMemberAllowded()
        {
            JsonReader reader = CreateReader("{ 'member':'value',}");
            reader.ReadToken(JsonTokenClass.Object);
            Assert.AreEqual("member", reader.ReadMember());
            Assert.AreEqual("value", reader.ReadString());
            Assert.AreSame(JsonTokenClass.EndObject, reader.TokenClass);
        }

        [ Test ]
        public void ExtraCommaAfterLastArrayElementAllowed()
        {
            JsonReader reader = CreateReader("[4,2,]");
            reader.ReadToken(JsonTokenClass.Array);
            Assert.AreEqual(4, reader.ReadNumber().ToInt32());
            Assert.AreEqual(2, reader.ReadNumber().ToInt32());
            reader.ReadToken(JsonTokenClass.EndArray);
        }

        [ Test ]
        public void AlternateKeyDelimiters()
        {
            JsonReader reader = CreateReader("{ 'm1' = 'v1', 'm2' => 'v2' }");
            reader.ReadToken(JsonTokenClass.Object);
            Assert.AreEqual("m1", reader.ReadMember());
            Assert.AreEqual("v1", reader.ReadString());
            Assert.AreEqual("m2", reader.ReadMember());
            Assert.AreEqual("v2", reader.ReadString());
            Assert.AreSame(JsonTokenClass.EndObject, reader.TokenClass);
        }

        [ Test ]
        public void MemberValuesMayBeDelimitedBySemiColon()
        {
            JsonReader reader = CreateReader("{ 'm1' = 'v1'; 'm2' => 'v2' }");
            reader.ReadToken(JsonTokenClass.Object);
            Assert.AreEqual("m1", reader.ReadMember());
            Assert.AreEqual("v1", reader.ReadString());
            Assert.AreEqual("m2", reader.ReadMember());
            Assert.AreEqual("v2", reader.ReadString());
            Assert.AreSame(JsonTokenClass.EndObject, reader.TokenClass);
        }

        [ Test ]
        public void ArrayValuesMayBeDelimitedBySemiColon()
        {
            JsonReader reader = CreateReader("[1;2;3]");
            reader.ReadToken(JsonTokenClass.Array);
            Assert.AreEqual(1, reader.ReadNumber().ToInt32());
            Assert.AreEqual(2, reader.ReadNumber().ToInt32());
            Assert.AreEqual(3, reader.ReadNumber().ToInt32());
            reader.ReadToken(JsonTokenClass.EndArray);
        }

        [ Test ]
        public void SlashSlashComment()
        {
            JsonReader reader = CreateReader(@"
            [
                1,
                // 2, this is a single line comment
                3
            ]");
            reader.ReadToken(JsonTokenClass.Array);
            Assert.AreEqual(1, reader.ReadNumber().ToInt32());
            Assert.AreEqual(3, reader.ReadNumber().ToInt32());
            reader.ReadToken(JsonTokenClass.EndArray);
        }

        [ Test ]
        public void SlashStarComment()
        {
            JsonReader reader = CreateReader(@"
            [ /*
                1,
                // 2, this is a single line comment
                3 */
            ]");
            reader.ReadToken(JsonTokenClass.Array);
            reader.ReadToken(JsonTokenClass.EndArray);
        }

        [ Test ]
        public void HashComment()
        {
            JsonReader reader = CreateReader(@"
            [
                1, # one
                2, # two
                3  # three
            ]");
            reader.ReadToken(JsonTokenClass.Array);
            Assert.AreEqual(1, reader.ReadNumber().ToInt32());
            Assert.AreEqual(2, reader.ReadNumber().ToInt32());
            Assert.AreEqual(3, reader.ReadNumber().ToInt32());
            reader.ReadToken(JsonTokenClass.EndArray);
        }

        [ Test ]
        public void StringWithHexEncodedChars()
        {
            Assert.AreEqual("1234\x1234\x56781234\xffff", CreateReader(@"'1234\u1234\u56781234\uffff'").ReadString());
        }

        [ Test ]
        public void StringWithEscapedBackspace()
        {
            Assert.AreEqual("\b", CreateReader("'\\b'").ReadString());
        }

        [ Test ]
        public void StringWithEscapedTab()
        {
            Assert.AreEqual("\t", CreateReader("'\\t'").ReadString());
        }

        [ Test ]
        public void StringWithEscapedLineFeed()
        {
            Assert.AreEqual("\n", CreateReader("'\\n'").ReadString());
        }
        
        [ Test ]
        public void StringWithEscapedFormFeed()
        {
            Assert.AreEqual("\f", CreateReader("'\\f'").ReadString());
        }

        [ Test ]
        public void StringWithEscapedCarriageReturn()
        {
            Assert.AreEqual("\r", CreateReader("'\\r'").ReadString());
        }
        
        [ Test, ExpectedException(typeof(JsonException)) ]
        public void StringWithIncompleteHexEscaping()
        {
            Read(@"'\u1'");
        }
        
        private void AssertTokenText(JsonTokenClass token, string text)
        {
            Assert.IsTrue(_reader.Read());
            Assert.AreEqual(token, _reader.TokenClass, "Found {0} (with text \x201c{1}\x201d) when expecting {2} (with text \x201c{3}\x201d).", _reader.TokenClass, _reader.Text, token, text);
            if (text != null)
                Assert.AreEqual(text, _reader.Text);
        }

        private void AssertMember(string name, JsonTokenClass valueToken)
        {
            AssertMember(name, valueToken, null);
        }

        private void AssertMember(string name, string value)
        {
            AssertMember(name, JsonTokenClass.String, value);
        }
        
        private void AssertMember(string name, JsonTokenClass valueToken, string valueText)
        {
            AssertTokenText(JsonTokenClass.Member, name);
            AssertTokenText(valueToken, valueText);
        }

        private void AssertEOF()
        {
            Assert.IsFalse(_reader.Read(), "Expected EOF.");
            Assert.AreEqual(JsonTokenClass.EOF, _reader.TokenClass);
        }

        private void Read(string s)
        {
            CreateReader(s).Read();
        }

        private JsonReader CreateReader(string s)
        {
            _reader = new JsonTextReader(new StringReader(s));
            return _reader;
        }
    }
}
