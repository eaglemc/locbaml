using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using System;
using System.Globalization;
using System.IO;
using System.Reflection;

namespace BamlLocalization
{
    public class LocBamlTranslate : Task
    {
        [Required]
        public string InputFile { get; set; }

        [Required]
        public string TranslationsFile { get; set; }

        [Required]
        public string Culture { get; set; }

        public string[] Assemblies { get; set; }

        LocBamlOptions _Options;

        public override bool Execute()
        {
            // First stage: parse
            _Options = new LocBamlOptions();
            _Options.ToParse = true;
            _Options.Input = InputFile;
            _Options.Output = TranslationsFile;
            _Options.CultureInfo = new CultureInfo(Culture);

            // Just to get rid of compiler warnings
            _Options.HasNoLogo = true;
            // generation-related options
            _Options.ToGenerate = false;
            _Options.Translations = string.Empty;

            // TODO:
            if ((Assemblies == null) || (Assemblies.Length == 0))
            {
                _Options.AssemblyPaths = null;
            }
            else
            {
                _Options.AssemblyPaths = new System.Collections.ArrayList();
                _Options.AssemblyPaths.AddRange(Assemblies);
            }

            AppDomain.CurrentDomain.AssemblyResolve += CurrentDomain_AssemblyResolve;

            // TODO: Add a logger interface so we can use MSBuild logger from
            // LocBamlOptions.Write() and WriteLine()
            _Options.IsVerbose = true;

            bool success = true;

            string errorString = _Options.CheckAndSetDefault();
            if (errorString != null)
            {
                Log.LogError(errorString);
                AppDomain.CurrentDomain.AssemblyResolve -= CurrentDomain_AssemblyResolve;
                success = false;
            }
            else
            {
                try
                {
                    TranslationDictionariesWriter.Write(_Options);
                }
                catch (Exception e)
                {
                    Log.LogError("Exception parsing: {0}", e.Message);
                    success = false;
                }
            }

            // TBD: a way to override automatic algorithm?
            string outputFolder = Path.GetDirectoryName(Path.GetFullPath(InputFile));
            string[] folderParts = outputFolder.Split(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
            // Try to figure out if the ouptut folder is a culture name, if so remove it
            foreach (CultureInfo culture in CultureInfo.GetCultures(CultureTypes.AllCultures))
            {
                string cName = culture.Name;
                if (string.IsNullOrEmpty(cName)) { continue; }
                if (folderParts[folderParts.Length - 1].Equals(cName, StringComparison.InvariantCultureIgnoreCase))
                {
                    outputFolder = string.Join(Path.DirectorySeparatorChar.ToString(), folderParts, 0, folderParts.Length - 1);
                    outputFolder = Path.Combine(outputFolder, _Options.CultureInfo.Name);
                    break;
                }
            }

            if (success)
            {
                // Don't call _Options.CheckAndSetDefault() again because it will reload the assemblies
                // generation-related options
                _Options.ToGenerate = true;
                _Options.Translations = TranslationsFile;
                _Options.Input = InputFile;
                _Options.Output = outputFolder;

                // TBD: Should we do this automatically or not?
                if (!Directory.Exists(outputFolder))
                {
                    Directory.CreateDirectory(outputFolder);
                }

                _Options.ToParse = false;

                try
                {
                    TranslationDictionariesReader dictionaries = _Options.GetTranslationsDictionary();
                    ResourceGenerator.Generate(_Options, dictionaries);
                }
                catch (Exception e)
                {
                    Log.LogError("Exception generating: {0}", e.Message);
                    success = false;
                }
            }

            // Cleanup
            AppDomain.CurrentDomain.AssemblyResolve -= CurrentDomain_AssemblyResolve;

            return success;
        }

        private Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
        {
            if (_Options.AssemblyPaths != null)
            {
                foreach (Assembly assembly in _Options.Assemblies)
                {
                    var name = assembly.GetName();
                    if ((name.FullName == args.Name) || (name.Name == args.Name))
                    {
                        return assembly;
                    }
                }
            }
            return null;
        }
    }
}