#region namesapces
using Autodesk.DesignScript.Runtime;
using Dynamo.Library;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
#endregion

namespace JsonData.Elements
{
    /// <summary>
    /// Class for handle information in a json format.
    /// </summary>
    [JsonConverter(typeof(JsonObjectConverter))]
    public class JsonObject : JsonNet
    {
        #region Variables
        internal Dictionary<string, object> dict = new Dictionary<string, object>();

        /// <summary>
        /// Returns keys of attributes in the JsonObject.
        /// </summary>
        /// <returns name="keys">JsonObject Keys</returns>
        /// <search>
        /// json, jsonobject, keys
        /// </search>
        public List<string> Keys => dict.Keys.ToList();

        /// <summary>
        /// Returns values of attributes in the JsonObject.
        /// </summary>
        /// <returns name="values">JsonObject Values</returns>
        /// <search>
        /// json, jsonobject, values
        /// </search>
        [AllowRankReduction]
        public List<object> Values => dict.Values.ToList();
        
        /// <summary>
        /// Returns the number of attributes on the JsonObject.
        /// </summary>
        /// <returns name="size">Number of attributes on JsonObject</returns>
        /// <search>
        /// json, jsonobject, size
        /// </search>
        public int Size => dict.Count;

        #endregion

        #region Internal Constructors

        internal JsonObject() { }

        /// <summary>
        /// JsonObject constructor by a given key-value pair of items.
        /// </summary>
        /// <param name="keys">Key or keys</param>
        /// <param name="values">Value or values</param>
        /// <param name="nesting">Apply nesting behaviour if key input is a single string concatenated by dots, representing the desired nested structure</param>
        /// <param name="jsonOption">Options to modify a JsonObject</param>
        internal JsonObject(List<string> keys, List<object> values, bool nesting, JsonOption jsonOption)
        {
            if (nesting)
            {
                List<KeyValuePair<string, object>> pair = keys.Zip(values, (k, v) => new KeyValuePair<string, object>(k, v)).ToList();
                pair.ForEach(p => AddRecursive(this, p.Key, p.Value, jsonOption));
            }
            else
            {
                List<KeyValuePair<string, object>> pair = keys.Zip(values, (k, v) => new KeyValuePair<string, object>(k, v)).ToList();
                pair.ForEach(p => AddInternal(this, p.Key, p.Value, jsonOption));
            }
        }


        /// <summary>
        /// JsonObject constructor by a given JObject element.
        /// </summary>
        /// <param name="jObject">JObject element.</param>
        /// <param name="nesting">Apply nesting behaviour if key input is a single string concatenated by 
        /// dots, representing the desired nested structure</param>
        internal JsonObject (JObject jObject, bool nesting = false)
        {
            List<string> keys = new List<string>();
            List<object> values = new List<object>();
            foreach (JProperty p in jObject.Properties())
            {
                keys.Add(p.Name);
                //Replacing null values to "null" string. When parsing XML, some items might be null and cause an exception.
                //TODO: Find a better way of handling nulls.
                object value = (p.Value.ToObject<object>() == null) ? "null" : JsonNet.ReturnValidObject(p.Value);
                values.Add(value);
            }

            List<KeyValuePair<string, object>> pair = keys.Zip(values, (k, v) => new KeyValuePair<string, object>(k, v)).ToList();
            pair.ForEach(p => AddInternal(this, p.Key, p.Value, JsonOption.None));
        }

        #endregion
        
        #region Internal Methods

        /// <summary>
        /// Static method to add a value to a JsonObject. Used for normal and recursive additions.
        /// </summary>
        /// <param name="json">JsonObject where to add a new item</param>
        /// <param name="key">Key</param>
        /// <param name="value">Value</param>
        /// <param name="jsonOption">Options</param>
        /// <returns></returns>
        [IsVisibleInDynamoLibrary(false)]
        internal static JsonObject AddInternal(JsonObject json, string key, object value, JsonOption jsonOption)
        {
            if (json.dict.ContainsKey(key))
            {
                switch (jsonOption)
                {
                    case JsonOption.Update:
                        json.dict[key] = value;
                        break;
                    case JsonOption.Combine:
                        if(json.dict[key] is IList<object> && json.dict[key].GetType().IsGenericType)
                        {
                            List<object> listValue = json.dict[key] as List<object>;
                            listValue.Add(value);
                            json.dict[key] = listValue;
                        }
                        else
                        {
                            json.dict[key] = new List<object>() { json.dict[key], value };
                        }
                        break;
                    default:
                        json.dict.Add(key, value);
                        break;
                }
            }
            else
            {
                json.dict.Add(key, value);
            }
            return json;
        }

