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
        IExchange exchange;
        IBlockchain blockchain;

        public GestoreFondiTests()
        {
            convertitore = new TestConvertitore();
            valutaFactory = new ValutaFactory(convertitore);
            gestoreFondi = new GestoreFondi();
            exchange = new TestExchange();
            blockchain = new TestBlockchain();
        }

        [TestMethod]
        public void AggiungiExchange_Test_True()
        {
            Assert.IsTrue(gestoreFondi.AggiungiExchange(exchange));
        }
        
        [TestMethod]
        public void AggiungiBlockchain_Test_True()
        {
            Assert.IsTrue(gestoreFondi.AggiungiBlockchain(blockchain));
        }

        [TestMethod]
        public void AggiornaFondi_TestExchange_OK()
        {
            Assert.IsTrue(gestoreFondi.AggiungiExchange(exchange));
            gestoreFondi.AggiornaFondi().Wait();
            Assert.AreEqual<int>(1, gestoreFondi.Exchanges.Count);
            Assert.AreEqual<string>("TestExchange", gestoreFondi.Exchanges.ToArray()[0].Nome);
            List<Fondo> fondi = gestoreFondi.Exchanges.ToArray()[0].Fondi;
            Assert.AreEqual<int>(2,fondi.Count);
        }
        
        [TestMethod]
        public void AggiornaFondi_TestBlockchain_1()
        {
            Assert.IsTrue(gestoreFondi.AggiungiBlockchain(blockchain));
            gestoreFondi.AggiornaFondi().Wait();
            Assert.AreEqual<int>(1, gestoreFondi.Blockchains.Count);
            Assert.AreEqual<string>("TestBlockchain", gestoreFondi.Blockchains.ToArray()[0].Nome);
            List<Fondo> fondi = new List<Fondo>() { gestoreFondi.Blockchains.ToArray()[0].Portafoglio.Fondo };
            Assert.AreEqual<int>(fondi.Count, 1);
        }

        [TestMethod]
        public void AggiornaFondi_Tutti_2()
        {
            Assert.IsTrue(gestoreFondi.AggiungiExchange(exchange));
            Assert.IsTrue(gestoreFondi.AggiungiBlockchain(blockchain));
            gestoreFondi.AggiornaFondi().Wait();
            //Controllo exchange
            Assert.AreEqual<int>(1, gestoreFondi.Exchanges.Count);
            Assert.AreEqual<string>("TestExchange", gestoreFondi.Exchanges.ToArray()[0].Nome);
            List<Fondo> fondi = gestoreFondi.Exchanges.ToArray()[0].Fondi;
            Assert.AreEqual<int>(2, fondi.Count);
            //Controllo blockchain
            Assert.AreEqual<int>(1, gestoreFondi.Blockchains.Count);
            Assert.AreEqual<string>("TestBlockchain", gestoreFondi.Blockchains.ToArray()[0].Nome);
            fondi = new List<Fondo>() { gestoreFondi.Blockchains.ToArray()[0].Portafoglio.Fondo };
            Assert.AreEqual<int>(fondi.Count, 1);
        }
    }
}
