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
        /// <returns name="type">Type of the returned object</returns>
        /// <search>
        /// json, parse, jsonstring
        /// </search>
        [MultiReturn(new[] { "object", "type" })]
        public static Dictionary<string, object> JsonString(string jsonString)
        {

            if (jsonString == null) { throw new ArgumentNullException("jsonString"); }

            JToken parsed = JToken.Parse(jsonString);

            var obj = JsonNet.ReturnValidObject(parsed);

            return new Dictionary<string, object>()
                {
                    {"object", obj },
                    {"type", obj.GetType().ToString() }
                };

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
        [MultiReturn(new[] { "object", "type" })]
        public static Dictionary<string, object> XMLString(string xmlString)
        {
            if (xmlString == null) { throw new ArgumentNullException("xmlString"); }

            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(xmlString);
            string jsonString = JsonConvert.SerializeXmlNode(xmlDoc);
            return JsonString(jsonString);
            
        }


        /// <summary>
        /// Converts a JsonObject to XML string fromat.
        /// </summary>
        /// <param name="jsonObject">JsonObject</param>
        /// <param name="root">Value to wrap XML in case is necesary. 
        /// More info https://www.newtonsoft.com/json/help/html/ConvertingJSONandXML.htm#! </param>
        /// <returns name="xmlString">XML formatted string converted from JsonObject</returns>
        /// <search>
        /// json, convert, xml
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

    }
}
