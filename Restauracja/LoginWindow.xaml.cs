using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Windows;
using System.Windows.Controls;

namespace Restauracja
{
    public partial class LoginWindow : Window
    {
        public LoginWindow() => InitializeComponent();

        private void TogglePanels_Click(object sender, RoutedEventArgs e)
        {
            if (LoginPanel.Visibility == Visibility.Visible)
            {
                LoginPanel.Visibility = Visibility.Collapsed;
                RegisterPanel.Visibility = Visibility.Visible;
            }
            else
            {
                LoginPanel.Visibility = Visibility.Visible;
                RegisterPanel.Visibility = Visibility.Collapsed;
            }
        }

        private void BtnLogin_Click(object sender, RoutedEventArgs e)
        {
            if (!File.Exists("users.json")) return;
            var users = JsonSerializer.Deserialize<List<User>>(File.ReadAllText("users.json"));

            var user = users.Find(u => u.Login == txtLogin.Text && u.Haslo == txtPassword.Password);
            if (user != null)
            {
                App.LoggedInUser = user;
                MainWindow mainWindow = new MainWindow();
                mainWindow.Show();
                this.Close();
            }
            else
            {
                MessageBox.Show("Błędne dane logowania!", "Błąd");
            }
        }

        private void BtnRegister_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(regLogin.Text) || string.IsNullOrWhiteSpace(regHaslo.Text))
            {
                MessageBox.Show("Wprowadź login i hasło!");
                return;
            }

            var users = JsonSerializer.Deserialize<List<User>>(File.ReadAllText("users.json")) ?? new List<User>();

            users.Add(new User
            {
                Imie = regImie.Text,
                Nazwisko = regNazwisko.Text,
                Login = regLogin.Text,
                Haslo = regHaslo.Text,
                Rola = (regRola.SelectedItem as ComboBoxItem).Content.ToString()
            });

            File.WriteAllText("users.json", JsonSerializer.Serialize(users));
            MessageBox.Show("Konto zarejestrowane pomyślnie.");
            TogglePanels_Click(null, null);
        }
    }
}