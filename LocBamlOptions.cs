using System;
using System.Collections;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Security;

namespace BamlLocalization
{
    internal sealed class LocBamlOptions
    {
        internal string Input;
        internal string Output;
        internal CultureInfo CultureInfo;
        internal string Translations;
        internal bool ToParse;
        internal bool ToGenerate;
        internal bool HasNoLogo;
        internal bool IsVerbose;
        internal TranslationFileType TranslationFileType;
        internal BinaryFileType InputType;
        internal ArrayList AssemblyPaths;
        internal Assembly[] Assemblies;

        /// <summary>
        /// return true if the operation succeeded.
        /// otherwise, return false
        /// </summary>
        internal string CheckAndSetDefault()
        {
            // we validate the options here and also set default
            // if we can

            // Rule #1: One and only one action at a time
            // i.e. Can't parse and generate at the same time
            //      Must do one of them
            if ((ToParse && ToGenerate) ||
                (!ToParse && !ToGenerate))
                return StringLoader.Get("MustChooseOneAction");

            // Rule #2: Must have an input 
            if (string.IsNullOrEmpty(Input))
            {
                return StringLoader.Get("InputFileRequired");
            }
            else
            {
                if (!File.Exists(Input))
                {
                    return StringLoader.Get("FileNotFound", Input);
                }

                string extension = Path.GetExtension(Input);
                InputType = BinaryFileTypeHelper.GetBinaryFileTypeFromExtension(extension);
                if (InputType == BinaryFileType.NONE)
                {
                    return StringLoader.Get("FileTypeNotSupported", extension);
                }
            }

            if (ToGenerate)
            {
                // Rule #3: before generation, we must have Culture string
                if (CultureInfo == null && InputType != BinaryFileType.BAML)
                {
                    // if we are not generating baml, 
                    return StringLoader.Get("CultureNameNeeded", InputType.ToString());
                }

                // Rule #4: before generation, we must have translation file
                if (string.IsNullOrEmpty(Translations))
                {

                    return StringLoader.Get("TranslationNeeded");
                }
                else
                {
                    string extension = Path.GetExtension(Translations);

                    if (!File.Exists(Translations))
                    {
                        return StringLoader.Get("TranslationNotFound", Translations);
                    }
                    else
                    {
                        TranslationFileType = TranslationFileTypeHelpers.GetTranslationFileTypeFromExtension(extension);
                    }
                }
            }

            // Rule #5: If the output file name is empty, we act accordingly
            if (string.IsNullOrEmpty(Output))
            {
                // Rule #5.1: If it is parse, we default to [input file name].csv
                if (ToParse)
                {
                    string fileName = Path.GetFileNameWithoutExtension(Input);
                    Output = fileName + TranslationFileType.CSV.GetExtension();
                    TranslationFileType = TranslationFileType.CSV;
                }
                else
                {
                    // Rule #5.2: If it is generating, and the output can't be empty
                    return StringLoader.Get("OutputDirectoryNeeded");
                }

            }
            else
            {
                // output isn't null, we will determind the Output file type                
                // Rule #6: if it is parsing. It will be .csv or .txt.
                if (ToParse)
                {
                    string fileName;
                    string outputDir;

                    if (Directory.Exists(Output))
                    {
                        // the output is actually a directory name
                        fileName = string.Empty;
                        outputDir = Output;
                    }
                    else
                    {
                        // get the extension
                        fileName = Path.GetFileName(Output);
                        outputDir = Path.GetDirectoryName(Output);
                    }

                    // Rule #6.1: if it is just the output directory
                    // we append the input file name as the output + csv as default
                    if (string.IsNullOrEmpty(fileName))
                    {
                        TranslationFileType = TranslationFileType.CSV;
                        Output = outputDir
                               + Path.DirectorySeparatorChar
                               + Path.GetFileName(Input)
                               + TranslationFileType.GetExtension();
                    }
                    else
                    {
                        // Rule #6.2: if we have file name, check the extension.
                        string extension = Path.GetExtension(Output);
                        TranslationFileType = TranslationFileTypeHelpers.GetTranslationFileTypeFromExtension(extension);
                    }
                }
                else
                {
                    // it is to generate. And Output should point to the directory name.                    
                    if (!Directory.Exists(Output))
                        return StringLoader.Get("OutputDirectoryError", Output);
                }
            }

            // Rule #7: if the input assembly path is not null
            if (AssemblyPaths != null && AssemblyPaths.Count > 0)
            {
                Assemblies = new Assembly[AssemblyPaths.Count];
                for (int i = 0; i < Assemblies.Length; i++)
                {
                    string errorMsg = null;
                    try
                    {
                        // load the assembly
                        Assemblies[i] = Assembly.LoadFrom((string)AssemblyPaths[i]);
                    }
                    catch (ArgumentException argumentError)
                    {
                        errorMsg = argumentError.Message;
                    }
                    catch (BadImageFormatException formatError)
                    {
                        errorMsg = formatError.Message;
                    }
                    catch (FileNotFoundException fileError)
                    {
                        errorMsg = fileError.Message;
                    }
                    catch (PathTooLongException pathError)
                    {
                        errorMsg = pathError.Message;
                    }
                    catch (SecurityException securityError)
                    {

                        errorMsg = securityError.Message;
                    }

                    if (errorMsg != null)
                    {
                        return errorMsg; // return error message when loading this assembly
                    }
                }
            }

            // if we come to this point, we are all fine, return null error message
            return null;
        }

