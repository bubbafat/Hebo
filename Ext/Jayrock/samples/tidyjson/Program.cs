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
    using System.Diagnostics;
    using System.IO;
    using System.Reflection;
    using Jayrock.Json;

    #endregion

    internal static class Program
    {
        private static int Main(string[] args)
        {
            try
            {
                ConsoleBrush defaultBrush = ConsoleBrush.Current;

                try
                {
                    ProgramOptions options = new ProgramOptions();
                    options.Help += delegate { Help(); Environment.Exit(0); };
                    options.Palette = JsonPalette.Auto(defaultBrush);
                    args = options.Parse(args);
                    
                    string path = args.Length > 0 ? args[0] : "-";
                    PrettyColorPrint(path, Console.Out, options.Palette);
                    
                    return 0;
                }
                finally
                {
                    defaultBrush.Apply();
                }
            }
            catch (Exception e)
            {
                Console.Error.WriteLine(e.GetBaseException().Message);
                Trace.WriteLine(e.ToString());
                return -1;
            }
        }

        private static void PrettyColorPrint(string path, TextWriter output, JsonPalette palette) 
        {
            Debug.Assert(output != null);

            using (TextReader input = path.Equals("-") ? Console.In : new StreamReader(path))
            using (JsonTextReader reader = new JsonTextReader(input))
            using (JsonTextWriter writer = new JsonTextWriter(output))
            {
                writer.PrettyPrint = true;
                JsonColorWriter colorWriter = new JsonColorWriter(writer, palette);
                colorWriter.WriteFromReader(reader);
                output.WriteLine();
            }
        }

        #region Help (Logo, Usage and Disclaimer)

        private static void Help()
        {
            WriteLogo();
            ProgramOptions.ShowUsage();
            Console.WriteLine(@"

THE SOFTWARE IS PROVIDED ""AS IS"", WITHOUT WARRANTY OF ANY KIND, EXPRESS 
OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF 
MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. 
IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY 
CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, 
TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE 
SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.");
        }

        private static void WriteLogo()
        {
            Assembly assembly = typeof(Program).Assembly;

            Console.WriteLine("{0}, v{1}",
                AttributeQuery<AssemblyTitleAttribute>.Get(assembly).Title,
                assembly.GetName().Version);
            Console.WriteLine(AttributeQuery<AssemblyDescriptionAttribute>.Get(assembly).Description);
            Console.WriteLine("Written by Atif Aziz -- http://www.raboof.com/");
            Console.WriteLine(AttributeQuery<AssemblyCopyrightAttribute>.Get(assembly).Copyright);
            Console.WriteLine();
        }

        #endregion
    }
}
