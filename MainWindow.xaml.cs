using AnarchocapitalismBot.Exchanges;
using AnarchocapitalismBot.Mathematics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace AnarchocapitalismBot
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Dictionary<string, IExchange> exchanges = new Dictionary<string, IExchange>();
        private IExchange exchange = null;
        private uint maximumTradeCount = 0;

        // Results
        Ticker[,] tickers = null;
        
        public MainWindow()
        {
            this.InitializeComponent();

            // Populate exchanges
            foreach (Type type in Assembly.GetExecutingAssembly().GetTypes())
            {
                if (type.GetCustomAttribute<ExchangeAttribute>() != null)
                {
                    IExchange exchange = (IExchange)type.GetConstructor(Type.EmptyTypes).Invoke(Array.Empty<object>());
                    this.exchanges.Add(exchange.Name, exchange);
                }
            }
            
            foreach (string exchangeName in this.exchanges.Keys.OrderBy(x => x))
            {
                this.ExchangeComboBox.Items.Add(exchangeName);
            }

            this.ExchangeComboBox.SelectedIndex = 0;

            // Populate maximum trade counts
            for (uint i = 2; i <= 10; i++)
            {
                this.MaximumTradeCountComboBox.Items.Add(i);
            }
            this.MaximumTradeCountComboBox.SelectedIndex = 5;

            // Timer
            DispatcherTimer timer = new DispatcherTimer();
            timer.Tick += this.Timer_Tick;
            timer.Interval = new TimeSpan(0, 0, 0, 0, 400);
            timer.Start();
        }

        private async Task UpdateCurrencyCycles()
        {
            IExchange exchange = this.exchange;

            if (!exchange.Connected)
            {
                await exchange.ConnectReadOnly();
            }

            this.tickers = await exchange.GetTicker();
            
            Matrix<ArbitragePath> arbitrage1 = Matrix<Ticker>.FromArray(NullRing<Ticker>.Instance, this.tickers)
                .Map(ArbitragePathSemiring.Instance, (y, x, value) =>
                {
                    decimal multiplier = 0;
                    if (value.LowestAskPrice != 0)
                    {
                        multiplier = 1m / value.LowestAskPrice * (1m - exchange.FeePercentage * 0.01m);
                    }

                    if (x == y)
                    {
                        return new ArbitragePath(multiplier, new List<string> { exchange.Currencies[(int)x] });
                    }
                    else
                    {
                        return new ArbitragePath(multiplier, new List<string> { exchange.Currencies[(int)x], exchange.Currencies[(int)y] });
                    }
                });
            Matrix<ArbitragePath> arbitrage = arbitrage1;
            Matrix<ArbitragePath> arbitrageN = arbitrage1;
            for (int tradeCount = 1; tradeCount < this.maximumTradeCount; tradeCount++)
            {
                arbitrageN = arbitrage1 * arbitrageN;
                arbitrage = arbitrage + arbitrageN;
            }

            await this.Dispatcher.InvokeAsync(() =>
            {
                if (this.CurrencyCyclesListView.Items.Count != exchange.Currencies.Count)
                {
                    this.CurrencyCyclesListView.Items.Clear();
                    for (int i = 0; i < exchange.Currencies.Count; i++)
                    {
                        this.CurrencyCyclesListView.Items.Add(new CurrencyCycleListViewItem { Name = exchange.Currencies[i], Exchange = exchange, ArbitragePath = new ArbitragePath(0, new List<string>()) });
                    }
                }

                this.suppressExecutionUpdate = true;
                int selectedIndex = this.CurrencyCyclesListView.SelectedIndex;
                for (int i = 0; i < exchange.Currencies.Count; i++)
                {
                    ArbitragePath arbitragePath = arbitrage[(uint)i, (uint)i];
                    this.CurrencyCyclesListView.Items[i] = new CurrencyCycleListViewItem { Name = exchange.Currencies[i], Exchange = exchange, ArbitragePath = arbitragePath };
                }
                this.CurrencyCyclesListView.SelectedIndex = selectedIndex;
                this.suppressExecutionUpdate = false;
            });
        }

        CurrencyCycleListViewItem? executionCycle = null;
        bool updatingExecution = false;
        private async Task UpdateExecution()
        {
            if (this.executionCycle != null)
            {
                this.updatingExecution = true;

                int selectedIndex = this.CurrencyCyclesListView.SelectedIndex;
                CurrencyCycleListViewItem item = executionCycle.Value;
                
                ArbitragePath arbitragePath = item.ArbitragePath;

                Ticker[,] tickers = this.tickers;

                decimal sourceQuantity = 0.1m;

                List<ExecutionListViewItem> items = new List<ExecutionListViewItem>();
                for (int i = 1; i < arbitragePath.Currencies.Count; i++)
                {
                    string sourceCurrency      = arbitragePath.Currencies[i - 1];
                    string destinationCurrency = arbitragePath.Currencies[i];
                    int sourceCurrencyIndex      = item.Exchange.Currencies.IndexOf(sourceCurrency);
                    int destinationCurrencyIndex = item.Exchange.Currencies.IndexOf(destinationCurrency);

                    TradeType type;
                    (string, string) tradingPair;
                    switch (item.Exchange.TradingPairs[item.Exchange.Currencies.IndexOf(destinationCurrency), item.Exchange.Currencies.IndexOf(sourceCurrency)])
                    {
                        case TradingPairType.Buy:
                            type = TradeType.Buy;
                            tradingPair = (destinationCurrency, sourceCurrency);
                            break;
                        case TradingPairType.Sell:
                            type = TradeType.Sell;
                            tradingPair = (sourceCurrency, destinationCurrency);
                            break;
                        case TradingPairType.Invalid:
                        default:
                            throw new InvalidOperationException();
                    }

                    Ticker ticker = tickers[item.Exchange.Currencies.IndexOf(tradingPair.Item1), item.Exchange.Currencies.IndexOf(tradingPair.Item2)];
                    ticker = ticker.Update(await item.Exchange.GetOrderBook(tradingPair));

                    decimal destinationQuantity;
                    if (type == TradeType.Buy)
                    {
                        destinationQuantity = sourceQuantity / ticker.LowestAskPrice * (1m - item.Exchange.FeePercentage * 0.01m);
                    }
                    else
                    {
                        destinationQuantity = sourceQuantity * ticker.HighestBidPrice * (1m - item.Exchange.FeePercentage * 0.01m);
                    }

                    items.Add(new ExecutionListViewItem
                    {
                        Exchange = item.Exchange,
                        TradingPair = tradingPair,
                        Type = type,
                        Price = type == TradeType.Buy ? ticker.LowestAskPrice : ticker.HighestBidPrice,
                        Ticker = ticker,
                        SourceQuantity = sourceQuantity,
                        DestinationQuantity = destinationQuantity
                    });

                    sourceQuantity = destinationQuantity;
                }

                if (this.CurrencyCyclesListView.SelectedIndex == selectedIndex)
                {
                    await this.Dispatcher.InvokeAsync(() =>
                    {
                        this.ExecutionListView.Items.Clear();
                        items.ForEach(x => this.ExecutionListView.Items.Add(x));
                    });
                }

                this.updatingExecution = false;
            }
            else
            {
                this.ExecutionListView.Items.Clear();
            }
        }

        private async void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            await this.UpdateCurrencyCycles();
        }

        private void FileExit_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private async void ExchangeComboBox_SelectionChanged(object sender, RoutedEventArgs e)
        {
            IExchange exchange = this.exchanges[(string)this.ExchangeComboBox.SelectedItem];
            if (this.exchange == exchange) { return; }

            this.exchange = exchange;
            this.CurrencyCyclesListView.Items.Clear();
            this.ExecutionListView.Items.Clear();
            this.executionCycle = null;

            await this.UpdateCurrencyCycles();
        }

        private async void MaximumTradeCountComboBox_SelectionChanged(object sender, RoutedEventArgs e)
        {
            if (this.MaximumTradeCountComboBox.SelectedItem == null) { return; }

            uint maximumTradeCount = (uint)this.MaximumTradeCountComboBox.SelectedItem;
            if (this.maximumTradeCount == maximumTradeCount) { return; }

            this.maximumTradeCount = maximumTradeCount;
            await this.UpdateCurrencyCycles();
        }

        private async void Refresh_Click(object sender, RoutedEventArgs e)
        {
            await this.UpdateCurrencyCycles();
        }

        bool suppressExecutionUpdate = false;
        private async void CurrencyCyclesListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (this.suppressExecutionUpdate) { return; }

            this.executionCycle = (CurrencyCycleListViewItem?)this.CurrencyCyclesListView.SelectedItem;
            await this.UpdateExecution();
        }

        private async void Timer_Tick(object sender, EventArgs e)
        {
            if (this.updatingExecution) { return; }
            await this.UpdateExecution();
        }
    }
}
