Updated version of Microsoft's locbaml tool.

## New Features
* Ability to read and write XLIFF 1.2 files (*.xlf)
  * These can be edited with the Multilingual Editor that's part of Microsoft's Multilingual App Toolkit
  * When writing to an existing XLIFF file the results are automatically merged with the old file
* Also builds a locbaml.dll file that contains Tasks that can be used from MSBuild instead of needing to run locbaml.exe from the command line

## Todo
* Filtering what elements and attributes to include in generated translation files
* Test more advanced use cases (more complex applications)
* ???

## Original [broken] links:

download source from microsoft: [http://archive.msdn.microsoft.com/wpfsamples](http://archive.msdn.microsoft.com/wpfsamples) - LocBaml Tool Sample

direct download link: [http://go.microsoft.com/fwlink/?LinkID=160016](http://go.microsoft.com/fwlink/?LinkID=160016)