        /// <summary>
        /// Method to add values recursively to a JsonObject.
        /// </summary>
        /// <param name="json"></param>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="jsonOption"></param>
        /// <returns></returns>
        internal static JsonObject AddRecursive(JsonObject json, string key, object value, JsonOption jsonOption)
        {
            //If key doens't contain a dot, recursiveness is not applied.
            if (!key.Contains("."))
            {
                json = AddInternal(json, key, value, jsonOption);
            }
            else
            {
                string[] keys = key.Split('.');
                string k = keys.First();
                string restOfKeys = String.Join(".", keys.Skip(1).ToArray());
                bool containsKey = json.dict.ContainsKey(k);

                //If contains key, the associated value will be a JsonObject.
                if (containsKey)
                {
                    JsonObject newJson = json.dict[k] as JsonObject;
                    JsonObject jsonRecursive = AddRecursive(newJson, restOfKeys, value, jsonOption);
                    json.dict[k] = jsonRecursive;
                }
                else
                {
                    JsonObject newJson = new JsonObject();
                    JsonObject jsonRecursive = AddRecursive(newJson, restOfKeys, value, jsonOption);
                    json = AddInternal(json, k, jsonRecursive, jsonOption);
                }

            }
            return json;

        }


        /// <summary>
        /// Returns the value associated with the given key from the dict.
        /// </summary>
        /// <param name="jsonObject">JsonObject to get values from</param>
        /// <param name="key">Attribute's key</param>
        /// <param name="nesting"></param>
        /// <returns name="value">Value returned</returns>
        /// <search>
        /// json, jsonobject, search, bykey
        /// </search> 
        internal static object GetValueRecursive(JsonObject jsonObject, string key, bool nesting = true)
        {
            if (nesting && key.Contains("."))
            {
                string[] keys = key.Split('.');
                string restOfKeys = String.Join(".", keys.Skip(1).ToArray());
                if(jsonObject.dict[keys.First()].GetType() == typeof(JsonObject))
                {
                    JsonObject json = jsonObject.dict[keys.First()] as JsonObject;
                    return GetValueRecursive(json, restOfKeys, nesting);
                }
                else
                {
                    return null;
                }
            }
            else
            {
                return jsonObject.dict.ContainsKey(key) ? jsonObject.dict[key] : null;
            }
        }

        #endregion
        
        #region Public Constructors

        /// <summary>
        /// JsonObject constructor by a given key-value pair.
        /// It accepts nested structures by providing keys divided by points as a single string.
        /// </summary>
        /// <param name="keys"> Key or list of keys</param>
        /// <param name="values">Value or list of values</param>
        /// <param name="nesting">Apply nesting behaviour if key input is a single string concatenated by 
        /// dots, representing the desired nested structure</param>
        /// <param name="jsonOption">Handling options where duplicate keys are found. Use JsonOptions 
        /// dropdown node to select appropiate behaviour</param>
        /// <returns name="jsonObject">New JsonObject</returns>
        /// <search>
        /// json, jsonobject, create, bykeysandvalues
        /// </search> 
        [IsVisibleInDynamoLibrary(false)]
        public static JsonObject ByKeysAndValues([ArbitraryDimensionArrayImport]List<string> keys, 
            [KeepReference] [ArbitraryDimensionArrayImport]List<object> values, 
            [DefaultArgument("true")] bool nesting, 
            [DefaultArgument("JsonOption.None")] JsonOption jsonOption)
        {
            if(keys == null) { throw new ArgumentNullException("keys"); }
            if(values == null) { throw new ArgumentNullException("values"); }
            if(keys.Count != values.Count) {
                string error = String.Format("Keys and values need to be of same size.\nKeys: {0}\nValues: {1}", keys.Count, values.Count);
                throw new ArgumentException(error);
            }
            if (values.Contains(null)) { throw new ArgumentNullException("values", "Values' input contains one or more null elements."); }
            return new JsonObject(keys, values, nesting, jsonOption);
        }

