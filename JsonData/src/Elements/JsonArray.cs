#region namesapces
using Autodesk.DesignScript.Runtime;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
#endregion

namespace JsonData.Elements
{
    /// <summary>
    /// Class for handle JArray objects.
    /// </summary>
    [JsonConverter(typeof(JsonArrayConverter))]
    public class JsonArray : JsonNet
    {
        #region Variables
        internal List<object> jsonArray = new List<object>();

        /// <summary>
        /// Returns elements in the JsonArray object.
        /// </summary>
        /// <returns name="elements">Elements in JsonArray</returns>
        /// <search>
        /// json, jsonarray, elements, items
        /// </search>
        public List<object> Elements => jsonArray;

        /// <summary>
        /// Returns the number of elements in the JsonArray object.
        /// </summary>
        /// <returns name="size">i Number of elements in JsonArray</returns>
        /// <search>
        /// json, jsonarray, size
        /// </search>
        public int Size => jsonArray.Count;

        #endregion

        #region Constructors
        /// <summary>
        /// JsonArray constructor by a given list of elements.
        /// </summary>
        /// <param name="items">Valid list of items</param>
        internal JsonArray(List<object> items)
        {
            foreach (var item in items)
            {
                jsonArray.Add(item);
            }
        }

        /// <summary>
        /// JsonArray constructor by a given JArray type of object.
        /// </summary>
        /// <param name="jArray">JArray object</param>
        internal JsonArray(JArray jArray)
        {
            foreach(JToken tk in jArray.Children())
            {
                object o = ReturnValidObject(tk);
                jsonArray.Add(o);
            }
        }

        #endregion

        #region Methods
        /// <summary>
        /// JsonArray constructor by a given set of elements.
        /// </summary>
        /// <param name="elements">Elements to create the New JsonArray </param>
        /// <returns name="jsonArray">New JsonArray</returns>
        /// <search>
        /// json, jsonarray, create
        /// </search>
        public static JsonArray ByElements([ArbitraryDimensionArrayImport] List<object> elements)
        {
            return new JsonArray(elements);
        }


        /// <summary>
        /// Serialize JsonArray to String
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return JsonConvert.SerializeObject(this, Formatting.Indented);
        } 
        #endregion

    }


}
