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

}
