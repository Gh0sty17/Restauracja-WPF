using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Restauracja
{
    public enum TableStatus { Wolny, Zajety, Rezerwowany }

    public class User
    {
        public string Imie { get; set; }
        public string Nazwisko { get; set; }
        public string Login { get; set; }
        public string Haslo { get; set; }
        public string Rola { get; set; }
    }

    public class Modifier
    {
        public string Nazwa { get; set; }
        public decimal Doplata { get; set; }
        public bool IsSelected { get; set; }
    }

    public class MenuItem
    {
        public string Id { get; set; }
        public string Nazwa { get; set; }
        public decimal CenaBazowa { get; set; }
        public string Kategoria { get; set; }
        public List<Modifier> Modyfikatory { get; set; } = new List<Modifier>();
    }

    public class OrderItem : INotifyPropertyChanged
    {
        private int _ilosc = 1;
        public string Nazwa { get; set; }
        public decimal CenaKoncowa { get; set; }
        public int Ilosc
        {
            get => _ilosc;
            set { _ilosc = value; OnPropertyChanged(); OnPropertyChanged(nameof(SumaPozycji)); }
        }
        public decimal SumaPozycji => CenaKoncowa * Ilosc;
        public List<string> WybraneModyfikatory { get; set; } = new List<string>();

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }

    public class Reservation
    {
        public int NumerStolika { get; set; }
        public string Data { get; set; }
        public string Godzina { get; set; }
        public string NazwiskoGoscia { get; set; }
    }

    public class Table : INotifyPropertyChanged
    {
        private TableStatus _status = TableStatus.Wolny;
        public int Number { get; set; }
        public TableStatus Status
        {
            get => _status;
            set { _status = value; OnPropertyChanged(); }
        }

        public List<OrderItem> Koszyk { get; set; } = new List<OrderItem>();
        public string UwagiDoKuchni { get; set; }
        public Reservation Rezerwacja { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}