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

        // Объявляем кнопки управления
        private Button btnDelivery;
        private Button btnSale;
        private Button btnUcenka;
        private Button btnInventory;

        public Form1()
        {
            // Настройки главного окна
            this.Text = "Магазин - Кассовый Апарат";
            this.Size = new Size(800, 600);
            this.StartPosition = FormStartPosition.CenterScreen;

            // Тестовые данные
            _products.Add(new Product { Name = "Хлеб", Unit = "шт", Price = 15.50m, Quantity = 20, LastDelivery = DateTime.Now });
            _products.Add(new Product { Name = "Молоко", Unit = "л", Price = 34.00m, Quantity = 10, LastDelivery = DateTime.Now });
            _products.Add(new Product { Name = "Колбаса", Unit = "кг", Price = 180.00m, Quantity = 5, LastDelivery = DateTime.Now.AddDays(-2) });

            // Таблица
            dgvProducts = new DataGridView();
            dgvProducts.Location = new Point(20, 20);
            dgvProducts.Size = new Size(740, 400);
            dgvProducts.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvProducts.ReadOnly = true;
            dgvProducts.DataSource = _products;
            this.Controls.Add(dgvProducts);

            // 1. Кнопка "Поступление товара"
            btnDelivery = new Button();
            btnDelivery.Text = "Поступление товара";
            btnDelivery.Location = new Point(20, 440);
            btnDelivery.Size = new Size(160, 40);
            this.Controls.Add(btnDelivery);

            // 2. Кнопка "Оформить продажу"
            btnSale = new Button();
            btnSale.Text = "Оформить продажу";
            btnSale.Location = new Point(200, 440);
            btnSale.Size = new Size(160, 40);
            this.Controls.Add(btnSale);

            // 3. Кнопка "Уценка и списание"
            btnUcenka = new Button();
            btnUcenka.Text = "Уценка / Списание";
            btnUcenka.Location = new Point(380, 440);
            btnUcenka.Size = new Size(160, 40);
            this.Controls.Add(btnUcenka);

            // 4. Кнопка "Инвентаризация"
            btnInventory = new Button();
            btnInventory.Text = "Инвентаризация";
            btnInventory.Location = new Point(560, 440);
            btnInventory.Size = new Size(200, 40); // Сделаем чуть шире для солидности
            btnInventory.BackColor = Color.LightGreen; // Выделим цветом главную кнопку отчета
            this.Controls.Add(btnInventory);
        }
    }
}