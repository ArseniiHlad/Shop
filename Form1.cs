using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using Kursach.Models;

namespace Kursach
{
    public partial class Form1 : Form
    {
        private List<Product> _products = null!;
        private List<Product> _cart = null!; 
        private BindingSource _bindingSource = null!;
        private Dictionary<string, bool> _sortDirections = null!;
        private Random _random = new Random();

        private DataGridView dgvProducts = null!;
        private TextBox txtSearch = null!;
        private Label lblSearch = null!;
        private Button btnAddNewProduct = null!;
        private Button btnSellProduct = null!;
        private Button btnDiscountOrDelete = null!; 
        private Button btnInventory = null!;        

        public Form1()
        {
            InitializeComponent();
            LoadInitialData();
            InitializeAdvancedLogic();
        }

        private void LoadInitialData()
        {
            _products = new List<Product>
            {
                new Product("ART-1054", "Хліб", "Їжа", "Хлібзавод №3", "шт", 24.50m, 50, DateTime.Now, false),
                new Product("ART-8841", "Молоко", "Їжа", "МолЗавод Харків", "пак", 38.00m, 20, DateTime.Now, false),
                new Product("ART-3129", "Пральний порошок", "Хімія", "Procter&Gamble", "шт", 145.00m, 10, DateTime.Now, false)
            };
            _cart = new List<Product>();
        }

        private void InitializeAdvancedLogic()
        {
            _sortDirections = new Dictionary<string, bool>();
            _bindingSource = new BindingSource();

            _bindingSource.DataSource = _products;
            dgvProducts.DataSource = _bindingSource;

            dgvProducts.Columns["Article"].HeaderText = "Артикул";
            dgvProducts.Columns["Name"].HeaderText = "Назва товару";
            dgvProducts.Columns["Price"].HeaderText = "Ціна (грн)";
            dgvProducts.Columns["Type"].HeaderText = "Категорія";
            dgvProducts.Columns["Provider"].HeaderText = "Постачальник";
            dgvProducts.Columns["Quantity"].HeaderText = "Кількість";
            dgvProducts.Columns["Unit"].HeaderText = "Од. вим.";
            
            if (dgvProducts.Columns["LastDelivery"] != null) 
                dgvProducts.Columns["LastDelivery"]!.HeaderText = "Остання поставка";
            
            if (dgvProducts.Columns["IsEligibleForDiscount"] != null)
                dgvProducts.Columns["IsEligibleForDiscount"]!.Visible = false;

            txtSearch.TextChanged += TxtSearch_TextChanged;
            dgvProducts.ColumnHeaderMouseClick += dgvProducts_ColumnHeaderMouseClick;
            btnAddNewProduct.Click += BtnAddNewProduct_Click;
            btnSellProduct.Click += BtnSellProduct_Click;
            btnDiscountOrDelete.Click += BtnDiscountOrDelete_Click;
            btnInventory.Click += BtnInventory_Click;
        }

        private void TxtSearch_TextChanged(object? sender, EventArgs e)
        {
            string query = txtSearch.Text.Trim().ToLower();
            if (string.IsNullOrEmpty(query))
            {
                _bindingSource.DataSource = _products;
            }
            else
            {
                _bindingSource.DataSource = _products.Where(p =>
                    (p.Name != null && p.Name.ToLower().Contains(query)) || 
                    (p.Article != null && p.Article.ToLower().Contains(query)) ||
                    (p.Type != null && p.Type.ToLower().Contains(query)) || 
                    (p.Provider != null && p.Provider.ToLower().Contains(query))
                ).ToList();
            }
            _bindingSource.ResetBindings(false);
        }

