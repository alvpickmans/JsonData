# JsonData 
JsonData is a [Dynamo](http://www.dynamobim.org) package to provide extra functionalities when handling data, based on Newtonsoft JsonNet C# library.

The original purpose was to provide a simpler way of handling data in Dynamo. Lists work just fine, but sometimes they are just not enough. With several functionalities to filter, update, combine and group your data, this package can help to boost your Dynamo workflows.
- [JsonData](#jsondata)
    - [Elements](#elements)
        - [JsonObject](#jsonobject)
            - [Constructors](#constructors)
            - [Methods](#methods)
            - [Properties](#properties)
        - [JsonArray](#jsonarray)
            - [Constructors](#constructors)
            - [Properties](#properties)
    - [Utilities](#utilities)
        - [Parse](#parse)
        - [Read](#read)
        - [Write](#write)


___

## Elements
The package is based on the use of two types objects **JsonObject** and **JsonArray**.

### JsonObject
This is the core element of the package. Being a `Dictionary` its underlying c# object type, JsonObjects provide several of a dictionary's methods with the extra value of displaying them using a json structure format.

#### Constructors

| &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; | Name           | Description  |
| ------------- |:-------------|:-----|
| ![ByKeysAndValues]  | **ByKeysAndValues** | JsonObject constructor by a given key-value pair. It accepts nested structures by providing keys divided by points as a single string. |

#### Methods

|  &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;     | Name | Description  |
| ----- |:-----| -----|
| ![Add]  | **Add** | Adds new attribute to the JsonObject. If given key already on the object and update set to True, value associated with the key will be updated. An error will be thrown otherwise. |
| ![FilterByKeyAndValue]  | **FilterByKeyAndValue** | Filters JsonObjects which contains the given key-value pair. If value is of type string, it will test if it contains the value given. |
| ![GetValueByKey]  | **GetValueByKey** | Returns the value associated with the given key from the jsonObject. |
| ![JsonOptions]  | **JsonOptions** | Options for updating JsonObjects: None, Update and Combine. |
| ![Merge]  | **Merge** | Merge one JsonObject with one or multiple other JsonObjects.|
| ![Remove]  | **Remove** |Remove keys from the given JsonObject. If any key doesn't exit on the object or duplicated keys found on the input, error will be thrown.|
| ![SortByKeyValue]  | **SortByKeyValue** | Sorts a list of JsonObjects by the ascending order of the values associated with the given key. |
| ![SortKeys]  | **SortKeys** | Sorts the JsonObject alphabetically by its keys. |

#### Properties

| &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; | Name | Description  |
| ----- |:-----| -----|
| ![Keys]  | **Keys** | Returns keys of attributes in the JsonObject. |
| ![JsonObjectSize]  | **Size** | Returns the number of attributes on the JsonObject. |
| ![Values]  | **Size** | Returns values of attributes in the JsonObject. |


### JsonArray
JsonArray acts more as a helper object than a key one. Due to Dynamo's lacing properties, I soon found that the value of a JsonObject couldn´t be a list of items, so here is were JsonArray comes into play: it is a c# `List` behaving as a single element, so JsonObjects can host them as a value.

#### Constructors

|         | Name           | Description  |
| ------------- |:-------------| -----|
| ![ByElements]  | **ByElements** | JsonArray constructor by a given list of elements. |

#### Properties

| &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;| Name | Description  |
| ----- |:-----| -----|
| ![Elements]  | **Elements** | Returns elements in the JsonArray object. |
| ![JsonObjectSize]  | **Size** | Returns the number of elements in the JsonArray object. |


## Utilities
The tools provided along with the package are currently focused on handling and parsing files from and to json format.

### Parse

| &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;| Name | Description  |
| ----- |:-----| -----|
| ![CSVString]  | **CSVString** | Parses a CSV formated string. It will return a list of JsonObjects. Error will be thrown if parser fails.|
| ![JsonString]  | **JsonString** | Parses a json formated string. It will return JsonObject, JsonArray or other match that the parser can do from the input. Error will be thrown if parser fails.|
| ![JsonToCSV]  | **JsonToCSV** | Converts a list of JsonObject to CSV string format. JsonObjects must have one level only (no other JsonObject or JsonArray as values), being the keys the header of the CSV string.Parses a CSV formated string. It will return a list of JsonObjects. Error will be thrown if parser fails.|
| ![JsonToXML]  | **JsonToXML** | Converts a JsonObject to XML string format. Parses a CSV formated string. It will return a list of JsonObjects. Error will be thrown if parser fails.|
| ![XMLString]  | **XMLString** | Parses a xml formated string. It will return JsonObject, JsonArray or other match that the parser can do from the input. Error will be thrown if parser fails.|

### Read

|&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;| Name | Description  |
| ----- |:-----| -----|
| ![FromCSVFile]  | **FromCSVFile** | Reads and parses a CSV formated file. It will return a list of JsonObjects. Error will be thrown if parser fails.|
| ![FromJsonFile]  | **FromJsonFile** | Reads and parses a json file. It will return JsonObject, JsonArray or other match that the parser can do from the input. Error will be thrown if parser fails.|
| ![FromXMLFile]  | **FromXMLFile** | Reads and parses a XML file. It will return JsonObject, JsonArray or other match that the parser can do from the input. Error will be thrown if parser fails.|

### Write

|&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;| Name | Description  |
| ----- |:-----| -----|
| ![ToCSVFile]  | **ToCSVFile** | Writes a list of JsonObject to a CSV file. JsonObjects must have one level only (no other JsonObject or JsonArray as values), being the keys the header of the CSV string.|
| ![ToJsonFile]  | **ToJsonFile** | Writes the JsonObject or JsonArray to a json file.|
| ![ToXMLFile]  | **ToXMLFile** | Writes the JsonObject or JsonArray to a XML file.|

[ByKeysAndValues]: assets/images/JsonData.Elements.JsonObject.ByKeysAndValues.Small.png
[Add]: assets/images/JsonData.Elements.JsonObject.Add.Small.png
[FilterByKeyAndValue]: assets/images/JsonData.Elements.JsonObject.FilterByKeyAndValue.Small.png
[GetValueByKey]: assets/images/JsonData.Elements.JsonObject.GetValueByKey.Small.png
[JsonOptions]: assets/images/JsonDataUI.JsonOptions.Small.png
[Merge]: assets/images/JsonData.Elements.JsonObject.Merge.Small.png
[Remove]: assets/images/JsonData.Elements.JsonObject.Remove.Small.png
[SortByKeyValue]: assets/images/JsonData.Elements.JsonObject.SortByKeyValue.Small.png
[SortKeys]: assets/images/JsonData.Elements.JsonObject.SortKeys.Small.png
[Keys]: assets/images/JsonData.Elements.JsonObject.Keys.Small.png
[JsonObjectSize]: assets/images/JsonData.Elements.JsonObject.Size.Small.png
[Values]: assets/images/JsonData.Elements.JsonObject.Values.Small.png

[ByElements]: assets/images/JsonData.Elements.JsonArray.ByElements.Small.png
[Elements]: assets/images/JsonData.Elements.JsonArray.Elements.Small.png
[JsonArraySize]: assets/images/JsonData.Elements.JsonArray.Size.Small.png

[CSVString]: assets/images/JsonData.Utilities.Parse.CSVString.Small.png
[JsonString]: assets/images/JsonData.Utilities.Parse.JsonString.Small.png
[JsonToCSV]: assets/images/JsonData.Utilities.Parse.JsonToCSV.Small.png
[JsonToXML]: assets/images/JsonData.Utilities.Parse.JsonToXML.Small.png
[XMLString]: assets/images/JsonData.Utilities.Parse.XMLString.Small.png

[FromCSVFile]: assets/images/JsonData.Utilities.Read.FromCSVFile.Small.png
[FromJsonFile]: assets/images/JsonData.Utilities.Read.FromJsonFile.Small.png
[FromXMLFile]: assets/images/JsonData.Utilities.Read.FromXMLFile.Small.png

[ToCSVFile]: assets/images/JsonData.Utilities.Write.ToCSVFile.Small.png
[ToJsonFile]: assets/images/JsonData.Utilities.Write.ToJsonFile.Small.png
[ToXMLFile]: assets/images/JsonData.Utilities.Write.ToXMLFile.Small.png