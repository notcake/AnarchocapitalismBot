using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnarchocapitalismBot.Exchanges
{
    public struct OrderBookEntry
    {
        public decimal Price    { get; set; }
        public decimal Quantity { get; set; }

        public OrderBookEntry WithQuantity(decimal quantity)
        {
            return new OrderBookEntry { Price = this.Price, Quantity = quantity };
        }
    }
}
