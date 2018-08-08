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
        string password;
        public MainWindow(string password)
        {
            InitializeComponent();
            this.password = password;
            convertitore = new CryptoCompareConvertitore();
            valutaFactory = new ValutaFactory(convertitore);
            gestoreFondi = new GestoreFondi();
            //TestInizializza();
            CaricaApi();
            //TODO Aggiungere gestione impostazioni per bitfinex e etherscan
        }

        private void TestInizializza()
        {
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

        #region Fondi

        private async Task AggiornaFondi()
        {
            prgAggiornaFondi.IsIndeterminate = true;
            //TODO inserire timeout?
            await Task.Run(async () =>
            {
                try
                {
                    await valutaFactory.AggiornaCambi();
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
                    }));
                }
                catch (Exception eccezione)
                {
                    MessageBox.Show("errore: " + eccezione.Message, "Errore", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                finally
                {
                    await Dispatcher.BeginInvoke(new Action(() =>
                    {
                        prgAggiornaFondi.IsIndeterminate = false;
                    }));
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

        #endregion

        #region Impostazioni


        private void CaricaApi()
        {
            BinanceExchange binance = GestoreImpostazioni.LeggiDatiExchange<BinanceExchange>(valutaFactory);
            if (binance != null)
            {
                gestoreFondi.AggiungiExchange(binance);
            }
            BitcoinBlockexplorer bitcoin = GestoreImpostazioni.LeggiDatiBlockchain<BitcoinBlockexplorer>(valutaFactory);
            if (bitcoin != null)
            {
                gestoreFondi.AggiungiBlockchain(bitcoin);
            }
        }

        private void SalvaPasswordBtn_Click(object sender, RoutedEventArgs e)
        {
            //TODO Aggiungere controllo 8 caratteri alla password
            string password = PasswordTxt.Text;
            if(password == null || password == "")
            {
                MessageBox.Show("Inserire prima una password");
            }
            else
            {
                GestoreImpostazioni.SalvaPassword(password);
                MessageBox.Show("Password Salvata");
            }
        }

        private void SalvaBinanceBtn_Click(object sender, RoutedEventArgs e)
        {
            string apiPub = BinancePubblicaTxt.Text;
            string apiPriv = BinancePrivataTxt.Text;
            try
            {
                BinanceExchange binance = new BinanceExchange(apiPub, apiPriv, valutaFactory);
                if(gestoreFondi.AggiungiExchange(binance))
                {
                    GestoreImpostazioni.SalvaDatiExchange(binance);
                    Dispatcher.BeginInvoke(new Action(async () => { await AggiornaFondi(); }));
                    MessageBox.Show("Chiavi api salvate. Avviato aggiornamento fondi");
                }
                else
                {
                    MessageBox.Show("Ci sono già delle chiavi API associate per binance, rimuoverle prima di aggiungerne delle altre.");
                }
                
            }
            catch (ArgumentException eccezione)
            {
                MessageBox.Show(eccezione.Message);
            }
        }

        private void RimuoviBinanceBtn_Click(object sender, RoutedEventArgs e)
        {
            bool rimossoDaGestore = gestoreFondi.RimuoviExchange(typeof(BinanceExchange));
            bool rimossoDaConfigurazione = GestoreImpostazioni.RimuoviDatiApi<BinanceExchange>();
            if (rimossoDaGestore || rimossoDaConfigurazione)
            {
                Dispatcher.BeginInvoke(new Action(async () => { await AggiornaFondi(); }));
                MessageBox.Show("Exchange rimosso correttamente.");
            }
            else
                MessageBox.Show("Non ci sono attualmente chiavi associate per Binance.");
        }

        private void SalvaBitcoinBtn_Click(object sender, RoutedEventArgs e)
        {
            string indirizzo = BitcoinTxt.Text;
            try
            {
                BitcoinBlockexplorer bitcoin = new BitcoinBlockexplorer(indirizzo, valutaFactory);
                if(gestoreFondi.AggiungiBlockchain(bitcoin))
                {
                    GestoreImpostazioni.SalvaDatiBlockchain(bitcoin);
                    MessageBox.Show("Indirizzo salvato. Avviato aggiornamento fondi");
                    Dispatcher.BeginInvoke(new Action(async () => { await AggiornaFondi(); }));
                }
                else
                {
                    MessageBox.Show("Esiste già un indirizzo associato, rimuoverlo prima di aggiungerne un altro.");
                }
            }
            catch (ArgumentException eccezione)
            {
                MessageBox.Show(eccezione.Message);
            }
        }

        private void RimuoviBitcoinBtn_Click(object sender, RoutedEventArgs e)
        {
            bool rimossoDaGestore = gestoreFondi.RimuoviBlockchain(typeof(BitcoinBlockexplorer));
            bool rimossoDaConfigurazione = GestoreImpostazioni.RimuoviDatiApi<BinanceExchange>();
            if (rimossoDaGestore || rimossoDaConfigurazione)
            {
                Dispatcher.BeginInvoke(new Action(async () => { await AggiornaFondi(); }));
                MessageBox.Show("Protafoglio rimosso correttamente.");
            }
            else
                MessageBox.Show("Non ci sono attualmente indirizzi associati.");
        }

        #endregion


    }
}
