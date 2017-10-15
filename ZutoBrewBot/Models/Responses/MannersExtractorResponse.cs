using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ZutoBrewBot.Models.Responses
{
    public class MannersExtractorResponse
    {
        public bool MannersFound { get; set; }
        public string MannersText { get; set; }

        public override bool Equals(object obj)
        {
            MannersExtractorResponse other = obj as MannersExtractorResponse;
            if (other == null)
            {
                return false;
            }

            return (this.MannersFound == other.MannersFound)
                && (this.MannersText == other.MannersText);
        }
    }
}
