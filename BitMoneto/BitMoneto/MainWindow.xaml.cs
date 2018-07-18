using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Criptovalute;
using GestoriAPI;

namespace BitMoneto
{
    /// <summary>
    /// Logica di interazione per MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        ValutaFactory valutaFactory;
        IConvertitore convertitore;
        GestoreFondi gestoreFondi;
        public MainWindow()
        {
            InitializeComponent();
            convertitore = new CryptoCompareConvertitore();
            valutaFactory = new ValutaFactory(convertitore);
            gestoreFondi = new GestoreFondi();
            BitfinexExchange bitfinex = new BitfinexExchange("5RMqfG7b2qOBkoPIi97UjCpPxnIhAUsDMelbT5K3pB2", 
                "hnQNJgD80w1WJeZW7zclyJvFkTWNSN0N4r98t7oRrWw", valutaFactory);
            gestoreFondi.AggiungiExchange(bitfinex);
            BinanceExchange binance = new BinanceExchange("VhP4edkGMEmL51YSIXSdva0IkcGxC68r8dOIGg6G5PcNMr3srPcm4rXEled5KeMs",
                "1ET6MkbrkS2U1sIvQDu6gDzYzNuYgPX2ujG2Lt8tL5SFTygMKUeyDRFDJPT8Ry6Y", valutaFactory);
            gestoreFondi.AggiungiExchange(binance);
            BitcoinBlockexplorer btc = new BitcoinBlockexplorer("18cBEMRxXHqzWWCxZNtU91F5sbUNKhL5PX", valutaFactory);
            gestoreFondi.AggiungiBlockchain(btc);
            EthereumEtherscan eth = new EthereumEtherscan("0x901476A5a3C504398967C745F236124201298016", valutaFactory);
            gestoreFondi.AggiungiBlockchain(eth);
        }

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            await AggiornaFondi();
        }

        private async Task AggiornaFondi()
        {
            prgAggiornaFondi.IsIndeterminate = true;
            await Task.Run(async () =>
            {
                try
                {
                    await gestoreFondi.AggiornaFondi();
                    await Dispatcher.BeginInvoke(new Action(() =>
                    {
                        ContenitoreDati.Children.Clear();
                        Dictionary<string, List<Fondo>> fondi = gestoreFondi.Fondi;
                        foreach (string nome in fondi.Keys)
                        {
                            bool ok = fondi.TryGetValue(nome, out List<Fondo> valori);
                            if (ok) AggiungiDati(nome, valori);
                        }
                        prgAggiornaFondi.IsIndeterminate = false;
                    }));
                }
                catch (Exception eccezione)
                {
                    MessageBox.Show("errore: " + eccezione.Message, "Errore", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            });
        }

        private void AggiungiDati(string nome, List<Fondo> valori)
        {
            DockPanel pannelloDati = new DockPanel() { LastChildFill = true, Name = "bncPanel" };
            TextBlock titoloDati = new TextBlock() { Text = nome };
            DockPanel.SetDock(titoloDati, Dock.Top);
            pannelloDati.Children.Add(titoloDati);
            DataGrid dati = new DataGrid
            {
                ItemsSource = valori,
                AutoGenerateColumns = false,
                CanUserAddRows = false,
                CanUserDeleteRows = false,
                IsReadOnly = true
            };

            var colonnaNome = new DataGridTextColumn
            {
                Binding = new Binding("Valuta.Nome"),
                Header = "Nome",
                Width = DataGridLength.Auto
            };
            dati.Columns.Add(colonnaNome);

            var colonnaSimbolo = new DataGridTextColumn
            {
                Binding = new Binding("Valuta.Simbolo"),
                Header = "Simbolo",
                Width = DataGridLength.Auto
            };
            dati.Columns.Add(colonnaSimbolo);

            var colonnaQuantità = new DataGridTextColumn
            {
                Binding = new Binding("Quantità"),
                Header = "Quantità",
                Width = DataGridLength.Auto
            };
            dati.Columns.Add(colonnaQuantità);

            foreach(string s in convertitore.SimboliConversioni)
            {
                var colConv = new DataGridTextColumn
                {
                    Header = "Valore(" + s + ")",
                    Width = DataGridLength.Auto
                };
                var binding = new Binding();
                CambioConverter cambioConverter = new CambioConverter(s);
                binding.Converter = cambioConverter;
                colConv.Binding = binding;
                dati.Columns.Add(colConv);
            }
            

            pannelloDati.Children.Add(dati);
            ContenitoreDati.Children.Add(pannelloDati);
        }

        private async void btnAggiornaFondi_Click(object sender, RoutedEventArgs e)
        {
            await AggiornaFondi();
        }

        private void ScrollViewerDati_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            ScrollViewer scv = (ScrollViewer)sender;
            scv.ScrollToVerticalOffset(scv.VerticalOffset - e.Delta);
            e.Handled = true;
        }
    }
}
