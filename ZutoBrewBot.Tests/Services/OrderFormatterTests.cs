using System;
using System.Collections.Generic;
using System.Text;
using ZutoBrewBot.Services;
using NUnit.Framework;
using ZutoBrewBot.Models;

namespace ZutoBrewBot.Tests.Services
{
    public class OrderFormatterTests
    {
        private OrderFormatter _sut;

        [SetUp]
        public void SetUp()
        {
            _sut = new OrderFormatter();
        }

        [Test]
        public void FormatOrder_ValidOrder_StringReturned()
        {
            string orderText = "Two cappucino's to table 5 please";
            var order = new Order
            {
                TableNumber = 5,
                OrderText = orderText,
                MannersUsed = true
            };

            string expected = "Is this order correct?";
            expected += Environment.NewLine;
            expected += Environment.NewLine;
            expected += "Table 5 Drinks Order:";
            expected += Environment.NewLine;
            expected += orderText;

            string actual = _sut.FormatOrder(order);

            Assert.That(expected, Is.EqualTo(actual));
        }
    }
}
