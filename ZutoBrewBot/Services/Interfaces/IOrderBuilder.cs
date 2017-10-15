using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ZutoBrewBot.Models;

namespace ZutoBrewBot.Services.Interfaces
{
    public interface IOrderBuilder
    {
        Order BuildOrder(string orderString, string requestingUser);
    }
}
