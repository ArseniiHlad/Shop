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

        private Button btnDelivery;
        private Button btnSale;
        private Button btnUcenka;
        private Button btnInventory;

        public Form1()
        {
            this.Text = "Магазин - Кассовый Апарат";
            this.Size = new Size(800, 600);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.MaximizeBox = true;

            _products.Add(new Product { Name = "Хлеб", Unit = "шт", Price = 15.50m, Quantity = 20, LastDelivery = DateTime.Now });
            _products.Add(new Product { Name = "Молоко", Unit = "л", Price = 34.00m, Quantity = 10, LastDelivery = DateTime.Now });
            _products.Add(new Product { Name = "Колбаса", Unit = "кг", Price = 180.00m, Quantity = 5, LastDelivery = DateTime.Now.AddDays(-2) });

            dgvProducts = new DataGridView();
            dgvProducts.Location = new Point(20, 20);
            dgvProducts.Size = new Size(740, 400);
            dgvProducts.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvProducts.ReadOnly = true;
            dgvProducts.DataSource = _products;
            dgvProducts.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            
            dgvProducts.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvProducts.MultiSelect = false;
            
            this.Controls.Add(dgvProducts);

            btnDelivery = new Button();
            btnDelivery.Text = "Поступление товара";
            btnDelivery.Location = new Point(20, 440);
            btnDelivery.Size = new Size(160, 40);
            btnDelivery.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            this.Controls.Add(btnDelivery);

            btnSale = new Button();
            btnSale.Text = "Оформить продажу";
            btnSale.Location = new Point(200, 440);
            btnSale.Size = new Size(160, 40);
            btnSale.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            this.Controls.Add(btnSale);

            btnUcenka = new Button();
            btnUcenka.Text = "Уценка / Списание";
            btnUcenka.Location = new Point(380, 440);
            btnUcenka.Size = new Size(160, 40);
            btnUcenka.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            btnUcenka.Click += BtnUcenka_Click;
            this.Controls.Add(btnUcenka);

            btnInventory = new Button();
            btnInventory.Text = "Инвентаризация";
            btnInventory.Location = new Point(560, 440);
            btnInventory.Size = new Size(200, 40);
            btnInventory.BackColor = Color.LightGreen;
            btnInventory.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            btnInventory.Click += BtnInventory_Click;
            this.Controls.Add(btnInventory);
        }

        private void BtnInventory_Click(object sender, EventArgs e)
        {
            decimal totalSum = 0;
            int totalItems = 0;

            foreach (var prod in _products)
            {
                totalSum += prod.Price * prod.Quantity;
                totalItems += prod.Quantity;
            }

            MessageBox.Show($"Всего товаров на складе: {totalItems} шт.\nОбщая стоимость: {totalSum:C}", "Отчет инвентаризации");
        }

        private void BtnUcenka_Click(object sender, EventArgs e)
        {
            if (dgvProducts.SelectedRows.Count > 0)
            {
                var selectedProduct = dgvProducts.SelectedRows[0].DataBoundItem as Product;
                if (selectedProduct != null)
                {
                    selectedProduct.Price = Math.Round(selectedProduct.Price * 0.85m, 2);
                    dgvProducts.Refresh();
                    MessageBox.Show($"Товар {selectedProduct.Name} уценен на 15%!", "Успех");
                }
            }
            else
            {
                MessageBox.Show("Выберите товар в таблице!", "Внимание");
            }
        }
    }
}