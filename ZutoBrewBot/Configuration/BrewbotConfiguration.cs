using Noobot.Core.Configuration;
using Noobot.Toolbox.Middleware;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ZutoBrewBot.Middleware;

namespace ZutoBrewBot.Configuration
{
    public class BrewbotConfiguration : ConfigurationBase
    {
        public BrewbotConfiguration()
        {
            UseMiddleware<HelpMiddleware>();
            UseMiddleware<OrderMiddleware>();
            UseMiddleware<ConfirmMiddleware>();
        }
    }
}
