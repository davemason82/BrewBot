using Noobot.Core.MessagingPipeline.Request;
using Noobot.Core.MessagingPipeline.Response;
using System;
using System.Collections.Generic;
using System.Text;

namespace ZutoBrewBot.Tests.Middleware
{
    public abstract class MiddlewareTestBase
    {
        protected string _orderText;
        protected string _user;

        protected IncomingMessage GetIncomingMessage()
        {
            return new IncomingMessage
            {
                FullText = _orderText,
                Username = _user
            };
        }

        protected List<string> GetResponseMessages(IEnumerable<ResponseMessage> response)
        {
            var returnList = new List<string>();

            using (IEnumerator<ResponseMessage> enumerator = response.GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    returnList.Add(enumerator.Current.Text);
                }
            }

            return returnList;
        }
    }
}
