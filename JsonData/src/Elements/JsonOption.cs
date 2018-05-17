using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.DesignScript.Runtime;
using Newtonsoft.Json;
using Dynamo.Graph.Nodes;

namespace JsonData.Elements
{

    /// <summary>
    /// Options for JsonObjects
    /// </summary>
    //[SupressImportIntoVM]
    public enum JsonOption
    {
        /// <summary>
        /// No action.
        /// </summary>
        [SupressImportIntoVM]
        None,
        /// <summary>
        /// If object contains the key, update its value.
        /// </summary>
        [SupressImportIntoVM]
        Update,
        /// <summary>
        /// If object contains the key, combine the values.
        /// </summary>
        [SupressImportIntoVM]
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
