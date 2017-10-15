using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ZutoBrewBot.Models;

namespace ZutoBrewBot.Repositories.Interfaces
{
    public interface IOrderRepository
    {
        void Insert(Order order);
        void Update(Order order);
        void Delete(int tableNumber);
        Order Select(int tableNumber);
        Order Select(string requestingUser);
        List<Order> GetConfirmedOrders();
    }
}
