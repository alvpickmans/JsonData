#region namesapces
using Autodesk.DesignScript.Runtime;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using Microsoft.VisualBasic.FileIO;
using System.Linq;
using System.Xml.Schema;
using System.Xml.XPath;
using JsonData;
#endregion

namespace JsonData.Utilities
{
    /// <summary>
    /// Class to handle parsing strings to Json format.
    /// </summary>
    public static class Parse
    {

        /// <summary>
        /// Parses a json formated string. It will return JsonObject,
        /// JsonArray or other match that the parser can do from the input.
        /// Error will be thrown if parser fails.
        /// </summary>
        /// <param name="jsonString">String with a json format</param>
        /// <returns name="object">Object returned by the parser</returns>
        /// <search>
        /// json, parse, jsonstring
        /// </search>
        public static object JsonString(string jsonString)
        {
            if (jsonString == null) { throw new ArgumentNullException("jsonString"); }

            JToken parsed = JToken.Parse(jsonString);
            return JsonNet.ReturnValidObject(parsed);
        }


        /// <summary>
        /// Parses a xml formated string. It will return JsonObject,
        /// JsonArray or other match that the parser can do from the input.
        /// Error will be thrown if parser fails.
        /// </summary>
        /// <param name="xmlString">XML formatted string</param>
        /// <returns name="object">Object returned by the parser</returns>
        /// <returns name="type">Type of the returned object</returns>
        /// <search>
        /// json, parse, xml
        /// </search>
        public static object XMLString(string xmlString)
        {
            if (xmlString == null) { throw new ArgumentNullException("xmlString"); }

            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(xmlString);
            string jsonString = JsonConvert.SerializeXmlNode(xmlDoc);
            return JsonString(jsonString);
            
        }

        /// <summary>
        /// Parses a CSV formated string. It will return a list of JsonObjects
        /// Error will be thrown if parser fails.
        /// </summary>
        /// <param name="csvString">CSV formatted string</param>
        /// <returns name="jsonObjects">List of JsonObjets returned by the parser</returns>
        /// <search>
        /// json, parse, csv
        /// </search>
        public static List<Elements.JsonObject> CSVString(string csvString)
        {
            try
            {   using (StringReader sr = new StringReader(csvString))
                using (TextFieldParser parser = new TextFieldParser(sr))
                {
                    parser.TextFieldType = FieldType.Delimited;
                    parser.SetDelimiters(",");
                    List<Elements.JsonObject> jsonObjects = new List<Elements.JsonObject>();
                    string[] headers = null;
                    while (!parser.EndOfData)
                    {
                        dynamic[] fields = parser.ReadFields();
                        if(headers == null)
                        {
                            headers = fields as string[];
                        }
                        else
                        {
                            Elements.JsonObject jObject = Elements.JsonObject.ByKeysAndValues(headers.ToList(), fields.ToList(), false, JsonOption.None);
                            jsonObjects.Add(jObject);
                        }
                    }

                    return jsonObjects;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }


        /// <summary>
        /// Converts a JsonObject to XML string format.
        /// </summary>
        /// <param name="jsonObject">JsonObject</param>
        /// <param name="root">Value to wrap XML in case is necesary. 
        /// More info https://www.newtonsoft.com/json/help/html/ConvertingJSONandXML.htm#! </param>
        /// <returns name="xmlString">XML formatted string converted from JsonObject</returns>
        /// <search>
        /// json, convert, xml, parser
        /// </search>
        public static string JsonToXML(Elements.JsonObject jsonObject , string root = "")
        {
            try
            {
                XmlDocument doc = (XmlDocument)JsonConvert.DeserializeXmlNode(jsonObject.ToString(), root);
                StringWriter sw = new StringWriter();
                XmlTextWriter xw = new XmlTextWriter(sw);
                xw.Formatting = System.Xml.Formatting.Indented;
                doc.WriteTo(xw);
                return sw.ToString();
            }
            catch (Exception e)
            {
                string exceptionMsg = e.Message;
                string msg1 = "JSON root object has multiple properties";
                string msg2 = "Root containig more than one property";
                string msg3 = "DocumentElement";
                if (exceptionMsg.Contains(msg1) || exceptionMsg.Contains(msg2) || exceptionMsg.Contains(msg3))
                {
                    throw new Exception("Root containig more than one property. Try wrapping the input on a JsonObject element or adding a value on the root input parameter");
                }
                else
                {
                    throw new Exception(e.Message);
                }
            }
        }

        /// <summary>
        /// Converts a list of JsonObject to CSV string format. JsonObjects must have one level only
        /// (no other JsonObject or JsonArray as values), being the keys the header of the CSV string.
        /// </summary>
        /// <param name="jsonObjects">List of JsonObjects with same keys</param>
        /// <returns name="csvString">CSV formatted string converted from a list of JsonObjects</returns>
        /// <search>
        /// json, convert, csv, parse
        /// </search>
        public static string JsonToCSV(List<Elements.JsonObject> jsonObjects)
        {
            try
            {
                List<string> headers = null;
                string csvString = "";
                foreach(Elements.JsonObject jObject in jsonObjects)
                {
                    if(headers == null)
                    {
                        headers = jObject.Keys;
                        csvString += string.Join(", ", headers);
                    }
                    else if(!headers.SequenceEqual(jObject.Keys))
                    {
                        throw new Exception("Not every JsonObject has the same keys. Please ammend to generate a valid CSV string.");
                    }
                    
                    csvString += Environment.NewLine + string.Join(", ", jObject.Values.Select(value => value.ToString()));

                }
                return csvString;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

    }
}
