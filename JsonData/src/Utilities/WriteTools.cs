#region namesapces
using Autodesk.DesignScript.Runtime;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Collections.Generic;
using System.IO;
using System.Text;
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
        public static string ToJsonFile([ArbitraryDimensionArrayImport] List<JsonNet> json, string filepath)
        {
            if(json == null) { throw new ArgumentNullException("json"); }
            if (filepath == null) { throw new ArgumentNullException("filepath"); }

            string ext = Path.GetExtension(filepath);
            List<string> validExt = new List<string>() { ".json", ".dyn" };

            if (validExt.Contains(ext.ToLower()))
            {
                if(json.Count == 1)
                {
                    File.WriteAllText(filepath, json[0].ToString());
                }
                else
                {
                    File.WriteAllText(filepath, JsonConvert.SerializeObject(json, Formatting.Indented));
                }
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
        /// <returns name="filepath">Returns the filepath if write operation is succesful</returns>
        /// <search>
        /// json, parser, to file, xmlfile, xml
        /// </search>
        public static string ToXMLFile(Elements.JsonObject jsonObject, string filepath, string root= "")
        {
           
            try
            {
                string xml = Parse.JsonToXML(jsonObject, root);
                File.WriteAllText(filepath, xml.ToString());
                return filepath;
            }
            catch(Exception e )
            {
                throw new Exception(e.Message);
            }
        }

        /// <summary>
        /// Writes a list of JsonObject to a CSV file. JsonObjects must have one level only
        /// (no other JsonObject or JsonArray as values), being the keys the header of the CSV string.
        /// </summary>
        /// <param name="jsonObjects">List of JsonObjects</param>
        /// <param name="filepath">File path for CSV file</param>
        /// <returns name="filepath">Returns the filepath if write operation is succesful</returns>
        /// <search>
        /// json, parser, to file, csvfile, csv
        /// </search>
        public static string ToCSVFile(List<Elements.JsonObject> jsonObjects, string filepath)
        {
            try
            {
                if (jsonObjects == null) { throw new ArgumentNullException("dict"); }
                if (filepath == null) { throw new ArgumentNullException("filepath"); }

                string ext = Path.GetExtension(filepath);
                if (ext.ToLower().Contains("csv"))
                {
                    string csv = Parse.JsonToCSV(jsonObjects);
                    File.WriteAllText(filepath, csv);
                    return filepath;
                }
                else
                {
                    throw new Exception("Not proper CSV file selected. Make sure you are writing to a file with '.csv' extension.");
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

    }
}
