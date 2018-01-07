﻿using AnarchocapitalismBot.Exchanges;
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
        }

        private async Task Update()
        {
            if (!this.exchange.Connected)
            {
                await this.exchange.ConnectReadOnly();
            }

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
                        this.ListView.Items.Add(new { Good = false, Name = "", BestMultiplier = 0, BestPath = "", ExchangeName = "" });
                    }
                }

                for (int i = 0; i < currencies.Count; i++)
                {
                    ArbitragePath arbitragePath = arbitrage[(uint)i, (uint)i];
                    this.ListView.Items[i] = new { Good = arbitragePath.Multiplier > 1.00m, Name = currencies[i], BestMultiplier = arbitragePath.Multiplier, BestPath = string.Join(" -> ", arbitragePath.Currencies), ExchangeName = this.exchange.Name };
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
            await this.Update();
        }

        private async void Update_Click(object sender, RoutedEventArgs e)
        {
            await this.Update();
        }
    }
}
