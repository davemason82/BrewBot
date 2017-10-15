using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using Moq;
using ZutoBrewBot.Services;
using ZutoBrewBot.Repositories.Interfaces;
using ZutoBrewBot.Models;
using System.Threading.Tasks;

namespace ZutoBrewBot.Tests.Services
{
    [TestFixture]
    public class OrderCacheTests
    {
        private OrderCache _sut;
        private Mock<IOrderRepository> _orderRepository;
        private int _tableNumber;
        private string _requestingUser;

        [SetUp]
        public void SetUp()
        {
            _tableNumber = 5;
            _requestingUser = "someuser";
            _orderRepository = new Mock<IOrderRepository>();
            _sut = new OrderCache(_orderRepository.Object);
        }

        [Test]
        public void CacheOrder_PassedNull_ThrowsException()
        {
            Order order = null;
            Assert.Throws<ArgumentException>(() => _sut.CacheOrder(order));
        }

        [Test]
        public void CacheOrder_PassedOrderDoesntExist_InsertsThroughRepo()
        {
            Order order = GetOrder();

            _orderRepository.Setup(x => x.Insert(order));
            _orderRepository.Setup(x => x.Select(order.TableNumber)).Returns(null as Order);
            _orderRepository.Setup(x => x.Update(order));
            _sut.CacheOrder(order);

            _orderRepository.Verify(x => x.Select(order.TableNumber), Times.Once);
            _orderRepository.Verify(x => x.Insert(order), Times.Once);
            _orderRepository.Verify(x => x.Update(order), Times.Never);
        }

        [Test]
        public void CacheOrder_PassedOrderExistsConfirmed_UpdatesThroughRepo()
        {
            Order order = GetOrder();

            _orderRepository.Setup(x => x.Insert(order));
            _orderRepository.Setup(x => x.Select(order.TableNumber)).Returns(order);
            _orderRepository.Setup(x => x.Update(order));
            _sut.CacheOrder(order);

            _orderRepository.Verify(x => x.Select(order.TableNumber), Times.Once);
            _orderRepository.Verify(x => x.Insert(order), Times.Never);
            _orderRepository.Verify(x => x.Update(order), Times.Once);
        }

        [Test]
        public void CacheOrder_PassedOrderExistsUnconfirmed_ThrowsException()
        {
            Order order = GetOrder(false);
            _orderRepository.Setup(x => x.Insert(order));
            _orderRepository.Setup(x => x.Select(order.TableNumber)).Returns(order);
            _orderRepository.Setup(x => x.Update(order));

            Assert.Throws<InvalidOperationException>(() => _sut.CacheOrder(order));

            _orderRepository.Verify(x => x.Select(order.TableNumber), Times.Once);
            _orderRepository.Verify(x => x.Insert(order), Times.Never);
            _orderRepository.Verify(x => x.Update(order), Times.Never);
        }

        [Test]
        public void ConfirmOrder_NoOrderFound_ThrowsException()
        {
            _orderRepository.Setup(x => x.Select(_tableNumber)).Returns(null as Order);

            Assert.Throws<ArgumentException>(() => _sut.ConfirmOrder(_requestingUser));
        }

        [Test]
        public void ConfirmOrder_OrderFoundAlreadyConfirmed_ThrowsException()
        {
            var order = GetOrder(true);
            _orderRepository.Setup(x => x.Select(_requestingUser)).Returns(order);

            Assert.Throws<InvalidOperationException>(() => _sut.ConfirmOrder(_requestingUser));
        }

        [Test]
        public void ConfirmOrder_OrderFoundNotConfirm_CallsOrderRepo()
        {
            var order = GetOrder(false);
            _orderRepository.Setup(x => x.Select(_requestingUser)).Returns(order);
            _orderRepository.Setup(x => x.Update(order));

            _sut.ConfirmOrder(_requestingUser);

            order.Confirmed = true;
            _orderRepository.Verify(x => x.Update(order), Times.Once);
        }

        [Test]
        public void DeleteOrder_OrderNotFound_ThrowsException()
        {
            _orderRepository.Setup(x => x.Select(_tableNumber)).Returns(null as Order);

            Assert.Throws<ArgumentException>(() => _sut.DeleteOrder(_requestingUser));
        }

        [Test]
        public void DeleteOrder_OrderFound_DeletesOrder()
        {
            var order = GetOrder();
            _orderRepository.Setup(x => x.Select(_requestingUser)).Returns(order);
            _orderRepository.Setup(x => x.Delete(_tableNumber));

            _sut.DeleteOrder(_requestingUser);

            _orderRepository.Verify(x => x.Delete(_tableNumber), Times.Once);
        }


        private Order GetOrder(bool confirmed = true)
        {
            return new Order
            {
                TableNumber = _tableNumber,
                OrderText = "Two cappucino's to table 5 please",
                Confirmed = confirmed,
                MannersUsed = true,
                RequestingUser = _requestingUser
            };
        }
    }
}
