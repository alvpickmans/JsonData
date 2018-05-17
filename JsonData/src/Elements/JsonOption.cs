using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.DesignScript.Runtime;
using Newtonsoft.Json;

namespace JsonData.Elements
{

    /// <summary>
    /// Options for JsonObjects
    /// </summary>
    [IsVisibleInDynamoLibrary(false)]
    public enum JsonOption
    {
        /// <summary>
        /// No action
        /// </summary>
        None,
        /// <summary>
        /// Update if value present in object
        /// </summary>
        Update,
        /// <summary>
        /// Combine Values
        /// </summary>
        Combine
    }

    /// <summary>
    /// Static Class wrapping method to return JsonOption by its name
    /// </summary>
    [IsVisibleInDynamoLibrary(false)]
    public static class JsonOptions
    {
        /// <summary>
        /// Static Method to return a JsonOption by its name.
        /// </summary>
        [IsVisibleInDynamoLibrary(false)]
        public static JsonOption ReturnOptionByName(string name)
        {
            return (JsonOption)Enum.Parse(typeof(JsonOption), name);
        }
    }
}