        private void BtnAddNewProduct_Click(object? sender, EventArgs e)
        {
            string name = PromptDialog.ShowDialog("Введіть назву нового товару:", "Додавання");
            if (string.IsNullOrEmpty(name)) return;

            string priceInput = PromptDialog.ShowDialog("Введіть ціну товару (грн):", "Ціна");
            if (!decimal.TryParse(priceInput, out decimal price) || price <= 0)
            {
                MessageBox.Show("Помилка введення! Ціна товару повинна бути числовим значенням більшим за нуль", "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            string qtyInput = PromptDialog.ShowDialog("Введіть початкову кількість:", "Кількість");
            if (!int.TryParse(qtyInput, out int qty) || qty < 0) qty = 0;

            string articleInput = PromptDialog.ShowDialog("Введіть артикул (залиште порожнім для автогенерації):", "Артикул");
            string finalArticle = string.IsNullOrWhiteSpace(articleInput) ? "ART-" + _random.Next(1000, 9999) : articleInput.Trim().ToUpper();
            
            string type = PromptDialog.ShowDialog("Введіть категорію (Їжа, Хімія):", "Категорія") ?? "Інше";
            string provider = PromptDialog.ShowDialog("Введіть постачальника:", "Постачальник") ?? "Невідомий";
            string unit = PromptDialog.ShowDialog("Введіть одиниці виміру (шт, кг, пак):", "Од. вим.") ?? "шт";

            _products.Add(new Product(finalArticle, name, type, provider, unit, price, qty, DateTime.Now, false));
            TxtSearch_TextChanged(null, EventArgs.Empty);
            MessageBox.Show("Товар успішно додано!", "Успіх");
        }

        private void BtnSellProduct_Click(object? sender, EventArgs e)
        {
            if (dgvProducts.SelectedRows.Count == 0)
            {
                MessageBox.Show("Будь ласка, виберіть товар у таблиці.", "Увага", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var selectedProduct = dgvProducts.SelectedRows[0].DataBoundItem as Product;
            if (selectedProduct == null) return;

            string qtyInput = PromptDialog.ShowDialog($"Введіть кількість для '{selectedProduct.Name}' (Доступно: {selectedProduct.Quantity}):", "Оформлення покупки");
            if (!int.TryParse(qtyInput, out int quantity) || quantity <= 0)
            {
                MessageBox.Show("Некоректна кількість!", "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            int alreadyInCart = _cart.Where(x => x.Article == selectedProduct.Article).Sum(x => x.Quantity);
            if (selectedProduct.Quantity < (quantity + alreadyInCart))
            {
                MessageBox.Show("Недостатньо товару на складі з урахуванням кошика!", "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var cartItem = selectedProduct.CloneForCart(quantity);
            _cart.Add(cartItem);

            DialogResult result = MessageBox.Show("Товар додано в чек. Бажаєте додати ще один товар? (Натисніть 'Так' для продовження обирання товару, 'Ні' для формування чека)", "Кошик", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (result == DialogResult.No)
            {
                decimal totalCheckSum = 0;
                
                try
                {
                    using (StreamWriter sw = new StreamWriter("check.txt", true))
                    {
                        sw.WriteLine($"--- ФІСКАЛЬНИЙ ЧЕК --- {DateTime.Now}");
                        foreach (var item in _cart)
                        {
                            var realProd = _products.FirstOrDefault(p => p.Article == item.Article);
                            if (realProd != null)
                            {
                                realProd.TrySell(item.Quantity);
                            }

                            decimal itemSum = item.Price * item.Quantity;
                            totalCheckSum += itemSum;

                            sw.WriteLine($"Товар: {item.Name} | {item.Quantity} {item.Unit} x {item.Price} грн = {itemSum} грн.");
                        }
                        sw.WriteLine($"ЗАГАЛЬНА СУМА: {totalCheckSum} грн.");
                        sw.WriteLine("---------------------------------------\n");
                    }

                    MessageBox.Show($"Продаж оформлено! Чек збережено у файл \"check.txt\". Загальна сума: {totalCheckSum} грн", "Продаж", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Помилка запису чеку: {ex.Message}", "Помилка I/O", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

                _cart.Clear(); 
                _bindingSource.ResetBindings(false);
            }
        }

        private void BtnDiscountOrDelete_Click(object? sender, EventArgs e)
        {
            if (dgvProducts.SelectedRows.Count == 0)
            {
                MessageBox.Show("Виберіть товар для проведення уцінки або списання.", "Увага", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var selectedProduct = dgvProducts.SelectedRows[0].DataBoundItem as Product;
            if (selectedProduct == null) return;

            DialogResult result = MessageBox.Show($"Ви бажаєте уцінити товар на 15 відсотків? (натисніть 'Так' для уцінки, 'Ні' для повного списання товару)", "Уцінка / Списання", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Warning);

            if (result == DialogResult.Yes)
            {
                selectedProduct.ApplyDiscount(15);
                _bindingSource.ResetBindings(false);
                MessageBox.Show($"Ціну товару '{selectedProduct.Name}' успішно знижено на 15%!", "Уцінка");
            }
            else if (result == DialogResult.No)
            {
                _products.Remove(selectedProduct);
                TxtSearch_TextChanged(null, EventArgs.Empty); 
                MessageBox.Show("Товар повністю списано (видалено з бази).", "Списання");
            }
        }

        private void BtnInventory_Click(object? sender, EventArgs e)
        {
            int totalQuantity = _products.Sum(p => p.Quantity);
            decimal totalValue = _products.Sum(p => p.Quantity * p.Price);

            string report = $"--- ЗВІТ ІНВЕНТАРИЗАЦІЇ СКЛАДУ ---\n\n" +
                            $"Загальна кількість позицій: {_products.Count}\n" +
                            $"Сумарна кількість одиниць товару: {totalQuantity} шт/пак\n" +
                            $"Загальна вартість усіх товарів на складі: {totalValue} грн.\n" +
                            $"----------------------------------------";

            MessageBox.Show(report, "Інвентаризація", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void dgvProducts_ColumnHeaderMouseClick(object? sender, DataGridViewCellMouseEventArgs e)
        {
            var dataSourceList = _bindingSource.DataSource as List<Product>;
            if (dataSourceList == null) return;

            string columnName = dgvProducts.Columns[e.ColumnIndex].DataPropertyName;
            if (string.IsNullOrEmpty(columnName)) return;

            if (!_sortDirections.ContainsKey(columnName)) _sortDirections[columnName] = true;
            else _sortDirections[columnName] = !_sortDirections[columnName];

            bool isAscending = _sortDirections[columnName];

            PropertyInfo? propertyInfo = typeof(Product).GetProperty(columnName);
            if (propertyInfo == null) return;

            if (isAscending) dataSourceList = dataSourceList.OrderBy(p => propertyInfo.GetValue(p, null)).ToList();
            else dataSourceList = dataSourceList.OrderByDescending(p => propertyInfo.GetValue(p, null)).ToList();

            _bindingSource.DataSource = dataSourceList;
            _bindingSource.ResetBindings(false);
        }

        private void InitializeComponent()
        {
            this.dgvProducts = new System.Windows.Forms.DataGridView();
            this.txtSearch = new System.Windows.Forms.TextBox();
            this.lblSearch = new System.Windows.Forms.Label();
            this.btnAddNewProduct = new System.Windows.Forms.Button();
            this.btnSellProduct = new System.Windows.Forms.Button();
            this.btnDiscountOrDelete = new System.Windows.Forms.Button();
            this.btnInventory = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.dgvProducts)).BeginInit();
            this.SuspendLayout();

            this.dgvProducts.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvProducts.Location = new System.Drawing.Point(12, 50);
            this.dgvProducts.Name = "dgvProducts";
            this.dgvProducts.Size = new System.Drawing.Size(760, 240);
            this.dgvProducts.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvProducts.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            this.dgvProducts.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
                | System.Windows.Forms.AnchorStyles.Left) 
                | System.Windows.Forms.AnchorStyles.Right)));

            this.txtSearch.Location = new System.Drawing.Point(120, 15);
            this.txtSearch.Name = "txtSearch";
            this.txtSearch.Size = new System.Drawing.Size(250, 20);

            this.lblSearch.Location = new System.Drawing.Point(12, 18);
            this.lblSearch.Name = "lblSearch";
            this.lblSearch.Size = new System.Drawing.Size(110, 20);
            this.lblSearch.Text = "Швидкий пошук:";

            this.btnAddNewProduct.Location = new System.Drawing.Point(12, 305);
            this.btnAddNewProduct.Name = "btnAddNewProduct";
            this.btnAddNewProduct.Size = new System.Drawing.Size(160, 35);
            this.btnAddNewProduct.Text = "Додати новий товар";
            this.btnAddNewProduct.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));

            this.btnSellProduct.Location = new System.Drawing.Point(185, 305);
            this.btnSellProduct.Name = "btnSellProduct";
            this.btnSellProduct.Size = new System.Drawing.Size(160, 35);
            this.btnSellProduct.Text = "Оформити продаж";
            this.btnSellProduct.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));

            this.btnDiscountOrDelete.Location = new System.Drawing.Point(360, 305);
            this.btnDiscountOrDelete.Name = "btnDiscountOrDelete";
            this.btnDiscountOrDelete.Size = new System.Drawing.Size(160, 35);
            this.btnDiscountOrDelete.Text = "Уцінка / Списання";
            this.btnDiscountOrDelete.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));

            this.btnInventory.Location = new System.Drawing.Point(535, 305);
            this.btnInventory.Name = "btnInventory";
            this.btnInventory.Size = new System.Drawing.Size(160, 35);
            this.btnInventory.Text = "Інвентаризація";
            this.btnInventory.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));

            this.ClientSize = new System.Drawing.Size(784, 355);
            this.MinimumSize = new System.Drawing.Size(740, 350);
            this.Controls.Add(this.btnInventory);
            this.Controls.Add(this.btnDiscountOrDelete);
            this.Controls.Add(this.btnSellProduct);
            this.Controls.Add(this.btnAddNewProduct);
            this.Controls.Add(this.lblSearch);
            this.Controls.Add(this.txtSearch);
            this.Controls.Add(this.dgvProducts);
            this.Name = "Form1";
            this.Text = "Магазин - Касовий апарат";
            ((System.ComponentModel.ISupportInitialize)(this.dgvProducts)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();
        }
    }
}