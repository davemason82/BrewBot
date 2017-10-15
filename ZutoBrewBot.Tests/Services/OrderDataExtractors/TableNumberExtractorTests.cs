using System;
using System.Collections.Generic;
using System.Text;
using ZutoBrewBot.Services.OrderDataExtractors;
using NUnit.Framework;
using ZutoBrewBot.Models.Responses;

namespace ZutoBrewBot.Tests.Services
{
    [TestFixture]
    public class TableNumberExtractorTests
    {
        private TableNumberExtractor _sut;

        [SetUp]
        public void SetUp()
        {
            _sut = new TableNumberExtractor();
        }

        [Test]
        public void GetTableNumber_NoTableText_IncorrectResponse()
        {
            var testString = "some random text";

            var expected = GetResponse(testString, false);
            var actual = _sut.GetTableNumber(testString);

            Assert.That(expected, Is.EqualTo(actual));
        }

        [Test]
        public void GetTableNumber_TableTextButNoNumber_IncorrectResponse()
        {
            var testString = "coffee to my table please";

            var expected = GetResponse("table please", false);
            var actual = _sut.GetTableNumber(testString);

            Assert.That(expected, Is.EqualTo(actual));
        }

        [Test]
        public void GetTableNumber_TableSpaceNumber_CorrectResponse()
        {
            var testString = "one coffee to table 5 please";

            var expected = GetResponse("table 5");
            var actual = _sut.GetTableNumber(testString);

            Assert.That(expected, Is.EqualTo(actual));
        }

        [Test]
        public void GetTableNumber_TableNumber_CorrectResponse()
        {
            var testString = "one coffee to table5 please";

            var expected = GetResponse("table5 please");
            var actual = _sut.GetTableNumber(testString);

            Assert.That(expected, Is.EqualTo(actual));
        }

        [Test]
        public void GetTableNumber_TableSpaceSpaceNumber_CorrectResponse()
        {
            var testString = "one coffee to table  5 please";

            var expected = GetResponse("table 5");
            var actual = _sut.GetTableNumber(testString);

            Assert.That(expected, Is.EqualTo(actual));
        }

        [Test]
        public void GetTableNumber_TableNumberText_CorrectResponse()
        {
            var testString = "one coffee to table 5please";

            var expected = GetResponse("table 5please");
            var actual = _sut.GetTableNumber(testString);

            Assert.That(expected, Is.EqualTo(actual));
        }

        [Test]
        public void GetTableNumber_SingleWord_IncorrectResponse()
        {
            var testString = "table";

            var expected = GetResponse("table", false);
            var actual = _sut.GetTableNumber(testString);

            Assert.That(expected, Is.EqualTo(actual));
        }

        [Test]
        public void GetTableNumber_TableIsLastWord_IncorrectResponse()
        {
            var testString = "Two coffees to my table";

            var expected = GetResponse("table", false);
            var actual = _sut.GetTableNumber(testString);

            Assert.That(expected, Is.EqualTo(actual));
        }

        private OrderTextTableExtractorResponse GetResponse(string tableText, bool tableFound = true)
        {
            return new OrderTextTableExtractorResponse
            {
                TableFound = tableFound,
                TableNumber = tableFound ? 5 : 0,
                TableText = tableText
            };
        }
    }
}
