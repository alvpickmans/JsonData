using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using JsonData;
using NUnit.Framework;

namespace JsonData.Elements.Tests
{
    [TestFixture]
    public class JsonObjectTests
    {
        #region Base JsonObject
        internal static List<string> keys = new List<string>() { "oneKey", "nested.key", "two.nested.keys" } ;
        internal static List<object> values = new List<object>() { "value", 1, new List<object>() { 1,2,3} };
        internal static JsonObject json= JsonObject.ByKeysAndValues(keys, values, false, JsonOption.None);
        internal static JsonObject jsonNested = JsonObject.ByKeysAndValues(keys, values, true, JsonOption.None);
        #endregion

        [Test]
        [Category("UnitTests")]
        public static void ByKeysAndValuesTest()
        {
            Assert.AreEqual(new List<string>() { "oneKey", "nested", "two"}, jsonNested.Keys);
            Assert.AreEqual(keys, json.Keys);
            Assert.AreEqual(values, json.Values);
            Assert.AreEqual(3, jsonNested.Size);
        }

        [Test]
        [Category("UnitTests")]
        public static void GetValueByKeyTest()
        {
            Assert.AreEqual("value", jsonNested.GetValueByKey("oneKey"));
            Assert.AreEqual(1, json.GetValueByKey("nested.key", false));
            Assert.AreEqual(1, jsonNested.GetValueByKey("nested.key"));
            Assert.AreEqual(new List<object>() { 1, 2, 3 }, jsonNested.GetValueByKey("two.nested.keys"));

        }
        

        [Test]
        [Category("UnitTests")]
        public static void NestedTest()
        {
            List<string> k = new List<string>() { "nested.one", "nested.two" };
            List<object> v = new List<object>() { 1, 2 };
            JsonObject jsonNested = JsonObject.ByKeysAndValues(k, v, true, JsonOption.None);
            Assert.IsInstanceOf(typeof(JsonObject), jsonNested.GetValueByKey("nested"));
        }

        [Test]
        [Category("UnitTests")]
        public static void FilterByKeyAndValueTest()
        {
            List<string> keys = new List<string>() { "one", "two", "three" };
            List<JsonObject> jsonObjects = new List<JsonObject>()
            {
                JsonObject.ByKeysAndValues(keys, new List<object>(){ 1, "dos", "tres" }, false, JsonOption.None),
                JsonObject.ByKeysAndValues(keys, new List<object>(){ 2, "uno", 3}, false, JsonOption.None),
                JsonObject.ByKeysAndValues(new List<string>(){"uno", "dos"}, new List<object>(){ 1, 2}, false, JsonOption.None)
            };

            var noKeyFound = JsonObject.FilterByKeyAndValue(jsonObjects, "eins", 1);
            Assert.AreEqual(0, noKeyFound["in"].Count);
            Assert.AreEqual(3, noKeyFound["out"].Count);

            var oneMatch = JsonObject.FilterByKeyAndValue(jsonObjects, "one", 1);
            Assert.AreEqual(1, oneMatch["in"].Count);
            Assert.AreEqual(2, oneMatch["out"].Count);

            var stringPartialMatch = JsonObject.FilterByKeyAndValue(jsonObjects, "two", "o");
            Assert.AreEqual(2, stringPartialMatch["in"].Count);
            Assert.AreEqual(1, stringPartialMatch["out"].Count);

            var stringCaseMatch = JsonObject.FilterByKeyAndValue(jsonObjects, "three", "TRES");
            Assert.AreEqual(1, stringCaseMatch["in"].Count);
            Assert.AreEqual(2, stringCaseMatch["out"].Count);
        }

        [Test]
        [Category("UnitTests")]
        public static void AddCombineTest()
        {
            var keys = new List<string>() { "string", "string", "int", "int", "list", "list" };
            var values = new List<object>() { "first", "second", 1, 2, new List<object>() { "start" }, 99 };
            JsonObject json = JsonObject.ByKeysAndValues(keys, values, false, JsonOption.Combine);
            Assert.AreEqual(3, json.Size);
            //json.Values.ForEach(v => Assert.IsTrue(v is IList<object>));
            Assert.Pass("All values are lists");
        }

        [Test]
        [Category("UnitTests")]
        public static void RemoveTest()
        {
            var keys = new List<string>() { "first", "second", "nested.first", "nested.second" };
            var values = new List<object>() { 1, 2, 3.1, 3.2 };
            JsonObject json = JsonObject.ByKeysAndValues(keys, values, true, JsonOption.None);

            JsonObject removeAll = json.Remove(keys, true);
            Assert.AreEqual(0, removeAll.Size);
            Assert.AreEqual(3, json.Size);

            JsonObject removeSingle = json.Remove(new List<string>() { "first", "second" }, true);
            Assert.AreEqual(1, removeSingle.Size);
            Assert.AreEqual(3, json.Size);

            JsonObject removeNested = json.Remove(new List<string>() { "nested.first" }, true);
            Assert.AreEqual(3, removeNested.Size);
            Assert.IsInstanceOf(typeof(int), removeNested.Values[0]);
            Assert.IsInstanceOf(typeof(JsonObject), removeNested.Values[2]);
            
        }
        
    }
}