using AnarchocapitalismBot.Exchanges;
using AnarchocapitalismBot.Mathematics;
using System;
using System.Collections.Generic;
using System.Linq;
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
        private IExchange exchange = new ExmoExchange();

        public MainWindow()
        {
            this.InitializeComponent();
        }

        private async Task Update()
        {
            IReadOnlyList<string> currencies = this.exchange.SupportedCurrencies;
            Matrix<decimal> prices = await this.exchange.GetSpotPrices();

            Matrix<ArbitragePath> arbitrage1 = prices.Map(ArbitragePathSemiring.Instance, (y, x, value) =>
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
            Matrix<ArbitragePath> arbitrage2 = arbitrage1 * arbitrage1;
            Matrix<ArbitragePath> arbitrage3 = arbitrage1 * arbitrage2;
            Matrix<ArbitragePath> arbitrage4 = arbitrage1 * arbitrage3;
            Matrix<ArbitragePath> arbitrage5 = arbitrage1 * arbitrage4;
            Matrix<ArbitragePath> arbitrage6 = arbitrage1 * arbitrage5;

            Matrix<ArbitragePath> arbitrage = arbitrage1;
            Matrix<ArbitragePath> arbitrageN = arbitrage1;
            for (int tradeCount = 1; tradeCount < 7; tradeCount++)
            {
                arbitrageN = arbitrage1 * arbitrageN;
                arbitrage = arbitrage + arbitrageN;
            }

            await this.Dispatcher.InvokeAsync(() =>
            {
                if (this.ListView.Items.Count != currencies.Count)
                {
                    this.ListView.Items.Clear();
                    for (int i = 0; i < currencies.Count; i++)
                    {
                        this.ListView.Items.Add(new { Name = "", BestMultiplier = "x0.0000", BestPath = "", ExchangeName = "" });
                    }
                }

                for (int i = 0; i < currencies.Count; i++)
                {
                    ArbitragePath arbitragePath = arbitrage[(uint)i, (uint)i];
                    this.ListView.Items[i] = new { Name = currencies[i], BestMultiplier = "x" + arbitragePath.Multiplier.ToString("0.0000"), BestPath = string.Join(" -> ", arbitragePath.Currencies), ExchangeName = this.exchange.Name };
                }
            });
        }

        private async void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            await this.exchange.ConnectReadOnly();
            await this.Update();
        }

        private void FileExit_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private async void Update_Click(object sender, RoutedEventArgs e)
        {
            await this.Update();
        }
    }
}
