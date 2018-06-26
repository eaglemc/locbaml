﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Markup.Localizer;
using BamlLocalization.Xliff1_2;
using System.Xml.Serialization;

namespace BamlLocalization
{
    class ResourceXliffWriter : ITranslationWriter, IDisposable
    {
        System.IO.Stream _OutputStream;
        XliffObject _RootObject;
        string _TargetLanguage;

        public ResourceXliffWriter(LocBamlOptions options, System.IO.Stream output)
        {
            _OutputStream = output;
            _RootObject = new XliffObject();
            _TargetLanguage = options.CultureInfo.Name;
        }

        public void Dispose()
        {
            if (_OutputStream != null)
            {
                _RootObject.Serialize(_OutputStream);
                _OutputStream.Dispose();
                _OutputStream = null;
            }
        }

        public void WriteResource(string bamlStreamName, string resourceKey, BamlLocalizableResource resource)
        {
            // Try to get the source culture from the stream name (this is pretty kludgey...)
            string[] nameParts = bamlStreamName.Split('.', ':');
            string sourceLanguage = string.Empty;
            for (int i = 1; i < nameParts.Length; i++)
            {
                if (nameParts[i] == "resources")
                {
                    sourceLanguage = nameParts[i - 1];
                }
            }

            File file = _RootObject.Files.FirstOrDefault(f => f.Original == bamlStreamName);
            if (file == null)
            {
                file = new File();
                file.Original = bamlStreamName;
                file.TargetLanguage = _TargetLanguage;
                file.SourceLanguage = sourceLanguage;
                _RootObject.Files.Add(file);
            }
            Body body = file.Body;

            Group group = body.Groups.FirstOrDefault(g => g.Id == bamlStreamName);
            if (group == null)
            {
                group = new Group() { DataType = "xml", Id = bamlStreamName };
                body.Groups.Add(group);
            }
            TranslationUnit unit = group.TranslationUnits.FirstOrDefault(tu => tu.Id == resourceKey);
            if (unit == null)
            {
                unit = new TranslationUnit()
                {
                    Id = resourceKey,
                    Source = resource.Content
                };
                group.TranslationUnits.Add(unit);
            }
            else
            {
                // TODO: Don't do anything if untranslated string is the same, figure out what to do if they're different....
            }
            if (!string.IsNullOrEmpty(resource.Comments))
            {
                const string SOURCE = "MultilingualBuild";
                Note note = unit.Notes.FirstOrDefault(n => n.From == SOURCE);
                if (note == null)
                {
                    note = new Note() { From = SOURCE };
                    unit.Notes.Add(note);
                }
                note.Annotates = "source";
                note.Priority = 2;
                note.Text = resource.Comments;
            }
        }
    }
}
