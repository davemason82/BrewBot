using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ZutoBrewBot.Services.Interfaces;
using Newtonsoft.Json;
using ZutoBrewBot.Models;
using ZutoBrewBot.Repositories.Interfaces;

namespace ZutoBrewBot.Controllers
{
    [Produces("application/json")]
    [Route("api/Orders")]
    public class OrdersController : Controller
    {
        private IOrderCache _orderCache;
        private ISlackRepository _slackRepository;

        public OrdersController(IOrderCache orderCache, ISlackRepository slackRepository)
        {
            _orderCache = orderCache;
            _slackRepository = slackRepository;
        }

        // GET api/orders
        [HttpGet]
        public List<Order> Get()
        {
            var confirmedOrders = _orderCache.GetConfirmedOrders();
            return confirmedOrders;
        }

        // DELETE api/orders/{tableNumber}/
        [HttpDelete]
        public void Delete(int tableNumber, bool cancelled)
        {
            var order = _orderCache.GetByTableNumber(tableNumber);
            var message = cancelled ? $"<@{order.RequestingUser}> Sorry, your order has been cancelled. Give our Barista a shout to find out why." :
                                        $"<@{order.RequestingUser}> Your drinks order is really for collection!";

            _orderCache.DeleteOrder(tableNumber);

            _slackRepository.PostMessage(message);
        }
    }
}