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
        IReadOnlyList<string> SupportedCurrencies { get; }
    }
}
