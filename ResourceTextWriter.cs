//---------------------------------------------------------------------------
//
// Copyright (c) Microsoft Corporation.  All rights reserved.
// 
// Description: ResourceTextWriter class 
//              It writes values to a CSV file or tab-separated TXT file
//
//---------------------------------------------------------------------------

using System;
using System.IO;
using System.Text;
using System.Resources;
using System.Collections;
using System.Globalization;
using System.Diagnostics;
using System.Windows.Markup.Localizer;

namespace BamlLocalization
{    
    /// <summary>
    /// the class that writes to a text file either tab delimited or comma delimited. 
    /// </summary>
    internal sealed class ResourceTextWriter : ITranslationWriter, IDisposable
    {
        //-------------------------------
        // constructor 
        //-------------------------------
        internal ResourceTextWriter(TranslationFileType fileType, Stream output)
        {
            
            _delimiter = LocBamlConst.GetDelimiter(fileType);

            if (output == null)
                throw new ArgumentNullException("output");

            // show utf8 byte order marker
            UTF8Encoding encoding = new UTF8Encoding(true);           
            _writer      = new StreamWriter(output, encoding);
            _firstColumn = true;
       }

       #region internal methods
       //-----------------------------------
       // Internal methods
        //-----------------------------------
        internal void WriteColumn(string value)
        {    
            if (value == null)
                    value = string.Empty;

            // if it contains delimeter, quote, newline, we need to escape them
            if (value.IndexOfAny(new char[]{'\"', '\r', '\n', _delimiter}) >= 0)
            {
                // make a string builder at the minimum required length;
                StringBuilder builder = new StringBuilder(value.Length + 2);

                // put in the opening quote
                builder.Append('\"');
                
                // double quote each quote
                for (int i = 0; i < value.Length; i++)
                {
                    builder.Append(value[i]);
                    if (value[i] == '\"')
                    {
                        builder.Append('\"');
                    }                       
                }

                // put in the closing quote
                builder.Append('\"');
                value = builder.ToString();
            }

            if (!_firstColumn)
            {
                // if we are not the first column, we write delimeter
                // to seperate the new cell from the previous ones.
                _writer.Write(_delimiter);                
            }
            else
            {
                _firstColumn = false;   // set false
            }

            _writer.Write(value);            
        }

        internal void EndLine()
        {
            // write a new line
            _writer.WriteLine();

            // set first column to true    
            _firstColumn = true;
        }
        internal void Close()
        {
            if (_writer != null)
            {
                _writer.Close();
            }
        }
       #endregion 

        void IDisposable.Dispose()
        {
            Close();
        }

        public void WriteResource(string bamlStreamName, string resourceKey, BamlLocalizableResource resource)
        {
            // column 1: baml stream name
            WriteColumn(bamlStreamName);

            // column 2: localizable resource key
            WriteColumn(resourceKey);

            // column 3: localizable resource's category
            WriteColumn(resource.Category.ToString());

            // column 4: localizable resource's readability
            WriteColumn(resource.Readable.ToString());

            // column 5: localizable resource's modifiability
            WriteColumn(resource.Modifiable.ToString());

            // column 6: localizable resource's localization comments
            WriteColumn(resource.Comments);

            // column 7: localizable resource's content
            WriteColumn(resource.Content);

            // Done. finishing the line
            EndLine();
        }

        #region private members
        private char        _delimiter;
        private TextWriter  _writer;        
        private bool        _firstColumn;
        #endregion
    }    
}





    