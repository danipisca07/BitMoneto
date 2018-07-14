using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using GestoriAPI;
using Criptovalute;

namespace GestoriAPI.UnitTests
{
    [TestClass]
    public class BitcoinExplorerTests
    {
        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void Costruttore_SenzaIndirizzo_Eccezione()
        {
            BitcoinBlockexplorer explorer = new BitcoinBlockexplorer(null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void Costruttore_IndirizzoVuoto_Eccezione()
        {
            BitcoinBlockexplorer explorer = new BitcoinBlockexplorer("");
        }

        [TestMethod]
        public void ScaricaPortafoglio_IndirizzoStabile_Valore()
        {
            String indirizzoStabile = "1JaPNwMXt2AuVkWmkUHbsw78MbGorTfmm2";
            Portafoglio atteso = new Portafoglio(indirizzoStabile, new Valuta("Bitcoin", "BTC", (decimal)2194.51101));
            BitcoinBlockexplorer blockexplorer = new BitcoinBlockexplorer(indirizzoStabile);
            Portafoglio ris = blockexplorer.ScaricaPortafoglio().Result;
            Assert.AreEqual<Portafoglio>(ris, atteso);
        }

        [TestMethod]
        [ExpectedException(typeof(AggregateException))]
        public void ScaricaPortafoglio_IndirizzoErrato_Eccezione()
        {
            BitcoinBlockexplorer blockexplorer = new BitcoinBlockexplorer("blabla");
            Portafoglio ris = blockexplorer.ScaricaPortafoglio().Result;
        }
    }
}
