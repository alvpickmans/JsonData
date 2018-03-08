#region namespace
using JsonData.Elements;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
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
            var name = value as JsonObject;
            writer.WriteStartObject();
            foreach (KeyValuePair<string, dynamic> item in name.dict)
            {
                writer.WritePropertyName(item.Key.ToString());
                string typeNameSpace = item.Value.GetType().Namespace;
                if (typeNameSpace.Contains("System") || typeNameSpace.Contains("Json") || typeNameSpace.Contains("Newtonsoft"))
                {
                    serializer.Serialize(writer, item.Value);
                }
                else
                {
                    serializer.Serialize(writer, item.Value.ToString());
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
