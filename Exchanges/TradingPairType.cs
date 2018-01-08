using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnarchocapitalismBot.Exchanges
{
    public enum TradingPairType : byte
    {
        Invalid = 0,
        Buy     = 1,
        Sell    = 2
    }

    public static class TradingPairTypeExtensions
    {
        public static bool IsValid(this TradingPairType tradingPairType)
        {
            return tradingPairType != TradingPairType.Invalid;
        }
    }
}
