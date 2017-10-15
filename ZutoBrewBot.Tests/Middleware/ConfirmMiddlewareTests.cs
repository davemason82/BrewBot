using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using Moq;
using ZutoBrewBot.Middleware;
using Noobot.Core.MessagingPipeline.Middleware;
using ZutoBrewBot.Services.Interfaces;
using ZutoBrewBot.Models;
using Noobot.Core.MessagingPipeline.Request;
using Noobot.Core.MessagingPipeline.Response;

namespace ZutoBrewBot.Tests.Middleware
{
    [TestFixture]
    public class ConfirmMiddlewareTests : MiddlewareTestBase
    {
        private ConfirmMiddleware _sut;
        private int _tableNumber;
        
        private Mock<IMiddleware> _nextMiddleware;
        private Mock<IOrderCache> _orderCache;

        [SetUp]
        public void SetUp()
        {
            _tableNumber = 5;
            _user = "someuser";
            _nextMiddleware = new Mock<IMiddleware>();
            _orderCache = new Mock<IOrderCache>();

            _sut = new ConfirmMiddleware(_nextMiddleware.Object, _orderCache.Object);
        }

        [Test]
        public void ConfirmHandler_YesAndOrderTBC_ConfirmsOrder()
        {
            _orderText = "Yes";
            _orderCache.Setup(x => x.GetByTableNumber(_tableNumber)).Returns(GetOrder(false));
            _orderCache.Setup(x => x.ConfirmOrder(_user));

            var response = _sut.ConfirmHandler(GetIncomingMessage(), null);
            var responseMessages = GetResponseMessages(response);

            _orderCache.Verify(x => x.ConfirmOrder(_user), Times.Once);
        }

        [Test]
        public void ConfirmHandler_YesAndOrderTBC_ReturnsMessage()
        {
            _orderText = "Yes";
            _orderCache.Setup(x => x.GetByTableNumber(_tableNumber)).Returns(GetOrder(false));
            _orderCache.Setup(x => x.ConfirmOrder(_user));

            var response = _sut.ConfirmHandler(GetIncomingMessage(), null);
            var responseMessages = GetResponseMessages(response);

            Assert.That(responseMessages.Count, Is.EqualTo(1));
            Assert.That(responseMessages[0], Is.EqualTo("Ok your order is with our baristas now! I'll let you know when it's ready."));
        }

        [Test]
        public void ConfirmHandler_YesButNoOrderTBC_ReturnsMessage()
        {
            _orderText = "Yes";
            _orderCache.Setup(x => x.ConfirmOrder(_user)).Throws<ArgumentException>();
            
            var response = _sut.ConfirmHandler(GetIncomingMessage(), null);
            var responseMessages = GetResponseMessages(response);

            Assert.That(responseMessages.Count, Is.EqualTo(1));
            Assert.That(responseMessages[0], Is.EqualTo("Sorry, I couldn't find an order to confirm. What drinks would you like to order?"));
        }

        [Test]
        public void ConfirmHandler_NoAndOrderTBC_DeletesOrder()
        {
            _orderText = "No";
            _orderCache.Setup(x => x.GetByTableNumber(_tableNumber)).Returns(GetOrder(false));
            _orderCache.Setup(x => x.DeleteOrder(_user));

            var response = _sut.ConfirmHandler(GetIncomingMessage(), null);
            var responseMessages = GetResponseMessages(response);

            _orderCache.Verify(x => x.DeleteOrder(_user), Times.Once);
        }

        [Test]
        public void ConfirmHandler_NoAndOrderTBC_ReturnsMessage()
        {
            _orderText = "No";
            _orderCache.Setup(x => x.GetByTableNumber(_tableNumber)).Returns(GetOrder(false));
            _orderCache.Setup(x => x.DeleteOrder(_user));

            var response = _sut.ConfirmHandler(GetIncomingMessage(), null);
            var responseMessages = GetResponseMessages(response);

            Assert.That(responseMessages.Count, Is.EqualTo(1));
            Assert.That(responseMessages[0], Is.EqualTo("Ok, I've removed that order for you. Thanks anyway."));
        }

        [Test]
        public void ConfirmHandler_NoButNoOrderTBC_ReturnsMessage()
        {
            _orderText = "No";
            _orderCache.Setup(x => x.DeleteOrder(_user)).Throws<ArgumentException>();

            var response = _sut.ConfirmHandler(GetIncomingMessage(), null);
            var responseMessages = GetResponseMessages(response);

            Assert.That(responseMessages.Count, Is.EqualTo(1));
            Assert.That(responseMessages[0], Is.EqualTo("You don't have any unconfirmed orders..."));
        }

        private Order GetOrder(bool confirmed)
        {
            return new Order
            {
                TableNumber = _tableNumber,
                OrderText = _orderText,
                Confirmed = confirmed,
                RequestingUser = _user,
                MannersUsed = true
            };
        }

        
    }
}
