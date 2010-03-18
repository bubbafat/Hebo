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
    using System.Configuration;
    using System.IO;
    using Jayrock.Json;
    using Jayrock.Json.Conversion;
    using TidyJson.Properties;

    #endregion

    [ Serializable ]
    internal sealed class JsonPalette : IJsonImportable
    {
        private ConsoleBrush defaultBrush;
        private ConsoleBrush nullBrush;
        private ConsoleBrush stringBrush;
        private ConsoleBrush numberBrush;
        private ConsoleBrush booleanBrush;
        private ConsoleBrush objectBrush;
        private ConsoleBrush memberBrush;
        private ConsoleBrush arrayBrush;

        public JsonPalette() : 
            this(ConsoleBrush.Current) {}

        public JsonPalette(ConsoleBrush defaultBrush)
        {
            this.defaultBrush = defaultBrush;
            this.nullBrush = defaultBrush;
            this.stringBrush = defaultBrush;
            this.numberBrush = defaultBrush;
            this.booleanBrush = defaultBrush;
            this.objectBrush = defaultBrush;
            this.memberBrush = defaultBrush;
            this.arrayBrush = defaultBrush;
        }

        public static JsonPalette Auto()
        {
            return Auto(ConsoleBrush.Current);
        }

        public static JsonPalette Auto(ConsoleBrush defaultBrush)
        {
            string setting;

            try
            {
                setting = (string) Settings.Default[defaultBrush.Background + "Palette"];
            }
            catch (SettingsPropertyNotFoundException)
            {
                setting = null;
            }

            if (string.IsNullOrEmpty(setting))
            {
                setting = Settings.Default.BlackPalette;
                defaultBrush = new ConsoleBrush(ConsoleColor.White, ConsoleColor.Black);
            }

            JsonPalette palette = new JsonPalette(defaultBrush);
            palette.ImportJson(setting);

            return palette;
        }

        public ConsoleBrush DefaultBrush
        {
            get { return defaultBrush; }
            set { defaultBrush = value; }
        }

        public ConsoleBrush Null
        {
            get { return nullBrush; }
            set { nullBrush = value; }
        }

        public ConsoleBrush String
        {
            get { return stringBrush; }
            set { stringBrush = value; }
        }

        public ConsoleBrush Number
        {
            get { return numberBrush; }
            set { numberBrush = value; }
        }

        public ConsoleBrush Boolean
        {
            get { return booleanBrush; }
            set { booleanBrush = value; }
        }

        public ConsoleBrush Object
        {
            get { return objectBrush; }
            set { objectBrush = value; }
        }

        public ConsoleBrush Member
        {
            get { return memberBrush; }
            set { memberBrush = value; }
        }

        public ConsoleBrush Array
        {
            get { return arrayBrush; }
            set { arrayBrush = value; }
        }

        public void ImportJson(string text)
        {
            ImportJson(new JsonTextReader(new StringReader(text)));
        }

        public void ImportJson(JsonReader reader)
        {
            ((IJsonImportable) this).Import(new ImportContext(), reader);
        }

        void IJsonImportable.Import(ImportContext context, JsonReader reader)
        {
            reader.MoveToContent();

            if (reader.TokenClass != JsonTokenClass.Object)
            {
                reader.Skip();
                return;
            }

            reader.Read(/* object */);

            do
            {
                string brushName = reader.ReadMember().ToLowerInvariant();
                string color = reader.ReadString();

                ConsoleColor foreground = EnumHelper.TryParse<ConsoleColor>(color, true) ?? DefaultBrush.Foreground;

                switch (brushName)
                {
                    case "arr":
                    case "array":
                        Array = Array.ResetForeground(foreground);
                        break;
                    case "obj":
                    case "object":
                        Object = Object.ResetForeground(foreground);
                        break;
                    case "mem":
                    case "member":
                        Member = Member.ResetForeground(foreground);
                        break;
                    case "str":
                    case "string":
                        String = String.ResetForeground(foreground);
                        break;
                    case "num":
                    case "number":
                        Number = Number.ResetForeground(foreground);
                        break;
                    case "bit":
                    case "boolean":
                        Boolean = Boolean.ResetForeground(foreground);
                        break;
                    case "nil":
                    case "null":
                        Null = Null.ResetForeground(foreground);
                        break;
                    default:
                        continue;
                }
            }
            while (reader.TokenClass != JsonTokenClass.EndObject);

            reader.Read( /* end object */);
        }
    }
}