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
    [IsVisibleInDynamoLibrary(false)]
    public enum JsonOption
    {
        /// <summary>
        /// You are reading this because of a known issue on Dynamo 2.0. These options are implemented 
        /// and embeded on nodes with dropdown selector. They will automatically hide once bug is resolve on next version.
        /// No action.
        /// </summary>
        None,
        /// <summary>
        /// You are reading this because of a known issue on Dynamo 2.0. These options are implemented 
        /// and embeded on nodes with dropdown selector. They will automatically hide once bug is resolve on next version.
        /// If object contains the key, update its value.
        /// </summary>
        Update,
        /// <summary>
        /// You are reading this because of a known issue on Dynamo 2.0. These options are implemented 
        /// and embeded on nodes with dropdown selector. They will automatically hide once bug is resolve on next version.
        /// If object contains the key, combine the values.
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
