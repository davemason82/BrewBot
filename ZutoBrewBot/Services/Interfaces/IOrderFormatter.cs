using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ZutoBrewBot.Models;

namespace ZutoBrewBot.Services.Interfaces
{
    public interface IOrderFormatter
    {
        string FormatOrder(Order order);
    }
}
