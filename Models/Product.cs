using System;

namespace Kursach.Models
{
    public class Product
    {
        public string Name { get; private set; }
        public string Unit { get; private set; }
        public decimal Price { get; private set; }
        public int Quantity { get; private set; }
        public DateTime LastDelivery { get; private set; }

        public Product(string name, string unit, decimal price, int quantity, DateTime lastDelivery)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            Unit = unit ?? throw new ArgumentNullException(nameof(unit));
            Price = price >= 0 ? price : throw new ArgumentException("Цена не может быть отрицательной");
            Quantity = quantity >= 0 ? quantity : throw new ArgumentException("Количество не может быть отрицательным");
            LastDelivery = lastDelivery;
        }

        public void ApplyDiscount(decimal percentage)
        {
            if (percentage <= 0 || percentage >= 100) return;
            Price = Math.Round(Price * (1 - percentage / 100m), 2);
        }

        public bool TrySell(int qty)
        {
            if (qty <= 0 || qty > Quantity) return false;
            Quantity -= qty;
            return true;
        }

        public void AddStock(int qty)
        {
            if (qty <= 0) return;
            Quantity += qty;
            LastDelivery = DateTime.Now;
        }

        public Product CloneForCart(int qty)
        {
            return new Product(Name, Unit, Price, qty, LastDelivery);
        }
    }
}