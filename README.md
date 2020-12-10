# UE Localization Tool
Tool for parsing localization .po files for UE

## About Localization tool

Localization tool features:

1) Import/export/merge .po files (check by key, replace translate field)
2) Import/export/merge .json files to current localization (.po file)
3) Import/export/merge .csv files to current localization (.po file)
4) Import/export/merge .xlsx files to current localization (.po file)
5) Shrink current localization for export only NotTranslated or New fields for example.

!!! 

One important thing: The program does not support creation of .po files, it only works with what has been imported, you can export the localization to any format, but you need to export the final localization to a .po file. For example import .xlsx, export to .po.

!!!

* [How it works](https://www.youtube.com/watch?v=eYbAv_A5AcY) - tutorial-example

## UI Description

File - Open File: Clear current localization and import supported format (.po, .json, .csv, .xlsx)

File - Clear Localization: - Clear current localization

File - Close: Close the tool

Import - (ext): Import file to current localization, if the localization was empty, it will create new fields, if the file has already been imported, it will compare the key and change the translation field

Export - (ext): Export current localization to file, if it is a .po file, then the tool will replace the translation fields (if a new one, then it will copy the localization to a new .po file (the file structure will be equivalent to the current .po file)

Special - Fast merge .po files: select two .po files for import and one for export(if it is an existing file, the program will replace the translation fields, if a new one, then it will copy the localization to a new .po file (the file structure will be equivalent to the 2 imported file) 

Special - Shrink: leave only fields of the specified status in localization

Color status: 
White: the field was in the previous localization and was not changed

Green: the new field

Yellow-green: the field was in the previous localization and was changed

Red: the field has no translation


### Prerequisites

The source code is available at the root of the Localization tool, if you just want to run the program itself, the executable file is in the PackagedBuild folder.

## Built With

* [Json.NET](https://www.newtonsoft.com/json) - JSON library
* [CsvHelper](https://joshclose.github.io/CsvHelper/) - CSV library
* [EPPlus](https://epplussoftware.com/) - Excel library

## Authors

* **Alex Lipatov** - UE Developer
