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

namespace JellyDemo
{
    #region Imports

    using System;
    using System.CodeDom;
    using System.CodeDom.Compiler;
    using System.Collections;
    using System.Diagnostics;
    using System.IO;
    using System.Net;
    using System.Xml;
    using Jayrock.Json;
    using Jelly;

    #endregion

    internal sealed class Program
    {
        [ STAThread ]
        static void Main()
        {
            //
            // Demonstration of client calls to a JSON-RPC service.
            //
            
            JsonRpcClient client = new JsonRpcClient();
            client.Url = "http://www.raboof.com/projects/jayrock/demo.ashx";
            Console.WriteLine(client.Invoke("system.about"));
            Console.WriteLine(client.Invoke("system.version"));
            Console.WriteLine(string.Join(Environment.NewLine, (string[]) (new ArrayList((ICollection) client.Invoke("system.listMethods"))).ToArray(typeof(string))));
            Console.WriteLine(client.Invoke("now"));
            Console.WriteLine(client.Invoke("sum", 123, 456));
            Console.WriteLine(client.Invoke("total", new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9 }));
            client.CookieContainer = new CookieContainer();
            Console.WriteLine(client.Invoke("counter"));
            Console.WriteLine(client.Invoke("counter"));

            //
            // Demonstration of a JsonWriter implementation (JsonCodeWriter) 
            // that writes code in terms of itself. So we take some JSON
            // text and covert it into the JsonWriter API.
            //

            CodeEntryPointMethod codeMainMethod = new CodeEntryPointMethod();
            codeMainMethod.Statements.Add(new CodeVariableDeclarationStatement(typeof(JsonWriter), "writer", new CodeObjectCreateExpression(typeof(EmptyJsonWriter))));

            JsonCodeWriter writer = new JsonCodeWriter(new CodeVariableReferenceExpression("writer"), codeMainMethod.Statements);
            writer.WriteFromReader(new JsonTextReader(new StringReader(@"
                { 
                    firstName : 'John', 
                    lastName : 'Doe' 
                }")));
            
            CodeTypeDeclaration codeType = new CodeTypeDeclaration("Program");
            codeType.Members.Add(codeMainMethod);
            
            CodeGeneratorOptions options = new CodeGeneratorOptions();
            GetLanguageProvider("c#").CreateGenerator().GenerateCodeFromType(codeType, Console.Out, options);
            GetLanguageProvider("vb").CreateGenerator().GenerateCodeFromType(codeType, Console.Out, options);
        }

        private static CodeDomProvider GetLanguageProvider(string language) 
        {
            Debug.Assert(language != null);
            
            XmlDocument config = new XmlDocument();

            config.LoadXml(@"
                <compilers>
                    <compiler language='c#;cs;csharp' extension='.cs' type='Microsoft.CSharp.CSharpCodeProvider, System, Version=1.0.5000.0, Culture=neutral, PublicKeyToken=b77a5c561934e089' warningLevel='1' />
                    <compiler language='vb;vbs;visualbasic;vbscript' extension='.vb' type='Microsoft.VisualBasic.VBCodeProvider, System, Version=1.0.5000.0, Culture=neutral, PublicKeyToken=b77a5c561934e089' />
                    <compiler language='js;jscript;javascript' extension='.js' type='Microsoft.JScript.JScriptCodeProvider, Microsoft.JScript, Version=7.0.5000.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a' />
                    <compiler language='VJ#;VJS;VJSharp' extension='.jsl' type='Microsoft.VJSharp.VJSharpCodeProvider, VJSharpCodeProvider, Version=7.0.5000.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a' />
                </compilers>");

            XmlElement compilerNode = (XmlElement) config.SelectSingleNode("/compilers/compiler[contains(concat(';', concat(@language, ';')), ';" + language + ";')]");
            Debug.Assert(compilerNode != null, "No compiler for '" + language + "'.");
            
            Type compilerType = Type.GetType(compilerNode.GetAttribute("type"));
            return (CodeDomProvider) Activator.CreateInstance(compilerType);
        }
    }
}