using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using GestoriAPI;
using Criptovalute;

namespace Bitmoneto.UnitTests
{
    [TestClass]
    public class BitcoinBlockExplorerTests
    {
        IConvertitore convertitore;
        ValutaFactory factory;
        public BitcoinBlockExplorerTests()
        {
            convertitore = new TestConvertitore();
            factory = new ValutaFactory(convertitore);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void Costruttore_SenzaIndirizzo_Eccezione()
        {
            BitcoinBlockexplorer explorer = new BitcoinBlockexplorer(null,factory);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void Costruttore_IndirizzoVuoto_Eccezione()
        {
            BitcoinBlockexplorer explorer = new BitcoinBlockexplorer("", factory);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void Costruttore_FactoryNull_Eccezione()
        {
            BitcoinBlockexplorer explorer = new BitcoinBlockexplorer("asdff2f23f3qr32r2rq23r2vrq23vr", null);
        }

        [TestMethod]
        public void ScaricaPortafoglio_IndirizzoStabile_Valore()
        {
            String indirizzoStabile = "1JaPNwMXt2AuVkWmkUHbsw78MbGorTfmm2";
            Valuta bitcoin = factory.OttieniValuta("BTC");
            Fondo fondo = new Fondo(bitcoin, (decimal)2194.51101);
            Portafoglio atteso = new Portafoglio(indirizzoStabile, fondo);
            BitcoinBlockexplorer blockexplorer = new BitcoinBlockexplorer(indirizzoStabile, factory);
            Portafoglio ris = blockexplorer.ScaricaPortafoglio().Result;
            Assert.AreEqual<Portafoglio>(ris, atteso);
        }

        [TestMethod]
        [ExpectedException(typeof(AggregateException))]
        public void ScaricaPortafoglio_IndirizzoErrato_Eccezione()
        {
            BitcoinBlockexplorer blockexplorer = new BitcoinBlockexplorer("blabla",factory);
            Portafoglio ris = blockexplorer.ScaricaPortafoglio().Result;
        }
    }
}
