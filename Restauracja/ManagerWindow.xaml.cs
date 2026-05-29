using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Windows;
using System.Windows.Controls;

namespace Restauracja
{
    public partial class ManagerWindow : Window
    {
        private decimal _todayTotal = 0;

        public ManagerWindow()
        {
            InitializeComponent();
            CalculateTodaySales();
            LoadAllProducts();

            if (File.Exists("users.json"))
                dgUsers.ItemsSource = JsonSerializer.Deserialize<List<User>>(File.ReadAllText("users.json"));
        }

        private void CalculateTodaySales()
        {
            _todayTotal = 0;
            string dzis = DateTime.Now.ToString("yyyy-MM-dd");

            if (File.Exists("sales_history.log"))
            {
                var linie = File.ReadAllLines("sales_history.log");
                foreach (var linia in linie)
                {
                    var czesci = linia.Split(';');
                    if (czesci.Length == 2 && czesci[0] == dzis)
                    {
                        if (decimal.TryParse(czesci[1], out decimal kwota))
                            _todayTotal += kwota;
                    }
                }
            }
            lblDailySum.Text = $"Suma dzisiejszych zamówień: {_todayTotal:F2} zł";
        }

        private void LoadAllProducts()
        {
            List<MenuItem> allItems = new List<MenuItem>();
            foreach (var kvp in App.Menu)
            {
                allItems.AddRange(kvp.Value);
            }
            listAllProducts.ItemsSource = allItems;
        }

        private void BtnExportCSV_Click(object sender, RoutedEventArgs e)
        {
            string dzis = DateTime.Now.ToString("yyyy-MM-dd");
            string csvFile = $"Raport_{dzis}.csv";
            string content = $"Data;Suma Sprzedazy\n{dzis};{_todayTotal:F2}";

            File.WriteAllText(csvFile, content, Encoding.UTF8);
            MessageBox.Show($"Raport zapisany jako: {csvFile}");
        }

        private void BtnAddProduct_Click(object sender, RoutedEventArgs e)
        {
            string kategoria = (cmbAddCat.SelectedItem as ComboBoxItem).Content.ToString();
            if (string.IsNullOrWhiteSpace(txtAddName.Text) || !decimal.TryParse(txtAddPrice.Text, out decimal cena))
            {
                MessageBox.Show("Podaj poprawną nazwę i cenę!");
                return;
            }

            var nowy = new MenuItem
            {
                Id = Guid.NewGuid().ToString().Substring(0, 5),
                Nazwa = txtAddName.Text,
                CenaBazowa = cena,
                Kategoria = kategoria
            };

            App.Menu[kategoria].Add(nowy);
            File.WriteAllText("menu.json", JsonSerializer.Serialize(App.Menu));

            txtAddName.Clear();
            txtAddPrice.Clear();
            LoadAllProducts();
            MessageBox.Show("Produkt został dodany.");
        }

        private void BtnDeleteProduct_Click(object sender, RoutedEventArgs e)
        {
            if (listAllProducts.SelectedItem is MenuItem wybrany)
            {
                foreach (var kat in App.Menu.Keys)
                {
                    if (App.Menu[kat].Remove(wybrany)) break;
                }

                File.WriteAllText("menu.json", JsonSerializer.Serialize(App.Menu));
                LoadAllProducts();
                MessageBox.Show("Produkt został usunięty.");
            }
        }
    }
}