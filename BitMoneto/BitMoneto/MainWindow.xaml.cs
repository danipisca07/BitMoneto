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

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //this.AggiornaFondi();
        }

        private async Task AggiornaFondi()
        {
            await Task.Run(async () =>
            {
                await gestoreFondi.AggiornaFondi();
                //Thread.Sleep(1000);
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
            });
        }

        private void AggiungiDati(string nome, List<Fondo> valori)
        {
            DockPanel pannelloDati = new DockPanel() { LastChildFill = true, Name = "bncPanel" };
            TextBlock titoloDati = new TextBlock() { Text = "Binance" };
            DockPanel.SetDock(titoloDati, Dock.Top);
            pannelloDati.Children.Add(titoloDati);
            DataGrid dati = new DataGrid();

            /*      await Dispatcher.BeginInvoke(new Action(() =>
              *    {
               *       bncData.ItemsSource = bncBalances;
                *  }));
                */
            dati.ItemsSource = valori;
            pannelloDati.Children.Add(dati);
            ContenitoreDati.Children.Add(pannelloDati);
        }

        private async void btnAggiornaFondi_Click(object sender, RoutedEventArgs e)
        {
            prgAggiornaFondi.IsIndeterminate = true;
            try
            {
                await AggiornaFondi();
            }
            catch (Exception eccezione)
            {
                MessageBox.Show("errore: " + eccezione.Message,"Errore", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
