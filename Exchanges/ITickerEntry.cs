using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnarchocapitalismBot.Exchanges
{
    public interface ITickerEntry
    {
        decimal HighestBidPrice { get; }
        decimal LowestAskPrice  { get; }
        decimal LastTradePrice  { get; }
    }
}
