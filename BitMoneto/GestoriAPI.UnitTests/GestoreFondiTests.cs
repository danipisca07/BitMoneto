using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Criptovalute;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GestoriAPI.UnitTests
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
            Dictionary<string, List<Fondo>> lista = gestoreFondi.Fondi;
            Assert.AreEqual<int>(lista.Count, 1);
            Assert.IsTrue(lista.TryGetValue("Bitfinex", out List<Fondo> fondi));
            Assert.AreEqual<int>(fondi.Count, 2);
        }

        [TestMethod]
        public void AggiornaFondi_Binance_28()
        {
            Assert.IsTrue(gestoreFondi.AggiungiExchange(binance));
            gestoreFondi.AggiornaFondi().Wait();
            Dictionary<string, List<Fondo>> lista = gestoreFondi.Fondi;
            Assert.AreEqual<int>(lista.Count, 1);
            Assert.IsTrue(lista.TryGetValue("Binance", out List<Fondo> fondi));
            Assert.AreEqual<int>(fondi.Count, 28);
        }

        [TestMethod]
        public void AggiornaFondi_Bitcoin_1()
        {
            Assert.IsTrue(gestoreFondi.AggiungiBlockchain(btc));
            gestoreFondi.AggiornaFondi().Wait();
            Dictionary<string, List<Fondo>> lista = gestoreFondi.Fondi;
            Assert.AreEqual<int>(lista.Count, 1);
            Assert.IsTrue(lista.TryGetValue("Bitcoin(18cBEMRxXHqzWWCxZNtU91F5sbUNKhL5PX)", out List<Fondo> fondi));
            Assert.AreEqual<int>(fondi.Count, 1);
        }

        [TestMethod]
        public void AggiornaFondi_Ethereum_1()
        {
            Assert.IsTrue(gestoreFondi.AggiungiBlockchain(eth));
            gestoreFondi.AggiornaFondi().Wait();
            Dictionary<string, List<Fondo>> lista = gestoreFondi.Fondi;
            Assert.AreEqual<int>(lista.Count, 1);
            Assert.IsTrue(lista.TryGetValue("Ethereum(0x901476A5a3C504398967C745F236124201298016)", out List<Fondo> fondi));
            Assert.AreEqual<int>(fondi.Count, 1);
        }

        [TestMethod]
        public void AggiornaFondi_Tutti_4()
        {
            Assert.IsTrue(gestoreFondi.AggiungiExchange(bitfinex));
            Assert.IsTrue(gestoreFondi.AggiungiExchange(binance));
            Assert.IsTrue(gestoreFondi.AggiungiBlockchain(btc));
            Assert.IsTrue(gestoreFondi.AggiungiBlockchain(eth));
            gestoreFondi.AggiornaFondi().Wait();
            Dictionary<string, List<Fondo>> lista = gestoreFondi.Fondi;
            Assert.AreEqual<int>(lista.Count, 4);
        }
    }
}
