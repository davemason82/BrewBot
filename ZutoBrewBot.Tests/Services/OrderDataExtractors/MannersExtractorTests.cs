using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using ZutoBrewBot.Services.OrderDataExtractors;
using ZutoBrewBot.Models.Responses;

namespace ZutoBrewBot.Tests.Services.OrderDataExtractors
{
    public class MannersExtractorTests
    {
        private MannersExtractor _sut;

        [SetUp]
        public void SetUp()
        {
            _sut = new MannersExtractor();
        }

        [TestCase("Two cappucino's to table 48 please", "please")]
        [TestCase("Two cappucino's to table 48 thanks", "thanks")]
        [TestCase("Two cappucino's to table 48 cheers", "cheers")]
        [TestCase("Please could we get two cappucino's to table 48?", "please")]
        public void GetMannersResponse_MannersUsed_TrueResponse(string orderText, string mannersText)
        {
            var expected = GetResponse(mannersText);
            var actual = _sut.GetMannersResponse(orderText);

            Assert.That(expected, Is.EqualTo(actual));
        }

        [Test]
        public void GetMannersResponse_MannersNotUsed_FalseResponse()
        {
            var orderText = "Two cappucino's to table 5";
            var expected = GetResponse(string.Empty, false);
            var actual = _sut.GetMannersResponse(orderText);

            Assert.That(expected, Is.EqualTo(actual));
        }

        private MannersExtractorResponse GetResponse(string mannersText, bool mannersFound = true)
        {
            return new MannersExtractorResponse
            {
                MannersFound = mannersFound,
                MannersText = mannersText
            };
        }
    }
}
