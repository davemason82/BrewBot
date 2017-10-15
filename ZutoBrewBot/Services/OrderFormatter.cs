using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ZutoBrewBot.Models;
using ZutoBrewBot.Services.Interfaces;
using Newtonsoft.Json;

namespace ZutoBrewBot.Services
{
    public class OrderFormatter : IOrderFormatter
    {
        public OrderFormatter() { }

        public string FormatOrder(Order order)
        {
            string message = "Is this order correct?";
            message += Environment.NewLine;
            message += Environment.NewLine;
            message += $"Table {order.TableNumber} Drinks Order:";
            message += Environment.NewLine;
            message += order.OrderText;

            return message;
        }
    }
}
