using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using System;
using System.Globalization;
using System.Reflection;


namespace BamlLocalization
{
    public class LocBamlGenerate : Task
    {
        [Required]
        public string TranslationsFile { get; set; }

        [Required]
        public string Culture { get; set; }

        /// <summary>
        /// Native satelite assembly which contains all resources
        /// </summary>
        [Required]
        public string InputFile { get; set; }

        [Required]
        public string OutputFolder { get; set; }

        public string[] Assemblies { get; set; }

        LocBamlOptions _Options;

        public override bool Execute()
        {
            _Options = new LocBamlOptions();
            _Options.CultureInfo = new CultureInfo(Culture);

            // generation-related options
            _Options.ToGenerate = true;
            _Options.Translations = TranslationsFile;
            _Options.Input = InputFile;
            _Options.Output = OutputFolder;

            // TBD: Should we do this automatically or not?
            if (!System.IO.Directory.Exists(OutputFolder))
            {
                System.IO.Directory.CreateDirectory(OutputFolder);
            }

            _Options.ToParse = false;

            // Just to get rid of compiler warnings
            _Options.HasNoLogo = true;

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
                success = false;
            }
            else
            {
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
                Log.LogMessage("Done with LocBamlGenerate");
            }
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
