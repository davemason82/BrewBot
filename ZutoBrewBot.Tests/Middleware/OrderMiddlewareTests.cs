using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using Moq;
using ZutoBrewBot.Middleware;
using ZutoBrewBot.Services.Interfaces;
using ZutoBrewBot.Models;
using Noobot.Core.MessagingPipeline.Middleware;
using Noobot.Core.MessagingPipeline.Request;
using Noobot.Core.MessagingPipeline.Response;

namespace ZutoBrewBot.Tests.Middleware
{
    [TestFixture]
    public class OrderMiddlewareTests : MiddlewareTestBase
    {
        private OrderMiddleware _sut;
        private Order _order;
        private Order _invalidOrder;
        private string _json;
        private Mock<IOrderBuilder> _orderBuilder;
        private Mock<IOrderFormatter> _orderFormatter;
        private Mock<IOrderCache> _orderCache;
        private Mock<IMiddleware> _nextMiddleware;

        [SetUp]
        public void SetUp()
        {
            _json = "some JSON string";
            _orderText = "Two cappucino's to table 5 please";
            _user = "someuser";

            _order = new Order
            {
                TableNumber = 5,
                OrderText = _orderText,
                MannersUsed = true
            };

            _invalidOrder = new Order
            {
                TableNumber = 0,
                OrderText = string.Empty,
                MannersUsed = false
            };

            _orderBuilder = new Mock<IOrderBuilder>();
            _orderBuilder.Setup(x => x.BuildOrder(_orderText, _user)).Returns(_order);

            _orderFormatter = new Mock<IOrderFormatter>();
            _orderFormatter.Setup(x => x.FormatOrder(_order)).Returns(_json);

            _orderCache = new Mock<IOrderCache>();
            _orderCache.Setup(x => x.CacheOrder(_order));

            _nextMiddleware = new Mock<IMiddleware>();

            _sut = new OrderMiddleware(_nextMiddleware.Object, _orderBuilder.Object, _orderFormatter.Object, _orderCache.Object);
        }

        [Test]
        public void OrderHandler_CallsOrderBuilderWithFullText()
        {
            var incomingMessage = GetIncomingMessage();

            var responseContent = _sut.OrderHandler(incomingMessage, null);
            var responses = GetResponseMessages(responseContent);

            _orderBuilder.Verify(x => x.BuildOrder(incomingMessage.FullText, _user), Times.Once);
        }

        [Test]
        public void OrderHandler_CallsOrderCacheWithOrder()
        {
            var incomingMessage = GetIncomingMessage();

            var responseContent = _sut.OrderHandler(incomingMessage, null);
            var responses = GetResponseMessages(responseContent);

            _orderCache.Verify(x => x.CacheOrder(_order), Times.Once);
        }

        [Test]
        public void OrderHandler_OrderCacheThrowsInvalidOpException_ReturnsCorrectMessage()
        {
            var incomingMessage = GetIncomingMessage();
            _orderCache.Setup(x => x.CacheOrder(_order)).Throws<InvalidOperationException>();

            var responseContent = _sut.OrderHandler(incomingMessage, null);
            var responses = GetResponseMessages(responseContent);

            Assert.That(responses.Count, Is.EqualTo(1));
            Assert.That(responses[0], Is.EqualTo("Oops, it looks like you have an existing, unconfirmed order. Please confirm it before trying to change it."));
        }

        [Test]
        public void OrderHandler_CallsOrderFormatterWithOrder()
        {
            var incomingMessage = GetIncomingMessage();

            var responseContent = _sut.OrderHandler(incomingMessage, null);
            var responses = GetResponseMessages(responseContent);

            _orderFormatter.Verify(x => x.FormatOrder(_order), Times.Once);

            Assert.That(responses.Count, Is.EqualTo(1));
            Assert.That(responses[0], Is.EqualTo(_json));
        }

        [Test]
        public void OrderHandler_CantGetTableNumber_SetsErrorMessage()
        {
            var incomingMessage = GetIncomingMessage();
            _orderBuilder.Setup(x => x.BuildOrder(incomingMessage.FullText, _user)).Throws<ArgumentException>();

            var responseContent = _sut.OrderHandler(incomingMessage, null);

            // ensure we don't call "format order" 
            _orderFormatter.Verify(x => x.FormatOrder(_invalidOrder), Times.Never);

            var responses = GetResponseMessages(responseContent);

            // ensure there's only one message in the response
            Assert.That(responses.Count, Is.EqualTo(1));

            // make sure the response message is the validation error
            Assert.That(responses[0], Is.EqualTo("Sorry, I couldn't figure out your table number. Please try again."));
        }
    }
}