        /// <summary>
        /// Write message line depending on IsVerbose flag
        /// </summary>
        internal void WriteLine(string message)
        {
            if (IsVerbose)
            {
                Console.WriteLine(message);
            }
        }

        /// <summary>
        /// Write the message depending on IsVerbose flag
        /// </summary>        
        internal void Write(string message)
        {
            if (IsVerbose)
            {
                Console.Write(message);
            }
        }

        /// <summary>
        /// Factory method to get an object for writing the translation file
        /// </summary>
        public ITranslationWriter GetTranslationWriter()
        {
            switch (TranslationFileType)
            {
                case TranslationFileType.CSV:
                case TranslationFileType.TXT:
                    // ResourceTextWriter will dispose of the stream when it is disposed of
                    return new ResourceTextWriter(TranslationFileType, new FileStream(Output, FileMode.Create));
                case TranslationFileType.XLIFF:
                    // ResourceXliffWriter will dispose of the stream when it is disposed of
                    if (File.Exists(Output))
                    {
                        Xliff1_2.XliffObject existingFile;
                        try
                        {
                            using (FileStream stream = new FileStream(Output, FileMode.Open))
                            {
                                existingFile = Xliff1_2.XliffObject.Deserialize(stream);
                            }
                        }
                        catch (Exception e)
                        {
                            throw new Exception("Error reading existing XLIFF file: " + e.Message, e);
                        }
                        return new ResourceXliffWriter(this, new FileStream(Output, FileMode.Create), existingFile);
                    }
                    else
                    {
                        return new ResourceXliffWriter(this, new FileStream(Output, FileMode.Create));
                    }
                default:
                    throw new Exception("Unknown translation file type");
            }
        }

        public TranslationDictionariesReader GetTranslationsDictionary()
        {
            switch (TranslationFileType)
            {
                case TranslationFileType.CSV:
                case TranslationFileType.TXT:
                    Stream input = File.OpenRead(Translations);
                    using (ResourceTextReader reader = new ResourceTextReader(TranslationFileType, input))
                    {
                        return new TranslationDictionariesReader(reader);
                    }
                case TranslationFileType.XLIFF:
                    using (Stream xlfInput = File.OpenRead(Translations))
                    {
                        Xliff1_2.XliffObject xliff = Xliff1_2.XliffObject.Deserialize(xlfInput);
                        return new TranslationDictionariesReader(xliff);
                    }
                default:
                    throw new Exception("Unknown translation file type");
            }
        }
    }
}
