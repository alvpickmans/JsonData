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
using Microsoft.VisualBasic.FileIO;
using JsonData;
//using JsonElements;
#endregion

namespace JsonData.Utilities
{
    /// <summary>
    /// Class to handle parsing and read/write from and to json format strings or files.
    /// </summary>
    public static class Read
    {
        /// <summary>
        /// Reads and parses a json file. It will return JsonObject, 
        /// JsonArray or other match that the parser can do from the input.
        /// Error will be thrown if parser fails.
        /// </summary>
        /// <param name="filepath">Filepath of the json file</param>
        /// <returns name="object">Object returned by the parser.</returns>
        /// <returns name="type">Type of the returned object.</returns>
        /// <search>
        /// json, parser, from file, jsonfile
        /// </search>
        [MultiReturn(new[] { "object", "type" })]
        public static Dictionary<string,object> FromJsonFile(string filepath)
        {
            string ext = Path.GetExtension(filepath);
            string validExt = "json";

            if(ext.ToLower().Contains(validExt))
            {
                return Parse.JsonString(File.ReadAllText(filepath));
            }
            else
            {
                throw new Exception(String.Format("File extension is not of {0} type. Please select a valid {0} file.", validExt));
            }

        }

        /// <summary>
        /// Reads and parses a XML file. It will return JsonObject, JsonArray or other match that the parser can do from the input.
        /// Error will be thrown if parser fails.
        /// </summary>
        /// <param name="filepath">Filepath of the XML file</param>
        /// <returns name="object">Object returned by the parser.</returns>
        /// <returns name="type">Type of the returned object.</returns>
        /// <search>
        /// json, parser, from file, xml, xmlfile
        /// </search>
        [MultiReturn(new[] { "object", "type" })]
        public static Dictionary<string, object> FromXMLFile(string filepath)
        {
            try
            {
                return Parse.XMLString(File.ReadAllText(filepath));
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// Reads and parses a CSV formated file. It will return a list of JsonObjects
        /// Error will be thrown if parser fails.
        /// </summary>
        /// <param name="filepath">Filepath of the XML file</param>
        /// <returns name="jsonObjects[]">List of JsonObjects returned by the parser.</returns>
        /// <search>
        /// json, parser, from file, csv, csvfile
        /// </search>
        public static List<Elements.JsonObject> FromCSVFile(string filepath)
        {
            try
            {
                return Parse.CSVString(File.ReadAllText(filepath));
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
