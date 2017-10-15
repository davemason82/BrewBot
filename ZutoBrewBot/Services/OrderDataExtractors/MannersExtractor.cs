using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ZutoBrewBot.Models.Responses;
using ZutoBrewBot.Services.OrderDataExtractors.Interfaces;

namespace ZutoBrewBot.Services.OrderDataExtractors
{
    public class MannersExtractor : IMannersExtractor
    {
        private readonly string[] _mannersTerms =
        {
            "please",
            "pls",
            "plz",
            "thanks",
            "ty",
            "thankz",
            "thx",
            "ta.",
            "cheers"
        };

        public MannersExtractorResponse GetMannersResponse(string orderText)
        {
            int i = 0;
            MannersExtractorResponse response = new MannersExtractorResponse
            {
                MannersFound = false,
                MannersText = string.Empty
            };

            // Loops through the accepted "manners" terms, looking for a match
            while (!response.MannersFound && i < _mannersTerms.Length)
            {
                if (orderText.ToLower().Contains(_mannersTerms[i]))
                {
                    response.MannersFound = true;
                    response.MannersText = _mannersTerms[i];
                }

                i++;
            }

            return response;
        }
    }
}
