using System;
using System.Windows;
using System.Windows.Controls;

namespace Kalkulator
{
    public partial class MainWindow : Window
    {
        // Zmienne globalne 
        double lewyArgument = 0;
        double prawyArgument = 0; 
        bool czyJestPrawyArgument = false;
        string ostatniOperator = "";
        bool czyNowaLiczba = true;
        bool czyPowtorzenieOperacji = false;

        public MainWindow()
        {
            InitializeComponent();
            ResetujWszystko();
        }

        // Wpisywanie cyfr 
        private void Liczba_Click(object sender, RoutedEventArgs e)
        {
            Button b = (Button)sender;
            string cyfra = b.Content.ToString();

            if (czyNowaLiczba)
            {
                if (cyfra == "0")
                {
                    TxtResult.Text = "0";
                }
                else
                {
                    TxtResult.Text = cyfra;
                }
                czyNowaLiczba = false;
            }
            else
            {
                if (TxtResult.Text == "0")
                {
                    TxtResult.Text = cyfra;
                }
                else
                {
                    TxtResult.Text += cyfra; 
                }
            }
            czyPowtorzenieOperacji = false;
        }

        // Obsługa przecinka
        private void Przecinek_Click(object sender, RoutedEventArgs e)
        {
            if (czyNowaLiczba)
            {
                TxtResult.Text = "0,";
                czyNowaLiczba = false;
                return;
            }

            
            if (!TxtResult.Text.Contains(","))
            {
                TxtResult.Text += ",";
            }
        }

