using Noobot.Core.MessagingPipeline.Middleware;
using Noobot.Core.MessagingPipeline.Middleware.ValidHandles;
using Noobot.Core.MessagingPipeline.Request;
using Noobot.Core.MessagingPipeline.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ZutoBrewBot.Services.Interfaces;

namespace ZutoBrewBot.Middleware
{
    public class ConfirmMiddleware : MiddlewareBase
    {
        private IOrderCache _orderCache;
        private string[] _positiveResponses = { "confirm", "Confirm", "CONFIRM", "yes", "Yes", "YES", "aye", "yep" };
        private string[] _negativeResponses = { "no", "No", "NO", "nope", "wrong" };

        public ConfirmMiddleware(IMiddleware next, IOrderCache orderCache) : base(next)
        {
            _orderCache = orderCache;
            

            HandlerMappings = new[]
            {
                new HandlerMapping
                {
                    ValidHandles = ExactMatchHandle.For(_positiveResponses.Concat(_negativeResponses).ToArray()),
                    Description = "Please confirm this order is correct",
                    EvaluatorFunc = ConfirmHandler
                }
            };
        }

        public IEnumerable<ResponseMessage> ConfirmHandler(IncomingMessage message, IValidHandle matchedHandle)
        {
            string response = string.Empty;

            if (_negativeResponses.Contains(message.FullText))
            {
                try
                {
                    _orderCache.DeleteOrder(message.Username);
                    response = "Ok, I've removed that order for you. Thanks anyway.";
                }
                catch (ArgumentException)
                {
                    response = "You don't have any unconfirmed orders...";
                }
            }
            else if (_positiveResponses.Contains(message.FullText))
            {
                try
                {
                    _orderCache.ConfirmOrder(message.Username);
                    response = "Ok your order is with our baristas now! I'll let you know when it's ready.";
                }
                catch (ArgumentException)
                {
                    response = "Sorry, I couldn't find an order to confirm. What drinks would you like to order?";
                }
            }

            yield return message.ReplyDirectlyToUser(response);
        }
    }
}
