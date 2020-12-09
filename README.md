# UE Localization Tool
Tool for parsing localization .po files for UE

## About Localization tool

Localization tool features:

1) Import/export/merge .po files (check by key, replace translate field)
2) Import/export/merge .json files to current localization (.po file)
3) Import/export/merge .csv files to current localization (.po file)
4) Import/export/merge .xlsx files to current localization (.po file)
5) Shrink current localization for export only NotTranslated or New fields for example.

!!! One important thing: The program does not support creation of .po files, it only works with what has been imported, you can export the localization to any format, but you need to export the final localization to a .po file. For example import to .xlsx, export to .po.!!!

### Prerequisites

The source code is available at the root of the Localization tool, if you just want to run the program itself, the executable file is in the PackagedBuild folder.

## Built With

* [Json.NET](https://www.newtonsoft.com/json) - JSON library
* [CsvHelper](https://joshclose.github.io/CsvHelper/) - CSV library
* [EPPlus](https://epplussoftware.com/) - Excel library

## Authors

* **Alex Lipatov** - UE Developer