using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnarchocapitalismBot.Exchanges
{
    public struct Ticker : ITickerEntry
    {
        public decimal HighestBidPrice { get; set; }
        public decimal LowestAskPrice  { get; set; }
        public decimal LastTradePrice  { get; set; }

        public decimal Volume24Hours   { get; set; }
        
        public Ticker(ITickerEntry ticker)
        {
            this.HighestBidPrice = ticker.HighestBidPrice;
            this.LowestAskPrice  = ticker.LowestAskPrice;
            this.LastTradePrice  = ticker.LastTradePrice;

            this.Volume24Hours   = ticker.Volume24Hours;
        }

        public Ticker Flip()
        {
            return new Ticker
            {
                HighestBidPrice = 1m / this.LowestAskPrice,
                LowestAskPrice  = 1m / this.HighestBidPrice,
                LastTradePrice  = 1m / this.LastTradePrice,
                Volume24Hours   = this.Volume24Hours / this.LastTradePrice
            };
        }

        public Ticker Update(OrderBook orderBook)
        {
            return new Ticker
            {
                HighestBidPrice = orderBook.Bids[0].Price,
                LowestAskPrice  = orderBook.Asks[0].Price,
                LastTradePrice  = 1m / this.LastTradePrice,
                Volume24Hours   = this.Volume24Hours / this.LastTradePrice
            };
        }
    }
}
