#region namesapces
using Autodesk.DesignScript.Runtime;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Schema;
using System.Xml.XPath;
using JsonData;
//using JsonElements;
#endregion

namespace JsonData.Utilities
{
    /// <summary>
    /// Class to handle parsing and read/write from and to json format strings or files.
    /// </summary>
    public static class Write
    {
        
        /// <summary>
        /// Writes the JsonObject or JsonArray to a json file.
        /// </summary>
        /// <param name="json">JsonObject or JsonArray element</param>
        /// <param name="filepath">File path for json file</param>
        /// <returns name="filepath">Returns the filepath if write operation is succesful</returns>
        /// <search>
        /// json, parser, to file, jsonfile
        /// </search>
        public static string ToJsonFile(JsonNet json, string filepath)
        {
            if(json == null) { throw new ArgumentNullException("json"); }
            if (filepath == null) { throw new ArgumentNullException("filepath"); }

            string ext = Path.GetExtension(filepath);
            string validExt = "json";

            if (ext.ToLower().Contains(validExt))
            {
                File.WriteAllText(filepath, json.ToString());
                return filepath;
            }
            else
            {
                throw new Exception(String.Format("File extension is not of {0} type. Please select a valid {0} file.", validExt));
            }
        }

        /// <summary>
        /// Writes the JsonObject or JsonArray to a XML file.
        /// </summary>
        /// <param name="jsonObject">JsonObject or JsonArray element</param>
        /// <param name="filepath">File path for XML file</param>
        /// <param name="root">Value to wrap XML in case is necesary.
        /// More info https://www.newtonsoft.com/json/help/html/ConvertingJSONandXML.htm#! </param>
        public static void ToXMLFile(Elements.JsonObject jsonObject, string filepath, string root= "")
        {
           
            try
            {
                string xml = Parse.JsonToXML(jsonObject, root);
                File.WriteAllText(filepath, xml.ToString());
            }
            catch(Exception e )
            {
                throw new Exception(e.Message);
            }
        }

    }
}
