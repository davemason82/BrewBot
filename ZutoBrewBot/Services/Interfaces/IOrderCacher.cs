using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ZutoBrewBot.Models;

namespace ZutoBrewBot.Services.Interfaces
{
    public interface IOrderCache
    {
        void CacheOrder(Order order);
        Order GetByTableNumber(int tableNumber);
        bool OrderExistsForTable(int tableNumber);
        void ConfirmOrder(string requestingUser);
        void DeleteOrder(string requestingUser);
        void DeleteOrder(int tableNumber);
        List<Order> GetConfirmedOrders();
    }
}
