using System;
using System.Windows.Forms;
using System.Drawing;
using System.Collections.Generic;
using System.IO;
using Kursach.Models;

namespace Kursach
{
    public class Form1 : Form
    {
        private List<Product> _products = new List<Product>();
        private List<Product> _cart = new List<Product>();
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

            // Теперь создаем объекты через каноничный конструктор
            _products.Add(new Product("Хлеб", "шт", 15.50m, 20, DateTime.Now));
            _products.Add(new Product("Молоко", "л", 34.00m, 10, DateTime.Now));
            _products.Add(new Product("Колбаса", "кг", 180.00m, 5, DateTime.Now.AddDays(-2)));

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
            btnDelivery.Click += BtnDelivery_Click;
            this.Controls.Add(btnDelivery);

            btnSale = new Button();
            btnSale.Text = "Оформить продажу";
            btnSale.Location = new Point(200, 440);
            btnSale.Size = new Size(160, 40);
            btnSale.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            btnSale.Click += BtnSale_Click;
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
                    DialogResult result = MessageBox.Show(
                        $"Вы хотите уценить товар на 15%?\n(Нажмите 'Да' для уценки, 'Нет' для полного списания товара)", 
                        "Выбор действия", 
                        MessageBoxButtons.YesNoCancel
                    );

                    if (result == DialogResult.Yes)
                    {
                        // Используем метод объекта вместо ручного изменения свойства
                        selectedProduct.ApplyDiscount(15);
                        dgvProducts.Refresh();
                    }
                    else if (result == DialogResult.No)
                    {
                        _products.Remove(selectedProduct);
                        dgvProducts.DataSource = null;
                        dgvProducts.DataSource = _products;
                    }
                }
            }
            else
            {
                MessageBox.Show("Выберите товар в таблице!", "Внимание");
            }
        }

        private void BtnDelivery_Click(object sender, EventArgs e)
        {
            string name = PromptDialog.ShowDialog("Введите наименование товара:", "Поступление");
            if (string.IsNullOrWhiteSpace(name)) return;

            string qtyStr = PromptDialog.ShowDialog("Введите количество:", "Поступление");
            if (!int.TryParse(qtyStr, out int qty) || qty <= 0) return;

            var existingProduct = _products.Find(p => p.Name != null && p.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
            if (existingProduct != null)
            {
                // Используем метод объекта
                existingProduct.AddStock(qty);
            }
            else
            {
                string unit = PromptDialog.ShowDialog("Введите единицу измерения (шт/л/кг):", "Новый товар");
                string priceStr = PromptDialog.ShowDialog("Введите цену:", "Новый товар");
                if (!decimal.TryParse(priceStr, out decimal price) || price <= 0) return;

                // Используем конструктор
                _products.Add(new Product(name, unit, price, qty, DateTime.Now));
            }

            dgvProducts.DataSource = null;
            dgvProducts.DataSource = _products;
            MessageBox.Show("Склад обновлен!", "Успех");
        }

        private void BtnSale_Click(object sender, EventArgs e)
        {
            if (dgvProducts.SelectedRows.Count > 0)
            {
                var selectedProduct = dgvProducts.SelectedRows[0].DataBoundItem as Product;
                if (selectedProduct != null)
                {
                    string qtyStr = PromptDialog.ShowDialog($"Сколько единиц товара '{selectedProduct.Name}' добавить в чек?", "Продажа");
                    if (!int.TryParse(qtyStr, out int qty) || qty <= 0) return;

                    int alreadyInCart = 0;
                    var inCartProd = _cart.Find(p => p.Name == selectedProduct.Name);
                    if (inCartProd != null) alreadyInCart = inCartProd.Quantity;

                    if (qty + alreadyInCart > selectedProduct.Quantity)
                    {
                        MessageBox.Show("Недостаточно товара на складе с учетом корзины!", "Ошибка");
                        return;
                    }

                    if (inCartProd != null)
                    {
                        inCartProd.AddStock(qty);
                    }
                    else
                    {
                        // Используем специальный метод клонирования для корзины
                        _cart.Add(selectedProduct.CloneForCart(qty));
                    }

                    DialogResult result = MessageBox.Show(
                        "Товар добавлен в чек. Хотите добавить еще один товар?\n(Нажмите 'Да' для продолжения выбора, 'Нет' для закрытия чека)", 
                        "Кассовый аппарат", 
                        MessageBoxButtons.YesNo
                    );

                    if (result == DialogResult.No)
                    {
                        decimal totalCheckPrice = 0;
                        string checkPath = "check.txt";

                        using (StreamWriter writer = new StreamWriter(checkPath, false))
                        {
                            writer.WriteLine("====== ФИСКАЛЬНЫЙ ЧЕК ======");
                            writer.WriteLine($"Дата: {DateTime.Now}");
                            writer.WriteLine("----------------------------");

                            foreach (var item in _cart)
                            {
                                var stockProd = _products.Find(p => p.Name == item.Name);
                                if (stockProd != null)
                                {
                                    // Объект сам уменьшает свое количество, проверяя валидность операции
                                    stockProd.TrySell(item.Quantity);
                                }

                                decimal itemTotal = item.Price * item.Quantity;
                                totalCheckPrice += itemTotal;

                                writer.WriteLine($"{item.Name} - {item.Quantity} {item.Unit} x {item.Price:C} = {itemTotal:C}");
                            }

                            writer.WriteLine("----------------------------");
                            writer.WriteLine($"ИТОГО К ОПЛАТЕ: {totalCheckPrice:C}");
                            writer.WriteLine("============================");
                        }

                        _cart.Clear();
                        dgvProducts.Refresh();
                        MessageBox.Show($"Продажа оформлена! Чек сохранен в файл check.txt\nОбщая сумма: {totalCheckPrice:C}", "Успешно");
                    }
                }
            }
            else
            {
                MessageBox.Show("Выберите товар в таблице для продажи!", "Внимание");
            }
        }
    }
}