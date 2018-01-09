using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnarchocapitalismBot.Exchanges
{
    public struct OrderBook
    {
        public OrderBookEntry[] Asks;
        public OrderBookEntry[] Bids;
        
        /// <summary>
        /// Annihilate asks until quantity is bought.
        /// </summary>
        /// <param name="quantity"></param>
        /// <returns>(total cost, leftover entry) </returns>
        public (decimal, OrderBookEntry)? ComputeBuyCost(decimal quantity)
        {
            return this.AnnihilateQuantity(this.Asks, quantity);
        }

        /// <summary>
        /// Annihilate asks until cost is spent
        /// </summary>
        /// <param name="cost"></param>
        /// <returns>(total quantity, leftover entry)</returns>
        public (decimal, OrderBookEntry)? ComputeBuyQuantity(decimal cost)
        {
            return this.AnnihilateCost(this.Asks, cost);
        }

        /// <summary>
        /// Annihilate bids until quantity is sold
        /// </summary>
        /// <param name="quantity"></param>
        /// <returns>(total cost, leftover entry)</returns>
        public (decimal, OrderBookEntry)? ComputeSellCost(decimal quantity)
        {
            return this.AnnihilateQuantity(this.Bids, quantity);
        }

        /// <summary>
        /// Annihilate bids until cost is spent
        /// </summary>
        /// <param name="cost"></param>
        /// <returns>(total quantity, leftover entry)</returns>
        public (decimal, OrderBookEntry)? ComputeSellQuantity(decimal cost)
        {
            return this.AnnihilateCost(this.Bids, cost);
        }

        private (decimal, OrderBookEntry)? AnnihilateQuantity(OrderBookEntry[] orders, decimal quantity)
        {
            decimal totalCost = 0;
            foreach (OrderBookEntry entry in orders)
            {
                if (entry.Quantity < quantity)
                {
                    quantity -= entry.Quantity;
                    totalCost += entry.Price * entry.Quantity;
                }
                else
                {
                    totalCost += entry.Price * quantity;
                    return (totalCost, entry.WithQuantity(entry.Quantity - quantity));
                }
            }

            return null;
        }

        private (decimal, OrderBookEntry)? AnnihilateCost(OrderBookEntry[] orders, decimal cost)
        {
            decimal totalQuantity = 0;
            foreach (OrderBookEntry entry in orders)
            {
                if (entry.Quantity * entry.Price < cost)
                {
                    cost -= entry.Quantity * entry.Price;
                    totalQuantity += entry.Quantity;
                }
                else
                {
                    totalQuantity += cost / entry.Price;
                    return (totalQuantity, entry.WithQuantity(entry.Quantity - cost / entry.Price));
                }
            }

            return null;
        }
    }
}
