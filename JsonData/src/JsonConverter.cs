#region namespace
using JsonData.Elements;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using System.Linq;
using System;
using System.Collections.Generic;
#endregion

namespace JsonData
{
    internal class JsonObjectConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return typeof(JsonObject).IsAssignableFrom(objectType);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            JObject jsonObject = JObject.Load(reader);
            return new JsonObject(jsonObject);
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var json = value as JsonObject;
            writer.WriteStartObject();
            foreach (KeyValuePair<string, object> item in json.dict)
            {
                writer.WritePropertyName(item.Key);
                Type type = item.Value.GetType();
                var temp = item.Value as IEnumerable<object>;
                if (temp != null)
                {
                    var serializedList = new List<object>();
                    foreach (var element in temp)
                    {
                        try
                        {
                            JsonConvert.SerializeObject(element);
                            serializedList.Add(element);
                        }
                        catch (Exception)
                        {
                            serializedList.Add(element.ToString());
                        }
                    }
                    serializer.Serialize(writer, serializedList);
                }
                else
                {
                    try
                    {
                        serializer.Serialize(writer, item.Value);
                    }
                    catch (Exception)
                    {
                        serializer.Serialize(writer, item.Value.ToString(), typeof(string));
                    }
                }
                
            }
            writer.WriteEndObject();
        }
    }

    internal class JsonArrayConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return typeof(JsonArray).IsAssignableFrom(objectType);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            JArray jArray = JArray.Load(reader);
            return new JsonArray(jArray);
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var name = value as JsonArray;
            writer.WriteStartArray();
            foreach (dynamic item in name.jsonArray)
            {
                string typeNameSpace = item.GetType().Namespace;
                if (typeNameSpace.Contains("System") || typeNameSpace.Contains("Json") || typeNameSpace.Contains("Newtonsoft"))
                {
                    serializer.Serialize(writer, item);
                }
                else
                {
                    serializer.Serialize(writer, item.ToString());
                }
            }
            writer.WriteEndArray();
        }
    }
}
