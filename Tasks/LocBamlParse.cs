using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using System;
using System.Globalization;
using System.Reflection;

namespace BamlLocalization
{
    public class LocBamlParse : Task
    {
        [Required]
        public string OutputFile { get; set; }

        [Required]
        public string Culture { get; set; }

        [Required]
        public string InputFile { get; set; }

        public string[] Assemblies { get; set; }

        LocBamlOptions _Options;

        public override bool Execute()
        {
            _Options = new LocBamlOptions();
            _Options.ToParse = true;
            _Options.Input = InputFile;
            _Options.Output = OutputFile;
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
                Log.LogMessage("Done with LocBamlParse");
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
