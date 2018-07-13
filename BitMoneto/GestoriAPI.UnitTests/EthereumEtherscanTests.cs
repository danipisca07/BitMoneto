using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using GestoriAPI;
using Criptovalute;

namespace GestoriAPI.UnitTests
{
    [TestClass]
    public class EthereumEtherscanTests
    {
        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void Costruttore_SenzaIndirizzo_Eccezione()
        {
            EthereumEtherscan explorer = new EthereumEtherscan(null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void Costruttore_IndirizzoVuoto_Eccezione()
        {
            EthereumEtherscan explorer = new EthereumEtherscan("");
        }

        [TestMethod]
        public void ScaricaPortafoglio_IndirizzoStabile_Valore()
        {
            String indirizzoStabile = "0x901476A5a3C504398967C745F236124201298016";
            Portafoglio atteso = new Portafoglio(indirizzoStabile, new Valuta("Ethereum","ETH", (decimal)3.129570488777777777));
            EthereumEtherscan blockexplorer = new EthereumEtherscan(indirizzoStabile);
            Portafoglio ris = blockexplorer.ScaricaPortafoglio().Result;
            Assert.AreEqual<Portafoglio>(ris, atteso);
        }

        [TestMethod]
        [ExpectedException(typeof(AggregateException))]
        public void ScaricaPortafoglio_IndirizzoErrato_Eccezione()
        {
            EthereumEtherscan blockexplorer = new EthereumEtherscan("blabla");
            Portafoglio ris = blockexplorer.ScaricaPortafoglio().Result;
        }
    }
}
