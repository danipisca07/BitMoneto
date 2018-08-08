using System;
using System.Collections.Generic;
using Criptovalute;
using GestoriAPI;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bitmoneto.UnitTests
{
    [TestClass]
    public class BitfinexExchangeTests
    {
        private String pubKey = "5RMqfG7b2qOBkoPIi97UjCpPxnIhAUsDMelbT5K3pB2";
        private String privKey = "hnQNJgD80w1WJeZW7zclyJvFkTWNSN0N4r98t7oRrWw";
        IConvertitore convertitore;
        ValutaFactory factory;

        public BitfinexExchangeTests()
        {
            convertitore = new TestConvertitore();
            factory = new ValutaFactory(convertitore);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void Costruttore_SenzaPrivata_Eccezione()
        {
            BitfinexExchange explorer = new BitfinexExchange(pubKey, null, factory);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void Costruttore_SenzaPubblica_Eccezione()
        {
            BitfinexExchange explorer = new BitfinexExchange(null, privKey, factory);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void Costruttore_PrivataVuota_Eccezione()
        {
            BitfinexExchange explorer = new BitfinexExchange(pubKey, "", factory);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void Costruttore_PubblicaVuota_Eccezione()
        {
            BitfinexExchange explorer = new BitfinexExchange("", privKey, factory);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void Costruttore_FactoryNull_Eccezione()
        {
            BitfinexExchange explorer = new BitfinexExchange("", privKey, null);
        }

        [TestMethod]
        public void ScaricaFondi_ChiaviOk_Array2Valute()
        {
            BitfinexExchange explorer = new BitfinexExchange(pubKey, privKey, factory);
            List<Fondo> ris = explorer.ScaricaFondi().Result;
            Assert.AreEqual(ris.Count, 2);
        }

        [TestMethod]
        [ExpectedException(typeof(AggregateException))]
        public void ScaricaFondi_ChiaviErrate_Eccezzione()
        {
            BitfinexExchange explorer = new BitfinexExchange("blabla", "blabla", factory);
            List<Fondo> ris = explorer.ScaricaFondi().Result;
        }
    }
}
