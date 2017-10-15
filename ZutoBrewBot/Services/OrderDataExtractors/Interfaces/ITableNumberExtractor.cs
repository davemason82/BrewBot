using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ZutoBrewBot.Models.Responses;

namespace ZutoBrewBot.Services.OrderDataExtractors.Interfaces
{
    public interface ITableNumberExtractor
    {
        OrderTextTableExtractorResponse GetTableNumber(string orderString);
    }
}