        /// <summary>
        /// Creates a new instance of JsonObject from a Dynamo Dictionary's components.
        /// </summary>
        /// <param name="dictionary">DesignDcript.Builtin.Dictionary</param>
        /// <returns name="jsonObject">New JsonObject</returns>
        public static JsonObject ByDictionary(DesignScript.Builtin.Dictionary dictionary)
        {
            List<string> keys = dictionary.Keys.ToList();
            List<object> values = new List<object>();
            foreach(var value in dictionary.Values)
            {
                if(value.GetType() == typeof(DesignScript.Builtin.Dictionary))
                {
                    values.Add(JsonObject.ByDictionary(value as DesignScript.Builtin.Dictionary));
                }
                else
                {
                    values.Add(value);
                }
            }

            return new JsonObject(keys, values, true, JsonOption.None);
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Adds new attribute to the JsonObject. If given key already on the object and update set to
        /// True, value associated with the key will be updated. An error will be thrown otherwise.
        /// </summary>
        /// <param name="key">Key to add or update</param>
        /// <param name="value">Value to add or update</param> 
        /// <param name="nesting">Apply nesting behaviour if key input is a single string concatenated by 
        /// dots, representing the desired nested structure</param>
        /// <param name="jsonOption">Handling options where duplicate keys are found. Use JsonOptions dropdown node to select appropiate behaviour</param>
        /// <returns name="jsonObject">dict with added attribute/s</returns>
        /// <search>
        /// json, jsonobject, add
        /// </search> 
        [IsVisibleInDynamoLibrary(false)]
        public static JsonObject Add(JsonObject jsonObject, List<string> key, List<object> value, bool nesting, JsonOption jsonOption)
        {
            if(key == null) { throw new ArgumentNullException("key"); }
            if(value == null) { throw new ArgumentNullException("value"); }
            List<string> keys = jsonObject.Keys.Concat(key).ToList();
            List<object> values = jsonObject.Values.Concat(value).ToList();
            
            return new JsonObject(keys, values, nesting, jsonOption);
        }

        /// <summary>
        /// Remove keys from the given JsonObject if they exist.
        /// </summary>
        /// <param name="keys">Key or list of keys to remove</param>
        /// <returns name="jsonObject">JsonObject with keys removed</returns>
        /// <param name="nesting">Apply nesting behaviour if key input is a single string concatenated by 
        /// dots, representing the desired nested structure</param>
        /// <search>
        /// json, jsonobject, remove
        /// </search> 
        [IsVisibleInDynamoLibrary(false)]
        public static JsonObject Remove(JsonObject jsonObject, List<string> keys, bool nesting = true)
        {
            Dictionary<string, object> dict = new Dictionary<string, object>(jsonObject.dict);
            
            foreach(string key in keys)
            {
                if (nesting && key.Contains("."))
                {
                    string[] keysArray = key.Split('.').ToArray();
                    string currentKey = keysArray.First();
                    string nestedKey = String.Join(".", keysArray.Skip(1).ToArray());
                    JsonObject json = dict[currentKey] as JsonObject;
                    JsonObject returnedJson = JsonObject.Remove(json, new List<string>() { nestedKey }, true);
                    if(returnedJson.Size > 0)
                    {
                        dict[currentKey] = returnedJson;
                    }
                    else
                    {
                        dict.Remove(currentKey);
                    }
                }
                else
                {
                    dict.Remove(key);
                }
            }

            return new JsonObject(dict.Keys.ToList(), dict.Values.ToList(), false, JsonOption.None);
        }

        /// <summary>
        /// Merge one JsonObject with one or multiple other JsonObjects.
        /// </summary>
        /// <param name="others">JsonObject(s) to merge into the main one</param>
        /// <param name="jsonOption">Handling options where duplicate keys are found. Use JsonOptions dropdown node to select appropiate behaviour</param>
        /// <returns name="jsonObject">New JsonObject</returns>
        /// <search>
        /// json, jsonobject, merge
        /// </search> 
        [IsVisibleInDynamoLibrary(false)]
        public static JsonObject Merge(JsonObject jsonObject, List<JsonObject> others, [DefaultArgument("JsonOption.None")] JsonOption jsonOption)
        {
            Dictionary<string, object> dict = new Dictionary<string, object>();
            List<string> keys = new List<string>();
            List<object> values = new List<object>();
            foreach (KeyValuePair<string, object> x in jsonObject.dict.ToList())
            {
                keys.Add(x.Key);
                values.Add(x.Value);
            }
            foreach (var other in others)
            {
                foreach (KeyValuePair<string, object> y in other.dict.ToList())
                {
                    keys.Add(y.Key);
                    values.Add(y.Value);
                }
            }

            return new JsonObject(keys, values, true, jsonOption);
        }

        /// <summary>
        /// Returns the value associated with the given key from the dict. Returns null if key is not found.
        /// </summary>
        /// <param name="key">Attribute's key</param>
        /// <param name="nesting">Apply nesting behaviour if key input is a single string concatenated by 
        /// dots, representing the desired nested structure</param>
        /// <returns name="value">Value returned</returns>
        /// <search>
        /// json, jsonobject, search, bykey
        /// </search> 
        [IsVisibleInDynamoLibrary(false)]
        public static object GetValueByKey(JsonObject jsonObject, string key, bool nesting = true)
        {
            return GetValueRecursive(jsonObject, key, nesting);
        }

        /// <summary>
        /// Sorts JsonObject's properties alphabetically by its keys.
        /// </summary>
        /// <returns name="sortedJsonObject">Sorted JsonObject</returns>
        public JsonObject SortKeys()
        {
            var sorted = this.dict
                .OrderBy(j => j.Key);
            List<string> keys = sorted.Select(item => item.Key).ToList();
            List<object> values = sorted.Select(item => item.Value).ToList();
            return new JsonObject(keys, values, false, JsonOption.None);
        }

        /// <summary>
        /// Sorts a list of JsonObjects by the ascending order of the values associated with the given key.
        /// </summary>
        /// <param name="jsonObjects">List of JsonObjects</param>
        /// <param name="key">Key to sort JsonObjects with</param>
        /// <returns></returns>
        [MultiReturn(new[] { "sortedJsonObjects", "sortedValues" })]
        public static Dictionary<string, object> SortByKeyValue(List<JsonObject> jsonObjects, string key)
        {
            var sorted = jsonObjects
                        .Select((json) => new KeyValuePair<JsonObject, object>(json, GetValueRecursive(json, key)))
                        .OrderBy(item => item.Value)
                        .ToList();
            
            List<JsonObject> sortedJObjects = sorted.Select(x => x.Key).ToList();
            List<object> sortedValues = sorted.Select(x => x.Value).ToList();

            return new Dictionary<string, object>()
            {
                {"sortedJsonObjects", sortedJObjects },
                {"sortedValues", sortedValues }
            };
        }

        /// <summary>
        /// Filters JsonObjects which contains the given key-value pair. If value is of type string, it will test if it contains the value given.
        /// </summary>
        /// <param name="jsonObjects">List of JsonObjects</param>
        /// <param name="key">Key to look for</param>
        /// <param name="value">Value to match</param>
        /// <returns name="in">JsonObjects matching the key-value pair given</returns>
        /// <returns name="out">JsonObjects not matching the key-value pair given</returns>
        /// <search>
        /// json, jsonobject, filter
        /// </search> 
        [MultiReturn(new[] { "in", "out" })]
        public static Dictionary<string, List<JsonObject>> FilterByKeyAndValue(List<JsonObject> jsonObjects, string key, object value)
        {
            if(key == null) { throw new ArgumentNullException("key"); }
            if (value == null) { throw new ArgumentNullException("value"); }
            

            List<JsonObject> inJson = new List<JsonObject>();
            List<JsonObject> outJson = new List<JsonObject>();
            
            foreach (var js in jsonObjects)
            {
                bool valid = false;

                if (js.dict.ContainsKey(key))
                {
                    object comparer = js.dict[key];
                    switch (Type.GetTypeCode(value.GetType()))
                    {
                        case TypeCode.String:
                            string str = comparer as String;
                            string v = value as String;
                            valid = (str == null) ? false : str.ToLower().Contains(v.ToLower());
                            break;
                        default:
                            valid = comparer.Equals(value);
                            break;
                    }
                }

                if (valid)
                {
                    inJson.Add(js);
                }
                else
                {
                    outJson.Add(js);
                }
            }

            return new Dictionary<string, List<JsonObject>>()
            {
                {"in", inJson },
                {"out", outJson }
            };

        }

        /// <summary>
        /// Creates a new instance of Dynamo Dictionary from a JsonObject's components
        /// </summary>
        /// <param name="jsonObject">JsonObject</param>
        /// <returns name="dictionary">DesignScript.Builtin.Dictionary</returns>
        public static DesignScript.Builtin.Dictionary ToDictionary(JsonObject jsonObject)
        {
            List<string> keys = jsonObject.Keys;
            List<object> values = new List<object>();
            foreach (var value in jsonObject.Values)
            {
                if (value.GetType() == typeof(JsonObject))
                {
                    values.Add(JsonObject.ToDictionary(value as JsonObject));
                }
                else
                {
                    values.Add(value);
                }
            }
            return DesignScript.Builtin.Dictionary.ByKeysValues(keys, values);
        }

        /// <summary>
        /// Serialize dict to String
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return JsonConvert.SerializeObject(this, Formatting.Indented);
        }

        #endregion
    }

}
