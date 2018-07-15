﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bitfinex;
using Criptovalute;

namespace GestoriAPI
{
    public class BitfinexExchange : Criptovalute.Exchange
    {
        private readonly BitfinexApi _apiClient;
        private readonly ValutaFactory _factory;
        public BitfinexExchange(String publicKey, String privateKey, ValutaFactory factory)
        {
            if (publicKey == null || publicKey == "" || privateKey == null || privateKey == "")
                throw new ArgumentException("chiavi non valide!");
            _apiClient = new BitfinexApi(publicKey, privateKey);
            _factory = factory ?? throw new ArgumentException("factory non può essere null!");
        }

        public async Task<List<Fondo>> ScaricaFondi()
        {
            List<Fondo> fondi = new List<Fondo>();
            //L'implementazione delle API utilizzata non è asincrona quindi la rendo asincrona inserendola in un Task con Task.Run()
            await Task.Run(() =>
            {
                try
                {
                    var risArray = _apiClient.GetBalances().ToArray();

                    for (int i = 0; i < risArray.Length; i++)
                    {
                        if (risArray[i].Amount != 0)
                        {
                            decimal quantità = Convert.ToDecimal(risArray[i].Amount);
                            Valuta valuta = _factory.OttieniValuta(risArray[i].Currency);
                            Fondo fondo = new Fondo(valuta, quantità);
                            fondi.Add(fondo);
                        }
                    }
                }
                catch (Exception e)
                {
                    throw new EccezioneApi("Errore durante il collegamento: " + e.Message);
                }
            });
            return fondi;
        }
    }
}
