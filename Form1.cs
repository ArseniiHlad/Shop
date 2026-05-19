using System;
using System.Windows.Forms;
using System.Drawing;
using System.Collections.Generic;
using Kursach.Models;

namespace Kursach
{
    public class Form1 : Form
    {
        private List<Product> _products = new List<Product>();
        
        private DataGridView dgvProducts;

        public Form1()
        {
            this.Text = "Магазин - Кассовый Аппарат";
            this.Size = new Size(800, 600);
            this.StartPosition = FormStartPosition.CenterScreen;

            _products.Add(new Product { Name = "Хлеб", Unit = "шт", Price = 15.50m, Quantity = 20, LastDelivery = DateTime.Now });
            _products.Add(new Product { Name = "Молоко", Unit = "л", Price = 34.00m, Quantity = 10, LastDelivery = DateTime.Now });
            _products.Add(new Product { Name = "Колбаса", Unit = "кг", Price = 180.00m, Quantity = 5, LastDelivery = DateTime.Now.AddDays(-2) });

            dgvProducts = new DataGridView();
            dgvProducts.Location = new Point(20, 20);
            dgvProducts.Size = new Size(740, 400);
            dgvProducts.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvProducts.ReadOnly = true;

            dgvProducts.DataSource = _products;

            this.Controls.Add(dgvProducts);
        }
    }
}