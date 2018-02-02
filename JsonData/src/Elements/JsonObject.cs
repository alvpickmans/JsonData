#region namesapces
using Autodesk.DesignScript.Runtime;
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
        internal Dictionary<string, object> jsonObject = new Dictionary<string, object>();

        /// <summary>
        /// Returns keys of attributes in the JsonObject.
        /// </summary>
        /// <returns name="keys">JsonObject Keys</returns>
        /// <search>
        /// json, jsonobject, keys
        /// </search>
        public List<string> Keys
        {
            get { return jsonObject.Keys.ToList(); }
        }


        /// <summary>
        /// Returns values of attributes in the JsonObject.
        /// </summary>
        /// <returns name="values">JsonObject Values</returns>
        /// <search>
        /// json, jsonobject, values
        /// </search>
        public List<dynamic> Values
        {
            get { return GetValues(this); }
        }

        /// <summary>
        /// Returns the number of attributes on the JsonObject.
        /// </summary>
        /// <returns name="size">Number of attributes on JsonObject</returns>
        /// <search>
        /// json, jsonobject, size
        /// </search>
        public int Size
        {
            get { return jsonObject.Count; }
        }



        #endregion

        #region Constructors

        internal static JsonObject JsonObjectRecursive(JsonObject j, string key, dynamic value, [DefaultArgument("JsonOption.None")] JsonOption jsonOption, int level = 0)
        {
            IsValidKey(key);
            string[] keys = key.Split('.');
            string k = keys.First();
            if (keys.Count() < 2)
            {
                j = Add(j, k, value, jsonOption);
            }
            else
            {
                string restOfKeys = String.Join(".", keys.Skip(1).ToArray());
                bool containsKey = j.jsonObject.ContainsKey(k);
                JsonObject newJson;
                bool updateKey = false;
                if (containsKey)
                {
                    newJson = j.GetValueByKey(k) as JsonObject;
                    containsKey = false;
                    updateKey = true;
                }
                else
                {
                    newJson = new JsonObject();
                }
                
                JsonObject jO = JsonObjectRecursive(newJson, restOfKeys, value, jsonOption, level + 1);
                if (containsKey)
                {
                    newJson = newJson.Merge(new List<JsonObject>() { jO }, JsonOption.Combine);
                    j.jsonObject[k] = newJson;
                }
                else if (updateKey)
                {
                    j.jsonObject[k] = jO;
                }
                else
                {
                    j = Add(j, k, jO, jsonOption);
                }

            }
            return j;

        }

        internal JsonObject()
        {
            InitJsonNet(typeof(JsonObject));
        }

        /// <summary>
        /// JsonObject constructor by a given key-value pair of items.
        /// </summary>
        /// <param name="keys">Key or keys</param>
        /// <param name="values">Value or values</param>
        /// <param name="nesting">Apply nesting behaviour if key input is a single string concatenated by dots, representing the desired nested structure</param>
        /// <param name="jsonOption">Options to modify a JsonObject</param>
        internal JsonObject(List<string> keys, List<dynamic> values, [DefaultArgument("true")] bool nesting, [DefaultArgument("JsonOption.None")] JsonOption jsonOption)
        {
            InitJsonNet(typeof(JsonObject));
           
            for (var i = 0; i < keys.Count; i++)
            {
                string key = keys[i];
                object value = values[i];
                if (nesting && key.Contains("."))
                {
                    this.jsonObject = JsonObjectRecursive(this, key, value, jsonOption).jsonObject;
                }
                else
                {
                    this.jsonObject = Add(this, key, value, jsonOption).jsonObject;
                }
            }
        }


        /// <summary>
        /// JsonObject constructor by a given JObject element.
        /// </summary>
        /// <param name="jObject">JObject element.</param>
        /// <param name="nesting">Apply nesting behaviour if key input is a single string concatenated by 
        /// dots, representing the desired nested structure</param>
        internal static JsonObject ByJObject(JObject jObject, bool nesting = false)
        {
            List<string> keys = new List<string>();
            List<object> values = new List<object>();
            foreach (JProperty p in jObject.Properties())
            {
                keys.Add(p.Name);
                //Replacing null values to "null" string. When parsing XML, some items might be null and cause an exception.
                //Need to find a better way of handling nulls.
                object value = (p.Value.ToObject<object>() == null) ? "null" : p.Value.ToObject<object>();
                values.Add(value);
            }

            return new JsonObject(keys, values, nesting, JsonOption.None);
        }

        #endregion
        /// <summary>
        /// Static method to add a value to a JsonObject. Used for normal and recursive additions.
        /// </summary>
        /// <param name="j">JsonObject where to add a new item</param>
        /// <param name="key">Key</param>
        /// <param name="value">Value</param>
        /// <param name="jsonOption">Options</param>
        /// <returns></returns>
        internal static JsonObject Add(JsonObject j, string key, dynamic value, JsonOption jsonOption)
        {

            IsValidKey(key);
            IsValidValueAndException(value);

            if (j.jsonObject.ContainsKey(key))
            {
                switch (jsonOption)
                {
                    case JsonOption.Update:
                        j.jsonObject[key] = value;
                        break;
                    case JsonOption.Combine:
                        CombineDuplicates(j, key, value);
                        break;
                    default:
                        j.jsonObject.Add(key, value);
                        break;
                }
            }
            else
            {
                j.jsonObject.Add(key, value);
            }
            return j;
        }

        /// <summary>
        /// Internal method to combine values when key already exists on the JsonObject.
        /// </summary>
        /// <param name="j">JsonObject</param>
        /// <param name="key">Key to combine</param>
        /// <param name="value">New value</param>
        internal static void CombineDuplicates(JsonObject j, string key, object value)
        {
            object existingValue = j.jsonObject[key];
            if (!IsSameValue(existingValue, value))
            {
                if (existingValue.GetType() == typeof(JsonArray))
                {
                    JsonArray newJArray = existingValue as JsonArray;
                    if (value.GetType() == typeof(JsonArray))
                    {
                        JsonArray v = value as JsonArray;
                        v.jsonArray.ForEach(x => newJArray.jsonArray.Add(x));
                    }
                    else
                    {
                        newJArray.jsonArray.Add(value);
                    }
                    existingValue = newJArray;
                }
                else if(existingValue.GetType() == typeof(String) && value.GetType() == typeof(string))
                {
                    List<object> toAdd = new List<object>();
                    if ((string)existingValue == "null" || (string)existingValue == "")
                    {
                        j.jsonObject[key] = value;
                    }
                    else if ((string)value == "null" || (string)value == "")
                    {
                        j.jsonObject[key] = existingValue;

                    }
                    else
                    {
                        JsonArray newJArray = new JsonArray(new List<object>() { j.jsonObject[key], value });
                        j.jsonObject[key] = newJArray;
                    }

                }
                else
                {
                    JsonArray newJArray = new JsonArray(new List<object>() { j.jsonObject[key], value });
                    j.jsonObject[key] = newJArray;
                }

            }
            else
            {
                j.jsonObject.Add(key, value);
            }

        }


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
        public static JsonObject ByKeysAndValues([ArbitraryDimensionArrayImport]List<string> keys, List<dynamic> values, [DefaultArgument("true")] bool nesting, [DefaultArgument("JsonOption.None")] JsonOption jsonOption)
        {
            IsValidData(values, keys);
            return new JsonObject(keys, values, nesting, jsonOption);

        }

        /// <summary>
        /// Returns the value associated with the given key from the jsonObject.
        /// </summary>
        /// <param name="jsonObject">JsonObject to get values from</param>
        /// <param name="key">Attribute's key</param>
        /// <returns name="value">Value returned</returns>
        /// <search>
        /// json, jsonobject, search, bykey
        /// </search> 
        internal static object GetValueByKey(JsonObject jsonObject, string key)
        {
            string[] keys = key.Split('.');
            IsValidKey(key);

            if (keys.Length <= 1)
            {
                string k = keys[0];
                return GetValues(jsonObject, k).First();
            }
            else
            {
                string k = keys.First();
                JsonObject j = jsonObject.jsonObject[k] as JsonObject;
                string[] nestedKeys = keys.Skip(1).ToArray();
                return GetValueByKey(j, String.Join(".", nestedKeys));
            }
        }



        /// <summary>
        /// Adds new attribute to the JsonObject. If given key already on the object and update set to
        /// True, value associated with the key will be updated. An error will be thrown otherwise.
        /// </summary>
        /// <param name="key">Key to add or update</param>
        /// <param name="value">Value to add or update</param> 
        /// <param name="nesting">Apply nesting behaviour if key input is a single string concatenated by 
        /// dots, representing the desired nested structure</param>
        /// <param name="jsonOption">Handling options where duplicate keys are found. Use JsonOptions dropdown node to select appropiate behaviour</param>
        /// <returns name="jsonObject">jsonObject with added attribute/s</returns>
        /// <search>
        /// json, jsonobject, add
        /// </search> 
        public JsonObject Add(string[] key, dynamic[] value, [DefaultArgument("true")] bool nesting, [DefaultArgument("JsonOption.None")] JsonOption jsonOption)
        {
            JsonObject newJson = new JsonObject(Keys, Values, nesting, jsonOption);
            for (var i = 0; i < key.Length; i++)
            {
                string k = key[i];
                dynamic v = value[i];
                newJson = JsonObjectRecursive(newJson, k, v, jsonOption);
            }

            return newJson;
        }

        /// <summary>
        /// Remove keys from the given JsonObject. If any key doesn't exit on the object
        /// or duplicated keys found on the input, error will be thrown.
        /// </summary>
        /// <param name="key">Key or list of keys to remove</param>
        /// <returns name="jsonObject">JsonObject with keys removed</returns>
        /// <param name="nesting">Apply nesting behaviour if key input is a single string concatenated by 
        /// dots, representing the desired nested structure</param>
        /// <search>
        /// json, jsonobject, remove
        /// </search> 
        public JsonObject Remove(string[] key, bool nesting = true)
        {
            List<string> newKeys = new List<string>(Keys);
            List<dynamic> newValues = new List<dynamic>(Values);

            foreach (string k in key)
            {
                try
                {
                    string[] keySplit = (nesting) ? k.Split('.') : new string[] { k };
                    if (keySplit.Length <= 1)
                    {
                        int index = newKeys.FindIndex(x => x == keySplit.First());
                        newKeys.RemoveAt(index);
                        newValues.RemoveAt(index);
                    }
                    else
                    {
                        string kFirst = keySplit.First();
                        int index = newKeys.FindIndex(x => x == keySplit.First());
                        JsonObject j = newValues[index] as JsonObject;
                        string[] nestedKeys = keySplit.Skip(1).ToArray();
                        string[] nestedK = new string[] { String.Join(".", nestedKeys) };
                        JsonObject newValue = j.Remove(nestedK);
                        if(newValue.Size > 0)
                        {
                            newValues[index] = newValue;
                        }else
                        {
                            newKeys.RemoveAt(index);
                            newValues.RemoveAt(index);
                        }
                    }
                }
                catch (ArgumentNullException)
                {
                    throw new KeyNotFoundException("Key not found on JsonObject or duplicated on input keys.");
                }
            
            }

            return new JsonObject(newKeys, newValues, true, JsonOption.None);

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
        public JsonObject Merge(List<JsonObject> others, [DefaultArgument("JsonOption.None")] JsonOption jsonOption)
        {
            Dictionary<object, dynamic> dict = new Dictionary<object, dynamic>();
            List<string> keys = new List<string>();
            List<object> values = new List<object>();
            foreach (KeyValuePair<string, object> x in jsonObject.ToList())
            {
                keys.Add(x.Key);
                values.Add(x.Value);
            }
            foreach (var other in others)
            {
                foreach (KeyValuePair<string, object> y in other.jsonObject.ToList())
                {
                    keys.Add(y.Key);
                    values.Add(y.Value);
                }
            }

            return new JsonObject(keys, values, true, jsonOption);
        }

        /// <summary>
        /// Returns the value associated with the given key from the jsonObject.
        /// </summary>
        /// <param name="key">Attribute's key</param>
        /// <param name="nesting">Apply nesting behaviour if key input is a single string concatenated by 
        /// dots, representing the desired nested structure</param>
        /// <returns name="value">Value returned</returns>
        /// <search>
        /// json, jsonobject, search, bykey
        /// </search> 
        public object GetValueByKey(string key, bool nesting = true)
        {
            string[] keys = (nesting) ? key.Split('.'): new string[] { key };
            IsValidKey(key);

            try
            {
                if (keys.Length <= 1)
                {
                    string k = keys[0];
                    return GetValues(this, k).First();
                }
                else
                {
                    string k = keys[0];
                    JsonObject j = JsonNet.ReturnValidObject(this.jsonObject[k]) as JsonObject;
                    string[] nestedKeys = keys.Skip(1).ToArray();
                    return GetValueByKey(j, String.Join(".", nestedKeys));
                }
            }
            catch (Exception )
            {
                throw new KeyNotFoundException("Key not found on JsonObject.");
            }
        }

        //TODO: Handling of nesting IMPORTANT
        /// <summary>
        /// Sort the JsonObject alphabetically by its keys.
        /// </summary>
        /// <returns name="sortedJsonObject">Sorted JsonObject</returns>
        public JsonObject SortKeys()
        {
            var sorted = jsonObject
                .OrderBy(j => j.Key);
            List<string> keys = sorted.Select(item => item.Key).ToList();
            List<object> values = sorted.Select(item => item.Value).ToList();
            return new JsonObject(keys, values, false, JsonOption.None);
        }

        //TODO: Handling of nesting IMPORTANT
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
                        .Select((json) => new KeyValuePair<JsonObject, object>(json, json.GetValueByKey(key)))
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
        public static Dictionary<string, List<JsonObject>> FilterByKeyAndValue(JsonObject[] jsonObjects, string key, dynamic value)
        {
            List<JsonObject> inJson = new List<JsonObject>();
            List<JsonObject> outJson = new List<JsonObject>();

            IsValidKey(key);

            foreach (var js in jsonObjects)
            {
                bool valid = false;

                IsValidValueAndException(value);
                dynamic v;
                try
                {
                    v = js.GetValueByKey(key);
                }
                catch
                {
                    v = null;
                }

                Type valueType = value.GetType();
                if (v == null)
                {
                    valid = false;
                }
                else if (valueType == typeof(string) || valueType == typeof(char))
                {
                    if (v.ToLower().Contains(value.ToLower()))
                    {
                        valid = true;
                    }
                }
                else
                {
                    if (v == value)
                    {
                        valid = true;
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
        /// Serialize jsonObject to String
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return JsonConvert.SerializeObject(this, Formatting.Indented);
        }

    }

}
