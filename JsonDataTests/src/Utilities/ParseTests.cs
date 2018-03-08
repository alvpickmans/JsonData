using JsonData.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

namespace JsonData.Utilities.Tests
{
    [TestFixture]
    public class ParseTests
    {
        [Test]
        [Category("UnitTests")]
        public void JsonStringTest()
        {
            Assert.IsInstanceOf(typeof(Int64), Parse.JsonString("2"));
            Assert.IsInstanceOf(typeof(Elements.JsonObject), Parse.JsonString("{'one': 1}"));
            Assert.IsInstanceOf(typeof(List<object>), Parse.JsonString("[{'one': 1}, {'two': 2}]"));
        }

        [Test]
        [Category("UnitTests")]
        public void CVSStringTest()
        {
            string csv = @"
            id, name, surname
            01, john, doe
            02, jane, doe";
            var parsed = Parse.CSVString(csv);
            Assert.IsInstanceOf(typeof(List<Elements.JsonObject>), parsed);
            Assert.AreEqual(2, parsed.Count);
            Assert.Contains("john", parsed.First().Values);

        }

    }
}