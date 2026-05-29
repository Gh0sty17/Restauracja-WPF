using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Windows;

namespace Restauracja
{
    public partial class App : Application
    {
        public static User LoggedInUser { get; set; }
        public static List<Table> Tables { get; set; } = new List<Table>();
        public static Dictionary<string, List<MenuItem>> Menu { get; set; } = new Dictionary<string, List<MenuItem>>();
        public static List<Reservation> Reservations { get; set; } = new List<Reservation>();

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            Tables.Clear();
            for (int i = 1; i <= 9; i++)
            {
                Tables.Add(new Table { Number = i, Status = TableStatus.Wolny });
            }

            InitMockData();

            LoginWindow loginWindow = new LoginWindow();
            loginWindow.Show();
        }

        private void InitMockData()
        {
            var defaultUsers = new List<User>
            {
                new User { Imie="Jan", Nazwisko="Kowalski", Login="admin", Haslo="admin", Rola="Manager" },
                new User { Imie="Anna", Nazwisko="Nowak", Login="kelner", Haslo="1234", Rola="Kelner" }
            };
            File.WriteAllText("users.json", JsonSerializer.Serialize(defaultUsers));

            // Generowanie pełnego zestawu dań dla każdej z kategorii
            var defaultMenu = new Dictionary<string, List<MenuItem>>
            {
                { "Przystawki", new List<MenuItem> {
                    new MenuItem { Id="P1", Nazwa="Tatar wołowy", CenaBazowa=35, Kategoria="Przystawki" },
                    new MenuItem { Id="P2", Nazwa="Bruschetta", CenaBazowa=18, Kategoria="Przystawki" }
                } },
                { "Dania główne", new List<MenuItem> {
                    new MenuItem {
                        Id="D1", Nazwa="Pizza Margherita", CenaBazowa=30, Kategoria="Dania główne",
                        Modyfikatory = new List<Modifier>{ new Modifier{Nazwa="Dodatkowy ser", Doplata=5}, new Modifier{Nazwa="Salami", Doplata=6} }
                    },
                    new MenuItem { Id="D2", Nazwa="Kotlet Schabowy", CenaBazowa=28, Kategoria="Dania główne" }
                } },
                { "Napoje", new List<MenuItem> {
                    new MenuItem { Id="N1", Nazwa="Sok pomarańczowy", CenaBazowa=8, Kategoria="Napoje" },
                    new MenuItem { Id="N2", Nazwa="Kawa Espresso", CenaBazowa=10, Kategoria="Napoje" }
                } },
                { "Desery", new List<MenuItem> {
                    new MenuItem { Id="DE1", Nazwa="Sernik domowy", CenaBazowa=15, Kategoria="Desery" },
                    new MenuItem { Id="DE2", Nazwa="Puchar Lodowy", CenaBazowa=18, Kategoria="Desery" }
                } }
            };

            File.WriteAllText("menu.json", JsonSerializer.Serialize(defaultMenu));
            Menu = defaultMenu;
        }
    }
}