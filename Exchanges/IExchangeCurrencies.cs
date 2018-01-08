using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnarchocapitalismBot.Exchanges
{
    public interface IExchangeCurrencies : IReadOnlyList<string>
    {
        int IndexOf(string currency);
    }
}
