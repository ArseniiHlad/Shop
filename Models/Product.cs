using System;

namespace Kursach.Models
{
    public class Product
    {
        public string Article { get; set; } = null!;
        public string Name { get; set; } = null!;
        public string Type { get; set; } = null!;
        public string Provider { get; set; } = null!;
        public string Unit { get; set; } = null!;
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public DateTime LastDelivery { get; set; }
        public bool IsEligibleForDiscount { get; set; }

        public Product(string article, string name, string type, string provider, string unit, decimal price, int quantity, DateTime lastDelivery, bool discount)
        {
            Article = article;
            Name = name;
            Type = type;
            Provider = provider;
            Unit = unit;
            Price = price;
            Quantity = quantity;
            LastDelivery = lastDelivery;
            IsEligibleForDiscount = discount;
        }

        public void ApplyDiscount(decimal percentage)
        {
            Price = Math.Round(Price * (1 - percentage / 100), 2);
        }

        public void AddStock(int qty)
        {
            Quantity += qty;
            LastDelivery = DateTime.Now;
        }

        public bool TrySell(int qty)
        {
            if (Quantity >= qty)
            {
                Quantity -= qty;
                return true;
            }
            return false;
        }

        public Product CloneForCart(int qty)
        {
            return new Product(
                this.Article, 
                this.Name, 
                this.Type, 
                this.Provider, 
                this.Unit, 
                this.Price, 
                qty,
                this.LastDelivery, 
                this.IsEligibleForDiscount
            );
        }
    }
}