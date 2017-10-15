using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ZutoBrewBot.Models
{
    public class Order
    {
        public int TableNumber { get; set; }
        public string OrderText { get; set; }
        public bool MannersUsed { get; set; }
        public string RequestingUser { get; set; }
        public bool Confirmed { get; set; }

        public override bool Equals(object obj)
        {
            Order other = obj as Order;
            if (other == null)
            {
                return false;
            }

            return (this.TableNumber == other.TableNumber)
                && (this.OrderText == other.OrderText)
                && (this.MannersUsed == other.MannersUsed)
                && (this.RequestingUser == other.RequestingUser);
        }
    }
}