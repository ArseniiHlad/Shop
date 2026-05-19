using System;

namespace Kursach.Models
{
    public class Product
    {
        public string Name { get; set; }          // Наименование
        public string Unit { get; set; }          // Единица измерения (шт, кг, л)
        public decimal Price { get; set; }        // Цена единицы
        public int Quantity { get; set; }         // Количество
        public DateTime LastDelivery { get; set; } // Дата последнего завоза
    }
}