#region namesapces
using JsonData.Elements;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Linq;
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
        internal Type type { get; private set; }

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
        /// Same implementation as in:
        /// https://github.com/pboyer/Dynamo/blob/ac9178c5790e6704ce5f5a768ab3eaceb436782d/src/Libraries/CoreNodes/JSON.cs#L24
        /// </summary>
        /// <param name="token">Input object</param>
        /// <returns name="object">return object</returns>
        internal static object ReturnValidObject(JToken token)
        {
            switch (token.Type)
            {
                case JTokenType.Object:
                    return new JsonObject(token as JObject);
                case JTokenType.Array:
                    var arr = token as JArray;
                    return arr.Select(ReturnValidObject).ToList();
                case JTokenType.Null:
                    return null;
                case JTokenType.Integer:
                case JTokenType.Float:
                case JTokenType.String:
                case JTokenType.Boolean:
                case JTokenType.Date:
                case JTokenType.TimeSpan:
                    return (token as JValue).Value;
                case JTokenType.Guid:
                case JTokenType.Uri:
                    return (token as JValue).Value.ToString();
                default:
                    return null;
            }
        }

        #endregion

    }


}
