using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ZutoBrewBot.Models;
using ZutoBrewBot.Services.Interfaces;
using ZutoBrewBot.Services.OrderDataExtractors.Interfaces;

namespace ZutoBrewBot.Services
{
    public class OrderBuilder : IOrderBuilder
    {
        private ITableNumberExtractor _tableNumberExtractor;
        private IMannersExtractor _mannersExtractor;

        public OrderBuilder(ITableNumberExtractor tableNumberExtractor, IMannersExtractor mannersExtractor)
        {
            _tableNumberExtractor = tableNumberExtractor;
            _mannersExtractor = mannersExtractor;
        }

        public Order BuildOrder(string orderString, string requestingUser)
        {
            var tableNumberResponse = _tableNumberExtractor.GetTableNumber(orderString);
            if (!tableNumberResponse.TableFound)
            {
                throw new ArgumentException("No table number found in orderString");
            }

            var order = new Order
            { 
                TableNumber = tableNumberResponse.TableNumber,
                OrderText = orderString,
                RequestingUser = requestingUser
            };

            var mannersResponse = _mannersExtractor.GetMannersResponse(orderString);
            order.MannersUsed = mannersResponse.MannersFound;

            return order;
        }
    }
}
