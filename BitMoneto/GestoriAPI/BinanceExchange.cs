using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Binance.API.Csharp.Client;
using Binance.API.Csharp.Client.Models.Market;
using Criptovalute;




namespace GestoriAPI
{
    public class BinanceExchange : Criptovalute.Exchange
    {
        private ApiClient apiClient;
        public BinanceExchange(String publicKey, String privateKey)
        {
            if (publicKey == null || publicKey == "" || privateKey == null || privateKey == "")
                throw new ArgumentException("chiavi non valide!");
            apiClient = new ApiClient(publicKey, privateKey);
        }

        public async Task<List<Valuta>> ScaricaFondi()
        {
            List<Valuta> fondi = new List<Valuta>();
            try
            {
                var binanceClient = new BinanceClient(apiClient);
                AccountInfo ris = await binanceClient.GetAccountInfo();
                Binance.API.Csharp.Client.Models.Market.Balance[] risArray = ris.Balances.ToArray();

                for (int i = 0; i < risArray.Length; i++)
                {
                    if (risArray[i].Free != 0 || risArray[i].Locked != 0)
                    {
                        decimal quantità = Convert.ToDecimal(risArray[i].Free + risArray[i].Locked);
                        fondi.Add(new Valuta(risArray[i].Asset, risArray[i].Asset, quantità));
                    }
                }
            }
            catch(Exception e)
            {
                throw new EccezioneApi("Errore durante il collegamento: " + e.Message);
            }
            return fondi;
        }
    }
}
