using System;
using System.Windows;

namespace Restauracja
{
    public partial class ReservationWindow : Window
    {
        private int _tableNum;
        public Reservation ResultReservation { get; private set; }

        public ReservationWindow(int tableNumber)
        {
            InitializeComponent();
            _tableNum = tableNumber;
            lblTitle.Text = $"Rezerwacja stolika nr {tableNumber}";
            txtDate.Text = DateTime.Now.ToString("yyyy-MM-dd");
        }

        private void BtnConfirm_Click(object sender, RoutedEventArgs e)
        {
            ResultReservation = new Reservation
            {
                NumerStolika = _tableNum,
                Data = txtDate.Text,
                Godzina = txtTime.Text,
                NazwiskoGoscia = txtGuest.Text
            };
            this.DialogResult = true;
            this.Close();
        }
    }
}