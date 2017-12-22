#region namesapces
using JsonData.Elements;
using Newtonsoft.Json.Linq;
using System;
using System.Diagnostics;
using System.Collections.Generic;
#endregion

namespace JsonData
{
    /// <summary>
    /// Parent class for json elements.
    /// </summary>
    public abstract class JsonNet
    {
        #region Variables
        /// <summary>
        /// Type of the Json Element.
        /// </summary>
        internal Type type { private get; set; }


        internal static IList<Type> keyValidTypes = new List<Type>
        {
            typeof(string),
            typeof(IEnumerable<object>)
        };

        //internal static IList<Type> valueValidTypes = new List<Type>
        //{
        //    typeof(string),
        //    typeof(int),
        //    typeof(double),
        //    typeof(bool),
        //    typeof(char),
        //    typeof(JsonObject),
        //    typeof(JsonArray),
        //    typeof(Dictionary<object,dynamic>),
        //    typeof(System.Collections.ArrayList)
        //}; 
        #endregion

        #region Methods

        /// <summary>
        /// Constructor of the class.
        /// </summary>
        /// <param name="t">Element's type</param>
        internal void InitJsonNet(Type t)
        {
            type = t;
        }

        /// <summary>
        /// Overwrites the inherited GetType() method from the object type.
        /// Returns json element´s type.
        /// </summary>
        /// <returns name="type">Element´s type</returns>
        new internal Type GetType()
        {
            return type;
        }

        /// <summary>
        /// Returns a valid package json element (JsonObject or JsonArray) from the input object.
        /// </summary>
        /// <param name="o">Input object</param>
        /// <returns name="object">JsonObject, JsonArray or same input object if not valid.</returns>
        internal static object ReturnValidObject(object o)
        {
            try
            {
                Debug.WriteLine(o.GetType());
                if (o.GetType() == typeof(JObject))
                {
                    return JsonObject.ByJObject(o as JObject);
                }
                else if (o.GetType() == typeof(JArray))
                {
                    return new JsonArray(o as JArray);
                }
                else
                {
                    return o;
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        /// <summary>
        /// Returns values/elements from the Json element.
        /// </summary>
        /// <param name="jObject">JsonObject to get values from.</param>
        /// <param name="key">Optional. If json element is of type JsonObject and key is given, item matching 
        /// key will be returned or null if not found
        /// </param>
        /// <returns name="values">List of values or elements</returns>
        internal static List<object> GetValues(JsonObject jObject, string key = null)
        {
            List<object> values = new List<object>();
            List<object> temp = new List<object>();
            Type type = jObject.GetType();
            try
            {
                if (key == null)
                {
                    foreach (object el in jObject.jsonObject.Values)
                    {
                        values.Add(ReturnValidObject(el));
                    }
                }
                else
                {
                    values.Add(ReturnValidObject(jObject.jsonObject[key]));
                }
                
                return values;

            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }

        }

        /// <summary>
        /// Returns values/elements from the Json element.
        /// </summary>
        /// <returns name="values">List of values or elements</returns>
        internal static List<object> GetValues(JsonArray jArray)
        {
            List<object> values = new List<object>();
            List<object> temp = new List<object>();
            Type type = jArray.GetType();
            try
            {
                foreach (object el in jArray.jsonArray)
                {
                    values.Add(ReturnValidObject(el));
                }
                

                return values;

            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }

        }

        internal static bool IsSameValue(object value, object other)
        {
            bool isSame = true;
            Type valueType = value.GetType();
            Type otherType = other.GetType();
            if(valueType == typeof(JsonArray) && otherType == typeof(JsonArray))
            {
                JsonArray v = value as JsonArray;
                JsonArray o = other as JsonArray;
                if(v.Size == o.Size)
                {
                    for(var i = 0; i < v.Size; i++)
                    {
                        if(!IsSameValue(v.jsonArray[i], o.jsonArray[i]))
                        {
                            return false;
                        }
                    }
                }
            }else if (valueType == typeof(JsonObject) && otherType == typeof(JsonObject))
            {
                return false;
            }
            else if(value != other)
            {
                return false;
            }

            return isSame;
        }
        

        /// <summary>
        /// Returns true if key is of valid type, throws exception otherwise.
        /// </summary>
        /// <param name="k">Object to check.</param>
        /// <returns name="valid">True if valid key.</returns>
        internal static bool IsValidKey(string k)
        {            
            if (!keyValidTypes.Contains(k.GetType()))
            {
                throw new ArgumentException("At least one key is of invalid type " + k.GetType() + ". Only string type keys supported.");
            }
            else
            {
                return true;
            }
        }

        /// <summary>
        /// Returns true if value is of valid type, false otherwise.
        /// </summary>
        /// <param name="v">Object to check.</param>
        /// <returns name="valid">Boolean true if valid, false otherwise.</returns>
        internal static bool IsValidValue(object v = null)
        {
            if (v == null)
            {
                throw new Exception("One or more values are null. Please replace all null values.");
            }
            string typeNamespace = v.GetType().Namespace;
            bool systemNS = typeNamespace.StartsWith("System");
            bool autodeskNS = typeNamespace.StartsWith("Autodesk");
            bool revitNS = typeNamespace.StartsWith("Revit");
            bool jsonNS = typeNamespace.StartsWith("Json");
            bool newtonsoftNS = typeNamespace.StartsWith("Newtonsoft");
            bool dsCore = typeNamespace.StartsWith("DSCore");
            if (systemNS || autodeskNS || revitNS || jsonNS || newtonsoftNS || dsCore || v == null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Returns true if value is of valid type, throws exception otherwise
        /// </summary>
        /// <param name="v">Object to check.</param>
        /// <returns name="valid">True if valid key.</returns>
        internal static bool IsValidValueAndException(object v)
        {
            if (v == null)
            {
                throw new Exception("One or more values are null. Please replace all null values.");
            }

            if (IsValidValue(v))
            {
                return true;
            }
            else
            {
                throw new ArgumentException(String.Format("Values of type {0} not supported.", v.GetType()));
            }

        }

        /// <summary>
        /// Checks if keys and values are of valid types, and if them amounts match.
        /// True if valid, throws exception otherwise.
        /// </summary>
        /// <param name="values">Objects to check as values</param>
        /// <param name="keys">Objects to check as keys</param>
        /// <returns name="valid">True if keys and values are valid.</returns>
        internal static bool IsValidData(List<dynamic> values = null, List<string> keys = null)
        {
            if (values == null)
            {
                throw new Exception("One or more values are null. Please replace all null values.");
            }

            try
            {
                foreach (var k in keys)
                {
                    IsValidKey(k);
                }
                foreach (var v in values)
                {
                    IsValidValueAndException(v);
                }

                if (keys.Count != 0 && keys.Count != values.Count)
                {
                    throw new ArgumentException("Number of keys does not match number of values.\nKeys: " + keys.Count + "\nValues: " + values.Count);
                }

                return true;

            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }


        #endregion

    }


}
