using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;

namespace Zadanie_3
{
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        public MainWindow()
        {
            InitializeComponent();
            this.DataContext = this;
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private string _imieNazwisko = "";
        public string ImieNazwisko
        {
            get { return _imieNazwisko; }
            set
            {
                if (_imieNazwisko != value)
                {
                    _imieNazwisko = value;
                    OnPropertyChanged();
                    RozdzielImieNazwisko();
                }
            }
        }

        private string _dataUrodzin = "";
        public string DataUrodzin
        {
            get { return _dataUrodzin; }
            set
            {
                if (_dataUrodzin != value)
                {
                    _dataUrodzin = value;
                    OnPropertyChanged();
                    ObliczWiek();
                }
            }
        }

        private string _pierwszeImie = "";
        public string PierwszeImie
        {
            get { return _pierwszeImie; }
            set { _pierwszeImie = value; OnPropertyChanged(); }
        }

        private string _nazwisko = "";
        public string Nazwisko
        {
            get { return _nazwisko; }
            set { _nazwisko = value; OnPropertyChanged(); }
        }

        private string _wiek = "";
        public string Wiek
        {
            get { return _wiek; }
            set { _wiek = value; OnPropertyChanged(); }
        }

        private void RozdzielImieNazwisko()
        {
            if (string.IsNullOrWhiteSpace(ImieNazwisko))
            {
                PierwszeImie = "";
                Nazwisko = "";
                return;
            }

            string[] czesci = ImieNazwisko.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

            if (czesci.Length > 0)
            {
                PierwszeImie = czesci[0];
                Nazwisko = czesci.Length > 1 ? czesci[czesci.Length - 1] : "";
            }
        }

        private void ObliczWiek()
        {
            if (DateTime.TryParse(DataUrodzin, out DateTime data))
            {
                int lata = DateTime.Today.Year - data.Year;

                if (data.Date > DateTime.Today.AddYears(-lata))
                {
                    lata--;
                }
                Wiek = lata.ToString();
            }
            else
            {
                Wiek = "";
            }
        }
    }
}