using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace Restauracja
{
    public partial class OrderWindow : Window
    {
        private Table _currentTable;
        private List<OrderItem> _tempBasket = new List<OrderItem>();

        public OrderWindow(Table table)
        {
            InitializeComponent();
            _currentTable = table;
            this.Title = $"Zamówienie - Stolik {table.Number}";

            
            comboCategories.ItemsSource = App.Menu.Keys.ToList();

           
            if (comboCategories.Items.Count > 0)
            {
                comboCategories.SelectedIndex = 0;
            }

            _tempBasket = table.Koszyk.ToList();
            txtNotes.Text = table.UwagiDoKuchni;

            RefreshBasket();
        }

        private void ComboCategories_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (comboCategories.SelectedItem is string cat)
            {
                if (App.Menu.ContainsKey(cat))
                {
                    listProducts.ItemsSource = null;
                    listProducts.ItemsSource = App.Menu[cat];
                    listProducts.UpdateLayout();
                }
            }
        }

        private void ListProducts_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (listProducts.SelectedItem is MenuItem prod)
                listModifiers.ItemsSource = prod.Modyfikatory;
        }

        private void BtnAddToBasket_Click(object sender, RoutedEventArgs e)
        {
            if (listProducts.SelectedItem is MenuItem prod)
            {
                decimal cenaMod = 0;
                List<string> wybraneNazwy = new List<string>();

                if (prod.Modyfikatory != null)
                {
                    foreach (Modifier m in prod.Modyfikatory)
                    {
                        if (m.IsSelected)
                        {
                            cenaMod += m.Doplata;
                            wybraneNazwy.Add(m.Nazwa);
                            m.IsSelected = false;
                        }
                    }
                }

                _tempBasket.Add(new OrderItem
                {
                    Nazwa = prod.Nazwa,
                    CenaKoncowa = prod.CenaBazowa + cenaMod,
                    Ilosc = 1,
                    WybraneModyfikatory = wybraneNazwy
                });

                RefreshBasket();
                listModifiers.ItemsSource = null;
            }
        }

        private void RefreshBasket()
        {
            listBasket.ItemsSource = null;
            listBasket.ItemsSource = _tempBasket;

            decimal brutto = _tempBasket.Sum(i => i.SumaPozycji);
            decimal netto = decimal.Round(brutto / 1.08m, 2);
            decimal vat = brutto - netto;

            lblBrutto.Text = $"Suma Brutto: {brutto:F2} zł";
            lblNetto.Text = $"Suma Netto: {netto:F2} zł";
            lblVat.Text = $"VAT (8%): {vat:F2} zł";
        }

        private void BtnRemoveItem_Click(object sender, RoutedEventArgs e)
        {
            var item = (sender as FrameworkElement).DataContext as OrderItem;
            _tempBasket.Remove(item);
            RefreshBasket();
        }

        private void BtnInc_Click(object sender, RoutedEventArgs e)
        {
            var item = (sender as FrameworkElement).DataContext as OrderItem;
            item.Ilosc++;
            RefreshBasket();
        }

        private void BtnDec_Click(object sender, RoutedEventArgs e)
        {
            var item = (sender as FrameworkElement).DataContext as OrderItem;
            if (item.Ilosc > 1) item.Ilosc--;
            RefreshBasket();
        }

        private void BtnSaveSession_Click(object sender, RoutedEventArgs e)
        {
            _currentTable.Koszyk = _tempBasket;
            _currentTable.UwagiDoKuchni = txtNotes.Text;
            this.Close();
        }

        private void BtnPay_Click(object sender, RoutedEventArgs e)
        {
            if (!_tempBasket.Any()) { MessageBox.Show("Koszyk jest pusty!"); return; }

            var płatnośćResult = MessageBox.Show("Wybierz TAK dla płatności KARTĄ, NIE dla GOTÓWKI, ANULUJ dla BLIK",
                "Metoda Płatności", MessageBoxButton.YesNoCancel);

            string wybranaMetoda = płatnośćResult switch
            {
                MessageBoxResult.Yes => "Karta",
                MessageBoxResult.No => "Gotówka",
                _ => "Blik"
            };

            decimal brutto = _tempBasket.Sum(i => i.SumaPozycji);
            string fileReceipt = $"paragon_Stolik{_currentTable.Number}_{DateTime.Now:yyyyMMdd_HHmmss}.txt";

            using (StreamWriter sw = new StreamWriter(fileReceipt))
            {
                sw.WriteLine("========================================");
                sw.WriteLine("           PARAGON FISKALNY");
                sw.WriteLine("========================================");
                sw.WriteLine($"Data: {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
                sw.WriteLine($"Stolik: {_currentTable.Number}");
                sw.WriteLine($"Obsługa: {App.LoggedInUser.Imie} {App.LoggedInUser.Nazwisko}");
                sw.WriteLine("----------------------------------------");
                foreach (var item in _tempBasket)
                {
                    sw.WriteLine($"{item.Nazwa} x{item.Ilosc} - {item.SumaPozycji:F2} zł");
                }
                sw.WriteLine("----------------------------------------");
                sw.WriteLine($"RAZEM BRUTTO: {brutto:F2} zł");
                sw.WriteLine($"Metoda płatności: {wybranaMetoda}");
                sw.WriteLine("========================================");
            }

            File.AppendAllText("sales_history.log", $"{DateTime.Now:yyyy-MM-dd};{brutto}\n");

            _currentTable.Koszyk.Clear();
            _currentTable.UwagiDoKuchni = "";
            _currentTable.Status = TableStatus.Wolny;
            _currentTable.Rezerwacja = null;

            MessageBox.Show($"Zapisano paragon do pliku: {fileReceipt}");
            this.Close();
        }
    }
}
