using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ZutoBrewBot.Models.Responses
{
    public class OrderTextTableExtractorResponse
    {
        public bool TableFound { get; set; }
        public int TableNumber { get; set; }
        public string TableText { get; set; }

        public override bool Equals(object obj)
        {
            OrderTextTableExtractorResponse other = obj as OrderTextTableExtractorResponse;
            if (obj == null)
            {
                return false;
            }

            return (this.TableFound == other.TableFound)
                && (this.TableNumber == other.TableNumber)
                && (this.TableText == other.TableText);
        }
    }
}
