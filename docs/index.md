# JsonData 
JsonData is a [Dynamo](http://www.dynamobim.org) package to provide extra functionalities when handling data, based on Newtonsoft JsonNet C# library.

The original purpose was to provide a simpler way of handling data in Dynamo. Lists work just fine, but sometimes they are just not enough. With several functionalities to filter, update, combine and group your data, this package can help to boost your Dynamo workflows.

The package is divided in two groups: Elements and Utilities.

___

## Elements
The package is based on the use of two types objects **JsonObject** and **JsonArray**.

### JsonObject
This is the core element of the package. Being a `Dictionary` its underlying c# object type, JsonObjects provide several of a dictionary methods with the extra value of displaying them using a json structure format.

#### Constructor Methods

|         | Name           | Description  |
| ------------- |:-------------| -----|
| ![ByKeysAndValues]  | **ByKeysAndValues** | JsonObject constructor by a given key-value pair. It accepts nested structures by providing keys divided by points as a single string. |

#### Properties
|       | Name | Description  |
| ----- |:-----| -----|
| ![Keys]  | **Keys** | Returns keys of attributes in the JsonObject. |
| ![Size]  | **Size** | Returns the number of attributes on the JsonObject. |
| ![Values]  | **Size** | Returns values of attributes in the JsonObject. |

#### Methods
|       | Name | Description  |
| ----- |:-----| -----|
| ![Keys]  | **Keys** | Returns keys of attributes in the JsonObject. |




[ByKeysAndValues]: ../JsonData/Resources/Images/Small/JsonData.Elements.JsonObject.ByKeysAndValues.Small.png
[Keys]: ../JsonData/Resources/Images/Small/JsonData.Elements.JsonObject.Keys.Small.png
[Size]: ../JsonData/Resources/Images/Small/JsonData.Elements.JsonObject.Size.Small.png
[Values]: ../JsonData/Resources/Images/Small/JsonData.Elements.JsonObject.Values.Small.png