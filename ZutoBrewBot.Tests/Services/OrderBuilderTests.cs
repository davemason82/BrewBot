using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using Moq;
using ZutoBrewBot.Services.Interfaces;
using ZutoBrewBot.Services;
using ZutoBrewBot.Models;
using ZutoBrewBot.Services.OrderDataExtractors;

namespace ZutoBrewBot.Tests.Services
{
    [TestFixture]
    public class OrderBuilderTests
    {
        private OrderBuilder _sut;
        private string _user;

        [SetUp]
        public void SetUp()
        {
            _user = "testuser";

            var tableNumberExtractor = new TableNumberExtractor();
            var mannersExtractor = new MannersExtractor();

            _sut = new OrderBuilder(tableNumberExtractor, mannersExtractor);
        }

        [Test]
        public void BuildOrder_NoTableName_ThrowsException()
        {
            var testString = "One cup of tea please";
            Assert.Throws<ArgumentException>(() => _sut.BuildOrder(testString, _user), "No table number");
        }

        [Test]
        public void BuildOrder_TableNameDrinksManners_ReturnsCorrectlyPopulatedObject()
        {
            var testString = "Table 28. Two cappucino's, one tea with sugar and one hot chocolate please";
            var expectedReturn = GetValidOrder(testString);

            var responseObject = _sut.BuildOrder(testString, _user);

            Assert.That(expectedReturn, Is.EqualTo(responseObject));
        }

        [Test]
        public void BuildOrder_DrinksTableNameNoManners_ReturnsCorrectlyPopulatedObject()
        {
            var testString = "Two cappucino's, one hot chocolate and one tea to table 28";
            var expectedReturn = GetValidOrder(testString, false);

            var responseObject = _sut.BuildOrder(testString, _user);

            Assert.That(expectedReturn, Is.EqualTo(responseObject));
        }

        private Order GetValidOrder(string testString, bool mannersUsed = true)
        {
            return new Order
            {
                TableNumber = 28,
                OrderText = testString,
                MannersUsed = mannersUsed,
                RequestingUser = _user
            };
        }
    }
}
