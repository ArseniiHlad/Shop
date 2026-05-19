using System;
using System.Windows.Forms;
using System.Drawing;
using System.Data;

namespace Kursach
{
    public class Form1 : Form
    {
        private DataTable _productsTable = new DataTable();
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

            _productsTable.Columns.Add("Наименование", typeof(string));
            _productsTable.Columns.Add("Ед. измерения", typeof(string));
            _productsTable.Columns.Add("Цена за ед.", typeof(decimal));
            _productsTable.Columns.Add("Количество", typeof(int));
            _productsTable.Columns.Add("Дата закупки", typeof(DateTime));

            _productsTable.Rows.Add("Хлеб", "шт", 15.50m, 20, DateTime.Now);
            _productsTable.Rows.Add("Молоко", "л", 34.00m, 10, DateTime.Now);
            _productsTable.Rows.Add("Колбаса", "кг", 180.00m, 5, DateTime.Now.AddDays(-2));

            dgvProducts = new DataGridView();
            dgvProducts.Location = new Point(20, 20);
            dgvProducts.Size = new Size(740, 400);
            dgvProducts.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvProducts.ReadOnly = true;
            dgvProducts.DataSource = _productsTable;
            dgvProducts.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
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
            this.Controls.Add(btnUcenka);

            btnInventory = new Button();
            btnInventory.Text = "Инвентаризация";
            btnInventory.Location = new Point(560, 440);
            btnInventory.Size = new Size(200, 40);
            btnInventory.BackColor = Color.LightGreen;
            btnInventory.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            this.Controls.Add(btnInventory);
        }
    }
}