        // Zmiana znaku +/-
        private void Znak_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                double n = Convert.ToDouble(TxtResult.Text);
                n = n * -1;
                TxtResult.Text = n.ToString();
            }
            catch
            {
                TxtResult.Text = "Błąd";
            }
        }

        // Kasowanie pojedynczego znaku (Backspace)
        private void Backspace_Click(object sender, RoutedEventArgs e)
        {
            if (czyNowaLiczba) return;

            if (TxtResult.Text.Length > 1)
            {
                TxtResult.Text = TxtResult.Text.Substring(0, TxtResult.Text.Length - 1);
                if (TxtResult.Text == "-")
                {
                    TxtResult.Text = "0";
                    czyNowaLiczba = true;
                }
            }
            else
            {
                TxtResult.Text = "0";
                czyNowaLiczba = true;
            }
        }

        // Operatory (+, -, *, /)
        private void OperatorDwuargumentowy_Click(object sender, RoutedEventArgs e)
        {
            Button b = (Button)sender;
            string nowyOperator = b.Content.ToString();
            double aktualna = Convert.ToDouble(TxtResult.Text);

            
            if (ostatniOperator != "" && !czyNowaLiczba && !czyPowtorzenieOperacji)
            {
                lewyArgument = WykonajDzialanie(lewyArgument, aktualna, ostatniOperator);
                TxtResult.Text = lewyArgument.ToString();
            }
            else
            {
                lewyArgument = aktualna;
            }

            ostatniOperator = nowyOperator;
            czyJestPrawyArgument = false; // Reset flagi zapamiętanego argumentu
            czyNowaLiczba = true;
            czyPowtorzenieOperacji = false;

            TxtFormula.Text = lewyArgument.ToString() + " " + ostatniOperator;
        }

        // Operatory jednoargumentowe
        private void OperatorJednoargumentowy_Click(object sender, RoutedEventArgs e)
        {
            Button b = (Button)sender;
            string op = b.Content.ToString();
            double arg = Convert.ToDouble(TxtResult.Text);
            double wynik = 0;

            if (op == "1/x")
            {
                if (arg == 0)
                {
                    MessageBox.Show("Nie można dzielić przez zero!", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
                    ResetujWszystko();
                    return;
                }
                wynik = 1 / arg;
                DodajDoPaneluHistorii("1/(" + arg + ") = " + wynik);
            }
            else if (op == "x^2")
            {
                wynik = arg * arg; 
                System.Diagnostics.Debug.WriteLine(wynik);
                DodajDoPaneluHistorii("(" + arg + ")^2 = " + wynik);
            }
            else if (op == "sqrt")
            {
                if (arg < 0)
                {
                    MessageBox.Show("Niepoprawne dane!", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
                    ResetujWszystko();
                    return;
                }
                wynik = Math.Sqrt(arg);
                DodajDoPaneluHistorii("sqrt(" + arg + ") = " + wynik);
            }

            TxtResult.Text = wynik.ToString();
            czyNowaLiczba = true;
            ostatniOperator = op;
            prawyArgument = arg;
            czyJestPrawyArgument = true;
        }

        // Procenty
        private void Procent_Click(object sender, RoutedEventArgs e)
        {
            double prawy = Convert.ToDouble(TxtResult.Text);
            double wynik = 0;

            if (ostatniOperator == "+" || ostatniOperator == "-")
            {
                wynik = lewyArgument * (prawy / 100.0);
            }
            else
            {
                wynik = prawy / 100.0;
            }

            TxtResult.Text = wynik.ToString();
        }

        // Wynik (=)
        private void Rownasie_Click(object sender, RoutedEventArgs e)
        {
            
            if (ostatniOperator == "1/x" || ostatniOperator == "x^2" || ostatniOperator == "sqrt")
            {
                double arg = Convert.ToDouble(TxtResult.Text);
                double w = 0;
                if (ostatniOperator == "1/x" && arg != 0) w = 1 / arg;
                if (ostatniOperator == "x^2") w = arg * arg;
                if (ostatniOperator == "sqrt" && arg >= 0) w = Math.Sqrt(arg);

                TxtResult.Text = w.ToString();
                czyNowaLiczba = true;
                return;
            }

            if (ostatniOperator == "") return;

            double p;
            if (czyPowtorzenieOperacji)
            {
                if (czyJestPrawyArgument) p = prawyArgument;
                else p = lewyArgument;

                lewyArgument = Convert.ToDouble(TxtResult.Text);
            }
            else
            {
                p = Convert.ToDouble(TxtResult.Text);
                prawyArgument = p;
                czyJestPrawyArgument = true;
            }

            double wynikFinalny = WykonajDzialanie(lewyArgument, p, ostatniOperator);

            TxtFormula.Text = "";
            DodajDoPaneluHistorii(lewyArgument + " " + ostatniOperator + " " + p + " = " + wynikFinalny);

            TxtResult.Text = wynikFinalny.ToString();

            lewyArgument = wynikFinalny;
            czyNowaLiczba = true;
            czyPowtorzenieOperacji = true;
        }

        //Dzialania
        private double WykonajDzialanie(double l, double p, string op)
        {
            if (op == "+") return l + p;
            if (op == "-") return l - p;
            if (op == "*") return l * p;
            if (op == "/")
            {
                if (p == 0)
                {
                    MessageBox.Show("Nie można dzielić przez zero!", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
                    ResetujWszystko();
                    return 0;
                }
                return l / p;
            }
            return p;
        }

        private void C_Click(object sender, RoutedEventArgs e)
        {
            ResetujWszystko();
        }

        private void CE_Click(object sender, RoutedEventArgs e)
        {
            TxtResult.Text = "0";
            czyNowaLiczba = true;
        }

        private void ResetujWszystko()
        {
            lewyArgument = 0;
            prawyArgument = 0;
            czyJestPrawyArgument = false;
            ostatniOperator = "";
            czyNowaLiczba = true;
            czyPowtorzenieOperacji = false;
            TxtResult.Text = "0";
            TxtFormula.Text = "";
        }

        // Dodawanie wpisu do historii
        private void DodajDoPaneluHistorii(string tekst)
        {
            TextBlock wpis = new TextBlock();
            wpis.Text = tekst;
            wpis.Margin = new Thickness(0, 2, 0, 2);
            wpis.TextWrapping = TextWrapping.Wrap;
            wpis.FontSize = 13;

            PanelHistorii.Children.Add(wpis);
        }
    }
}