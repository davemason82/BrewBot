using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Noobot.Core.MessagingPipeline.Middleware;
using Noobot.Core.MessagingPipeline.Middleware.ValidHandles;
using ZutoBrewBot.Services.Interfaces;
using Noobot.Core.MessagingPipeline.Response;
using Noobot.Core.MessagingPipeline.Request;
using ZutoBrewBot.Models;

namespace ZutoBrewBot.Middleware
{
    public class OrderMiddleware : MiddlewareBase
    {
        private IOrderBuilder _orderBuilder;
        private IOrderFormatter _orderFormatter;
        private IOrderCache _orderCache;
        private IMannersMessageGenerator _mannersMessageGenerator;

        public OrderMiddleware(IMiddleware next, IOrderBuilder orderBuilder, IOrderFormatter orderFormatter, 
                                IOrderCache orderCache, IMannersMessageGenerator mannersMessageGenerator) : base(next)
        {
            _orderBuilder = orderBuilder;
            _orderFormatter = orderFormatter;
            _orderCache = orderCache;
            _mannersMessageGenerator = mannersMessageGenerator;

            HandlerMappings = new[]
            {
                new HandlerMapping
                {
                    ValidHandles = ContainsTextHandle.For("table", "Table", "TABLE"),
                    Description = "Can I take your order?",
                    EvaluatorFunc = OrderHandler
                }
            };
        }

        public IEnumerable<ResponseMessage> OrderHandler(IncomingMessage message, IValidHandle matchedHandle)
        {
            Order order = null;
            string responseText = string.Empty;

            try
            {
                order = _orderBuilder.BuildOrder(message.FullText, message.Username);

                if (!order.MannersUsed)
                {
                    responseText = _mannersMessageGenerator.GetMannerResponse();
                    responseText += Environment.NewLine;
                }

                _orderCache.CacheOrder(order);
                responseText += _orderFormatter.FormatOrder(order);
            }
            catch (ArgumentException)
            {
                responseText += "Sorry, I couldn't figure out your table number. Please try again.";
            }
            catch (InvalidOperationException ioEx)
            {
                responseText += ioEx.Message;
            }
            catch (Exception ex)
            {
                responseText += "Sorry, it looks like something has gone wrong. Please could you try again?";
                responseText += Environment.NewLine;
                responseText += ex.Message;
            }

            yield return message.ReplyDirectlyToUser(responseText);
        }
    }
}