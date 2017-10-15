using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ZutoBrewBot.Models;
using ZutoBrewBot.Repositories.Interfaces;
using ZutoBrewBot.Services.Interfaces;

namespace ZutoBrewBot.Services
{
    public class OrderCache : IOrderCache
    {
        private IOrderRepository _orderRepository;

        public OrderCache(IOrderRepository orderRepository)
        {
            _orderRepository = orderRepository;
        }

        public void CacheOrder(Order order)
        {
            if (order == null)
                throw new ArgumentException("An Order must be specified", "order");

            var sameUserOrder = _orderRepository.Select(order.RequestingUser);
            if (sameUserOrder != null && sameUserOrder.TableNumber > 0 && sameUserOrder.TableNumber != order.TableNumber)
            {
                throw new InvalidOperationException("Sorry, it looks like you have an order already for a different table");
            }

            var existingOrder = _orderRepository.Select(order.TableNumber);
            if (existingOrder == null || existingOrder.TableNumber == 0)
            {
                
                _orderRepository.Insert(order);
            }
            else
            {
                if (existingOrder.Confirmed)
                {
                    throw new InvalidOperationException("It looks like you already have a confirmed order with our baristas. Give them a shout if you'd like to change it.");
                }
                else
                {
                    throw new InvalidOperationException("Oops, it looks like you have an existing, unconfirmed order.");
                }                
            }
        }

        public void ConfirmOrder(string requestingUser)
        {
            var existingOrder = _orderRepository.Select(requestingUser);
            ConfirmOrder(existingOrder);
        }

        private void ConfirmOrder(Order existingOrder)
        {
            if (existingOrder == null || existingOrder.TableNumber == 0)
            {
                throw new ArgumentException("Unable to find an order for that table");
            }
            else if (existingOrder.Confirmed)
            {
                throw new InvalidOperationException("This order has already been confirmed");
            }
            else
            {
                existingOrder.Confirmed = true;
                _orderRepository.Update(existingOrder);
            }
        }

        public void DeleteOrder(string requestingUser)
        {
            var existingOrder = _orderRepository.Select(requestingUser);
            DeleteOrder(existingOrder);
        }

        public void DeleteOrder(int tableNumber)
        {
            var existingOrder = _orderRepository.Select(tableNumber);
            DeleteOrder(existingOrder);
        }

        private void DeleteOrder(Order existingOrder)
        {
            if (existingOrder == null || existingOrder.TableNumber == 0)
            {
                throw new ArgumentException("Unable to find order for that tableNumber");
            }

            _orderRepository.Delete(existingOrder.TableNumber);
        }

        public List<Order> GetConfirmedOrders()
        {
            return _orderRepository.GetConfirmedOrders();
        }

        public Order GetByTableNumber(int tableNumber)
        {
            return _orderRepository.Select(tableNumber);
        }

        public bool OrderExistsForTable(int tableNumber)
        {
            throw new NotImplementedException();
        }
    }
}
