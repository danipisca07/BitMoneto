using System;
using System.Collections.Generic;
using Criptovalute;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GestoriAPI.UnitTests
{
    [TestClass]
    public class BinanceExchangeTests
    {
        private String pubKey = "VhP4edkGMEmL51YSIXSdva0IkcGxC68r8dOIGg6G5PcNMr3srPcm4rXEled5KeMs";
        private String privKey = "1ET6MkbrkS2U1sIvQDu6gDzYzNuYgPX2ujG2Lt8tL5SFTygMKUeyDRFDJPT8Ry6Y";
        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void Costruttore_SenzaPrivata_Eccezione()
        {
            BinanceExchange explorer = new BinanceExchange(pubKey,null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void Costruttore_SenzaPubblica_Eccezione()
        {
            BinanceExchange explorer = new BinanceExchange(null, privKey);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void Costruttore_PrivataVuota_Eccezione()
        {
            BinanceExchange explorer = new BinanceExchange(pubKey, "");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void Costruttore_PubblicaVuota_Eccezione()
        {
            BinanceExchange explorer = new BinanceExchange("", privKey);
        }

        [TestMethod]
        public void ScaricaFondi_ChiaviOk_Array27Valute()
        {
            BinanceExchange explorer = new BinanceExchange(pubKey, privKey);
            List<Valuta> ris = explorer.ScaricaFondi().Result;
            Assert.AreEqual(ris.Count, 27);
        }

        [TestMethod]
        [ExpectedException(typeof(AggregateException))]
        public void ScaricaFondi_ChiaviErrate_Eccezzione()
        {
            BinanceExchange explorer = new BinanceExchange("blabla", "blabla");
            List<Valuta> ris = explorer.ScaricaFondi().Result;
        }
    }
}
