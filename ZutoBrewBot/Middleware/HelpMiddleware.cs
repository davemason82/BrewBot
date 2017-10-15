using Noobot.Core.MessagingPipeline.Middleware;
using Noobot.Core.MessagingPipeline.Middleware.ValidHandles;
using Noobot.Core.MessagingPipeline.Request;
using Noobot.Core.MessagingPipeline.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ZutoBrewBot.Middleware
{
    public class HelpMiddleware : MiddlewareBase
    {
        public HelpMiddleware(IMiddleware next) : base(next)
        {
            HandlerMappings = new[]
            {
                new HandlerMapping
                {
                    ValidHandles = ExactMatchHandle.For("hello", "hi", "hey", "help"),
                    Description = "Can I take your order?",
                    EvaluatorFunc = HelpHandler
                }
            };
        }

        public IEnumerable<ResponseMessage> HelpHandler(IncomingMessage message, IValidHandle matchedHandle)
        {
            var responseText = string.Empty;

            responseText += "Hello! I'm the Zuto BrewBot. I can help you bypass the queue and use Slack to get your drinks order to our Baristas. I'll then let you know when it's ready for you to collect.";
            responseText += Environment.NewLine;
            responseText += Environment.NewLine;
            responseText += "To put an order in, just tell me what you'd like and which table number you're sat at (and manners always help!). If you don't include your table number, I won't understand your order, so make sure you include it.";
            responseText += Environment.NewLine;
            responseText += Environment.NewLine;
            responseText += "I'll then ask you to confirm that your order is correct. Simply type \"yes\" or \"confirm\" if your order is right, or \"no\" if it's wrong.";
            responseText += Environment.NewLine;
            responseText += Environment.NewLine;
            responseText += "Once your order is with our Baristas, keep an eye on the #general channel for a message to let you know it's ready.";
            responseText += Environment.NewLine;
            responseText += Environment.NewLine;
            responseText += "Good luck with your hack... May the odds be ever in your favour.";

            yield return message.ReplyDirectlyToUser(responseText);
        }
    }
}
