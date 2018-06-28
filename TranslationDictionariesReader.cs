using BamlLocalization.Xliff1_2;
using System;
using System.Collections;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Markup.Localizer;

namespace BamlLocalization
{
    /// <summary>
    /// Reader to read the translations from CSV or tab-separated txt file    
    /// </summary> 
    internal class TranslationDictionariesReader
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="reader">resoure text reader that reads CSV or a tab-separated txt file</param>
        internal TranslationDictionariesReader(ResourceTextReader reader)
        {
            if (reader == null)
                throw new ArgumentNullException("reader");

            // hash key is case insensitive strings
            _table = new Hashtable();

            // we read each Row
            int rowNumber = 0;
            while (reader.ReadRow())
            {
                rowNumber++;

                // field #1 is the baml name.
                string bamlName = reader.GetColumn(0);

                // it can't be null
                if (bamlName == null)
                    throw new ApplicationException(StringLoader.Get("EmptyRowEncountered"));

                if (string.IsNullOrEmpty(bamlName))
                {
                    // allow for comment lines in csv file.
                    // each comment line starts with ",". It will make the first entry as String.Empty.
                    // and we will skip the whole line.
                    continue;   // if the first column is empty, take it as a comment line
                }

                // field #2: key to the localizable resource
                string key = reader.GetColumn(1);
                if (key == null)
                    throw new ApplicationException(StringLoader.Get("NullBamlKeyNameInRow"));

                BamlLocalizableResourceKey resourceKey = LocBamlConst.StringToResourceKey(key);

                // get the dictionary 
                BamlLocalizationDictionary dictionary = this[bamlName];
                if (dictionary == null)
                {
                    // we create one if it is not there yet.
                    dictionary = new BamlLocalizationDictionary();
                    this[bamlName] = dictionary;
                }

                BamlLocalizableResource resource;

                // the rest of the fields are either all null,
                // or all non-null. If all null, it means the resource entry is deleted.

                // get the string category
                string categoryString = reader.GetColumn(2);
                if (categoryString == null)
                {
                    // it means all the following fields are null starting from column #3.
                    resource = null;
                }
                else
                {
                    // the rest must all be non-null.
                    // the last cell can be null if there is no content
                    for (int i = 3; i < 6; i++)
                    {
                        if (reader.GetColumn(i) == null)
                            throw new Exception(StringLoader.Get("InvalidRow"));
                    }

                    // now we know all are non-null. let's try to create a resource
                    resource = new BamlLocalizableResource();

                    // field #3: Category
                    resource.Category = (LocalizationCategory)StringCatConverter.ConvertFrom(categoryString);

                    // field #4: Readable
                    resource.Readable = (bool)BoolTypeConverter.ConvertFrom(reader.GetColumn(3));

                    // field #5: Modifiable
                    resource.Modifiable = (bool)BoolTypeConverter.ConvertFrom(reader.GetColumn(4));

                    // field #6: Comments
                    resource.Comments = reader.GetColumn(5);

                    // field #7: Content
                    resource.Content = reader.GetColumn(6);

                    // in case content being the last column, consider null as empty.
                    if (resource.Content == null)
                        resource.Content = string.Empty;

                    // field > #7: Ignored.
                }

                // at this point, we are good.
                // add to the dictionary.
                dictionary.Add(resourceKey, resource);
            }
        }

        internal TranslationDictionariesReader(XliffObject xliff)
        {
            // hash key is case insensitive strings
            _table = new Hashtable();

            foreach (File file in xliff.Files)
            {
                string bamlName = file.Original;

                // get the dictionary 
                BamlLocalizationDictionary dictionary = this[bamlName];
                if (dictionary == null)
                {
                    // we create one if it is not there yet.
                    dictionary = new BamlLocalizationDictionary();
                    this[bamlName] = dictionary;
                }

                Body body = file.Body;
                // There should only be one group, but go through any that exist for good measure
                foreach (Group group in body.Groups)
                {
                    foreach (TranslationUnit transUnit in group.TranslationUnits)
                    {
                        string key = transUnit.Id;
                        BamlLocalizableResourceKey resourceKey = LocBamlConst.StringToResourceKey(key);
                        BamlLocalizableResource resource = new BamlLocalizableResource();
                        resource.Category = (LocalizationCategory)StringCatConverter.ConvertFrom(transUnit.ResourceType);

                        /*
                        resource.Readable = (bool)BoolTypeConverter.ConvertFrom(reader.GetColumn(3));
                        resource.Modifiable = (bool)BoolTypeConverter.ConvertFrom(reader.GetColumn(4));
                        */
                        Note comment = transUnit.Notes.FirstOrDefault(n => n.From == "MultilingualBuild");
                        if (comment != null)
                        {
                            resource.Comments = comment.Text;
                        }
                        resource.Content = transUnit.Target.Content ?? string.Empty;

                        dictionary.Add(resourceKey, resource);
                    }
                }
            }
        }

        internal BamlLocalizationDictionary this[string key]
        {
            get
            {
                return (BamlLocalizationDictionary)_table[key.ToLowerInvariant()];
            }
            set
            {
                _table[key.ToLowerInvariant()] = value;
            }
        }

        // hashtable that maps from baml name to its ResourceDictionary
        private Hashtable _table;
        private static TypeConverter BoolTypeConverter = TypeDescriptor.GetConverter(true);
        private static TypeConverter StringCatConverter = TypeDescriptor.GetConverter(LocalizationCategory.Text);
    }
}
