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
            Dictionary<string, List<Fondo>> lista = gestoreFondi.Fondi;
            Assert.AreEqual<int>(1,lista.Count);
            Assert.IsTrue(lista.TryGetValue("TestExchange", out List<Fondo> fondi));
            Assert.AreEqual<int>(2,fondi.Count);
        }
        
        [TestMethod]
        public void AggiornaFondi_TestBlockchain_1()
        {
            Assert.IsTrue(gestoreFondi.AggiungiBlockchain(blockchain));
            gestoreFondi.AggiornaFondi().Wait();
            Dictionary<string, List<Fondo>> lista = gestoreFondi.Fondi;
            Assert.AreEqual<int>(lista.Count, 1);
            Assert.IsTrue(lista.TryGetValue("TestBlockchain", out List<Fondo> fondi));
            Assert.AreEqual<int>(fondi.Count, 1);
        }

        [TestMethod]
        public void AggiornaFondi_Tutti_2()
        {
            Assert.IsTrue(gestoreFondi.AggiungiExchange(exchange));
            Assert.IsTrue(gestoreFondi.AggiungiBlockchain(blockchain));
            gestoreFondi.AggiornaFondi().Wait();
            Dictionary<string, List<Fondo>> liste = gestoreFondi.Fondi;
            Assert.AreEqual<int>(liste.Count, 2);
            foreach(List<Fondo> lista in liste.Values)
            {
                Assert.IsTrue(lista.Count > 0);
            }
        }
    }
}
