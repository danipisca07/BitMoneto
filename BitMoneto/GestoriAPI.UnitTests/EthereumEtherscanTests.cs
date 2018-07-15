using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using GestoriAPI;
using Criptovalute;

namespace GestoriAPI.UnitTests
{
    [TestClass]
    public class EthereumEtherscanTests
    {
        Convertitore convertitore;
        ValutaFactory factory;
        public EthereumEtherscanTests()
        {
            convertitore = new CryptoCompareConvertitore();
            factory = new ValutaFactory(convertitore);
        }
        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void Costruttore_SenzaIndirizzo_Eccezione()
        {
            EthereumEtherscan explorer = new EthereumEtherscan(null,factory);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void Costruttore_IndirizzoVuoto_Eccezione()
        {
            EthereumEtherscan explorer = new EthereumEtherscan("",factory);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void Costruttore_FactoryNull_Eccezione()
        {
            EthereumEtherscan explorer = new EthereumEtherscan("0x9065496049506905486094569045", null);
        }

        [TestMethod]
        public void ScaricaPortafoglio_IndirizzoStabile_Valore()
        {
            String indirizzoStabile = "0x901476A5a3C504398967C745F236124201298016";
            Valuta ethereum = factory.OttieniValuta("ETH");
            Fondo fondo = new Fondo(ethereum, (decimal)3.129570488777777777);
            Portafoglio atteso = new Portafoglio(indirizzoStabile, fondo);
            EthereumEtherscan blockexplorer = new EthereumEtherscan(indirizzoStabile, factory);
            Portafoglio ris = blockexplorer.ScaricaPortafoglio().Result;
            Assert.AreEqual<Portafoglio>(ris, atteso);
        }

        [TestMethod]
        [ExpectedException(typeof(AggregateException))]
        public void ScaricaPortafoglio_IndirizzoErrato_Eccezione()
        {
            EthereumEtherscan blockexplorer = new EthereumEtherscan("blabla",factory);
            Portafoglio ris = blockexplorer.ScaricaPortafoglio().Result;
        }
    }
}
