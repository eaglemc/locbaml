//---------------------------------------------------------------------------
//
// Copyright (c) Microsoft Corporation.  All rights reserved.
// 
// Description: TranslationDictionariesWriter & TranslationDictionariesReader class
//
//---------------------------------------------------------------------------

using System;
using System.IO;
using System.Resources;
using System.Collections;
using System.Globalization;
using System.Windows;
using System.Diagnostics;
using System.ComponentModel;
using System.Windows.Markup.Localizer;

namespace BamlLocalization
{
    /// <summary>
    /// Writer to write out localizable values into CSV or tab-separated txt files.     
    /// </summary>
    internal static class TranslationDictionariesWriter
    {
        /// <summary>
        /// Write the localizable key-value pairs
        /// </summary>
        /// <param name="options"></param>
        internal static void Write(LocBamlOptions options)            
        {   
            InputBamlStreamList bamlStreamList = new InputBamlStreamList(options);

            using (ITranslationWriter writer = options.GetTranslationWriter())
            {
                options.WriteLine(StringLoader.Get("WriteBamlValues"));
                for (int i = 0; i < bamlStreamList.Count; i++)
                {
                    options.Write("    ");
                    options.Write(StringLoader.Get("ProcessingBaml", bamlStreamList[i].Name));

                    // Search for comment file in the same directory. The comment file has the extension to be 
                    // "loc".
                    string commentFile = Path.ChangeExtension(bamlStreamList[i].Name, "loc");
                    TextReader commentStream = null;

                    try
                    {
                        if (File.Exists(commentFile))
                        {
                            commentStream = new StreamReader(commentFile);
                        }

                        // create the baml localizer
                        BamlLocalizer mgr = new BamlLocalizer(
                            bamlStreamList[i].Stream,
                            new BamlLocalizabilityByReflection(options.Assemblies),
                            commentStream
                            );

                        // extract localizable resource from the baml stream
                        BamlLocalizationDictionary dict = mgr.ExtractResources();

                        // write out each resource
                        foreach (DictionaryEntry entry in dict)
                        {
                            BamlLocalizableResourceKey key = (BamlLocalizableResourceKey)entry.Key;
                            BamlLocalizableResource resource = (BamlLocalizableResource)entry.Value;

                            writer.WriteResource(bamlStreamList[i].Name, LocBamlConst.ResourceKeyToString(key), resource);
                        }

                        options.WriteLine(StringLoader.Get("Done"));
                    }
                    finally
                    {
                        if (commentStream != null)
                            commentStream.Close();
                    }
                }
                
                // close all the baml input streams, output stream is closed by writer.
                bamlStreamList.Close();            
            }   
        }
    }
}
