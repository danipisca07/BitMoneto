using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Criptovalute;
using GestoriAPI;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bitmoneto.UnitTests
{
    [TestClass]
    public class GestoreFondiTests
    {
        ValutaFactory valutaFactory;
        IConvertitore convertitore;
        GestoreFondi gestoreFondi;
        private BitfinexExchange bitfinex;
        private BinanceExchange binance;
        private BitcoinBlockexplorer btc;
        private EthereumEtherscan eth;

        public GestoreFondiTests()
        {
            convertitore = new CryptoCompareConvertitore();
            valutaFactory = new ValutaFactory(convertitore);
            gestoreFondi = new GestoreFondi();
            bitfinex = new BitfinexExchange("5RMqfG7b2qOBkoPIi97UjCpPxnIhAUsDMelbT5K3pB2",
                "hnQNJgD80w1WJeZW7zclyJvFkTWNSN0N4r98t7oRrWw", valutaFactory);
            binance = new BinanceExchange("VhP4edkGMEmL51YSIXSdva0IkcGxC68r8dOIGg6G5PcNMr3srPcm4rXEled5KeMs",
                "1ET6MkbrkS2U1sIvQDu6gDzYzNuYgPX2ujG2Lt8tL5SFTygMKUeyDRFDJPT8Ry6Y", valutaFactory);
            btc = new BitcoinBlockexplorer("18cBEMRxXHqzWWCxZNtU91F5sbUNKhL5PX", valutaFactory);
            eth = new EthereumEtherscan("0x901476A5a3C504398967C745F236124201298016", valutaFactory);
        }

        [TestMethod]
        public void AggiungiExchange_Bitfinex_True()
        {
            Assert.IsTrue(gestoreFondi.AggiungiExchange(bitfinex));
        }

        [TestMethod]
        public void AggiungiExchange_Binance_True()
        {
            Assert.IsTrue(gestoreFondi.AggiungiExchange(binance));
        }

        [TestMethod]
        public void AggiungiBlockchain_Bitcoin_True()
        {
            Assert.IsTrue(gestoreFondi.AggiungiBlockchain(btc));
        }

        [TestMethod]
        public void AggiungiBlockchain_Ethereum_True()
        {
            Assert.IsTrue(gestoreFondi.AggiungiBlockchain(eth));
        }

        [TestMethod]
        public void AggiornaFondi_Bitfinex_2()
        {
            Assert.IsTrue(gestoreFondi.AggiungiExchange(bitfinex));
            gestoreFondi.AggiornaFondi().Wait();
            Assert.AreEqual<int>(1, gestoreFondi.Exchanges.Count);
            Assert.AreEqual<string>("Bitfinex", gestoreFondi.Exchanges.ToArray()[0].Nome);
            List<Fondo> fondi = gestoreFondi.Exchanges.ToArray()[0].Fondi;
            Assert.AreEqual<int>(fondi.Count, 2);
            foreach (Fondo fondo in fondi)
            {
                if (fondo.Valuta.Nome != fondo.Valuta.Simbolo)
                    Assert.IsTrue(fondo.Valuta.Cambi.Count == 4);
            }
        }

        [TestMethod]
        public void AggiornaFondi_Binance_30()
        {
            Assert.IsTrue(gestoreFondi.AggiungiExchange(binance));
            gestoreFondi.AggiornaFondi().Wait();
            Assert.AreEqual<int>(1, gestoreFondi.Exchanges.Count);
            Assert.AreEqual<string>("Binance", gestoreFondi.Exchanges.ToArray()[0].Nome);
            List<Fondo> fondi = gestoreFondi.Exchanges.ToArray()[0].Fondi;
            Assert.AreEqual<int>(fondi.Count, 30);
            foreach (Fondo fondo in fondi)
            {
                if(fondo.Valuta.Nome != fondo.Valuta.Simbolo)
                    Assert.IsTrue(fondo.Valuta.Cambi.Count == 4);
            }
        }

        [TestMethod]
        public void AggiornaFondi_Bitcoin_1()
        {
            Assert.IsTrue(gestoreFondi.AggiungiBlockchain(btc));
            gestoreFondi.AggiornaFondi().Wait();
            Assert.AreEqual<int>(1, gestoreFondi.Blockchains.Count);
            Assert.AreEqual<string>("Bitcoin(18cBEMRxXHqzWWCxZNtU91F5sbUNKhL5PX)", gestoreFondi.Blockchains.ToArray()[0].Nome);
            List<Fondo> fondi = new List<Fondo>() { gestoreFondi.Blockchains.ToArray()[0].Portafoglio.Fondo };
            Assert.AreEqual<int>(fondi.Count, 1);
            Assert.AreEqual<decimal>(1, fondi.ToArray()[0].Valuta.Cambi["BTC"]);
        }

        [TestMethod]
        public void AggiornaFondi_Ethereum_1()
        {
            Assert.IsTrue(gestoreFondi.AggiungiBlockchain(eth));
            gestoreFondi.AggiornaFondi().Wait();
            Assert.AreEqual<int>(1, gestoreFondi.Blockchains.Count);
            Assert.AreEqual<string>("Ethereum(0x901476A5a3C504398967C745F236124201298016)", gestoreFondi.Blockchains.ToArray()[0].Nome);
            List<Fondo> fondi = new List<Fondo>() { gestoreFondi.Blockchains.ToArray()[0].Portafoglio.Fondo };
            Assert.AreEqual<int>(fondi.Count, 1);
            Assert.AreEqual<decimal>(1, fondi.ToArray()[0].Valuta.Cambi["ETH"]);
        }

        [TestMethod]
        public void AggiornaFondi_Tutti_4()
        {
            Assert.IsTrue(gestoreFondi.AggiungiExchange(bitfinex));
            Assert.IsTrue(gestoreFondi.AggiungiExchange(binance));
            Assert.IsTrue(gestoreFondi.AggiungiBlockchain(btc));
            Assert.IsTrue(gestoreFondi.AggiungiBlockchain(eth));
            gestoreFondi.AggiornaFondi().Wait();
            Assert.AreEqual<int>(2, gestoreFondi.Exchanges.Count);
            Assert.AreEqual<int>(2, gestoreFondi.Blockchains.Count);
            foreach(IExchange ex in gestoreFondi.Exchanges)
            {
                Assert.IsTrue(ex.Fondi.Count > 0);
                foreach (Fondo fondo in ex.Fondi)
                {
                    if (fondo.Valuta.Nome != fondo.Valuta.Simbolo)
                        Assert.IsTrue(fondo.Valuta.Cambi.Count == 4);
                }
            }
            foreach (IBlockchain bc in gestoreFondi.Blockchains)
            {
                Assert.IsTrue(bc.Portafoglio != null);
                if (bc.Portafoglio.Fondo.Valuta.Nome != bc.Portafoglio.Fondo.Valuta.Simbolo)
                        Assert.IsTrue(bc.Portafoglio.Fondo.Valuta.Cambi.Count == 4);
            }
        }
    }
}
