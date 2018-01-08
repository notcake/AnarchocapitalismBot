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
        }

        private async Task Update()
        {
            if (!this.exchange.Connected)
            {
                await this.exchange.ConnectReadOnly();
            }

            IReadOnlyList<string> currencies = this.exchange.Currencies;
            decimal[,] prices = await this.exchange.GetSpotPrices();

            Matrix<ArbitragePath> arbitrage1 = Matrix<decimal>.FromArray(DecimalRing.Instance, prices).Map(ArbitragePathSemiring.Instance, (y, x, value) =>
            {
                if (x == y)
                {
                    return new ArbitragePath(value, new List<string> { currencies[(int)x] });
                }
                else
                {
                    return new ArbitragePath(value, new List<string> { currencies[(int)x], currencies[(int)y] });
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
                if (this.CurrencyCyclesListView.Items.Count != currencies.Count)
                {
                    this.CurrencyCyclesListView.Items.Clear();
                    for (int i = 0; i < currencies.Count; i++)
                    {
                        this.CurrencyCyclesListView.Items.Add(new { Good = false, Name = "", BestMultiplier = 0, BestPath = "", ExchangeName = "", ArbitragePath = new ArbitragePath(0, new List<string>()) });
                    }
                }

                for (int i = 0; i < currencies.Count; i++)
                {
                    ArbitragePath arbitragePath = arbitrage[(uint)i, (uint)i];
                    this.CurrencyCyclesListView.Items[i] = new { Good = arbitragePath.Multiplier > 1.00m, Name = currencies[i], BestMultiplier = arbitragePath.Multiplier, BestPath = string.Join(" -> ", arbitragePath.Currencies), ExchangeName = this.exchange.Name, ArbitragePath = arbitragePath };
                }
            });
        }

        private async void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            await this.Update();
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

            await this.Update();
        }

        private async void MaximumTradeCountComboBox_SelectionChanged(object sender, RoutedEventArgs e)
        {
            if (this.MaximumTradeCountComboBox.SelectedItem == null) { return; }

            uint maximumTradeCount = (uint)this.MaximumTradeCountComboBox.SelectedItem;
            if (this.maximumTradeCount == maximumTradeCount) { return; }

            this.maximumTradeCount = maximumTradeCount;
            await this.Update();
        }

        private async void Refresh_Click(object sender, RoutedEventArgs e)
        {
            await this.Update();
        }

        private void CurrencyCyclesListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            dynamic selectedItem = this.CurrencyCyclesListView.SelectedItem;
            if (selectedItem != null)
            {
                ArbitragePath arbitragePath = selectedItem.ArbitragePath;
                for (int i = 1; i < arbitragePath.Currencies.Count; i++)
                {

                }
            }
            else
            {
                this.ExecutionListView.Items.Clear();
            }
        }
    }
}
