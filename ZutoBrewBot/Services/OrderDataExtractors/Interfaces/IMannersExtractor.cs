using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ZutoBrewBot.Models.Responses;

namespace ZutoBrewBot.Services.OrderDataExtractors.Interfaces
{
    public interface IMannersExtractor
    {
        MannersExtractorResponse GetMannersResponse(string orderText);
    }
}
