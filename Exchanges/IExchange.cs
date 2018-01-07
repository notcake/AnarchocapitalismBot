using AnarchocapitalismBot.Mathematics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnarchocapitalismBot.Exchanges
{
    public interface IExchange
    {
        string Name { get; }

        // Connection
        Task<bool> ConnectReadOnly();
        Task<bool> ConnectReadWrite();
        Task Disconnect();
        bool Connected { get; }
        bool TradingReady { get; }

        // Currencies
        IReadOnlyList<string> SupportedCurrencies { get; }
        Matrix<bool> SupportedCurrencyPairs { get; }

        Task<Matrix<decimal>> GetSpotPrices();
    }
}
