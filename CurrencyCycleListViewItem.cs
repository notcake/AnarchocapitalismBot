using AnarchocapitalismBot.Exchanges;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnarchocapitalismBot
{
    public struct CurrencyCycleListViewItem
    {
        public bool Good => this.ArbitragePath.Multiplier > 1;

        public IExchange Exchange { get; set; }
        public string Name { get; set; }

        public ArbitragePath ArbitragePath { get; set; }
        public decimal Multiplier => this.ArbitragePath.Multiplier;
        public string Path => string.Join(" -> ", this.ArbitragePath.Currencies);
    }
}
