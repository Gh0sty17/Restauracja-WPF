using System.Windows;
using System.Windows.Documents;
using System.Windows.Input;

namespace Restauracja
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            lblUserInfo.Text = $"Zalogowany: {App.LoggedInUser.Imie} {App.LoggedInUser.Nazwisko} ({App.LoggedInUser.Rola})";

            if (App.LoggedInUser.Rola == "Manager")
                btnManagerPanel.Visibility = Visibility.Visible;

            itemsTables.ItemsSource = App.Tables;
        }

        private void Table_LPM_Click(object sender, RoutedEventArgs e)
        {
            var btn = sender as FrameworkElement;
            var table = btn.DataContext as Table;

            if (table.Status == TableStatus.Wolny || table.Status == TableStatus.Rezerwowany)
            {
                table.Status = TableStatus.Zajety;
            }

            OrderWindow orderWindow = new OrderWindow(table);
            orderWindow.ShowDialog();
            itemsTables.Items.Refresh();
        }

        private void Table_PPM_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            var btn = sender as FrameworkElement;
            var table = btn.DataContext as Table;

            if (table.Status != TableStatus.Wolny)
            {
                MessageBox.Show("Ten stolik nie jest wolny.");
                return;
            }

            ReservationWindow resWindow = new ReservationWindow(table.Number);
            if (resWindow.ShowDialog() == true)
            {
                table.Rezerwacja = resWindow.ResultReservation;
                table.Status = TableStatus.Rezerwowany;
                App.Reservations.Add(resWindow.ResultReservation);
                itemsTables.Items.Refresh();
            }
        }

        private void BtnManagerPanel_Click(object sender, RoutedEventArgs e)
        {
            ManagerWindow mgrWindow = new ManagerWindow();
            mgrWindow.ShowDialog();
            itemsTables.Items.Refresh();
        }
    }
}