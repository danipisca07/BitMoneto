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
    public class BinanceExchange : Criptovalute.IExchange
    {
        private readonly ApiClient _apiClient;
        private readonly ValutaFactory _factory;

        public string Nome { get { return "Binance"; } }

        public BinanceExchange(string publicKey, string privateKey, ValutaFactory factory)
        {
            if (publicKey == null || publicKey == "" || privateKey == null || privateKey == "")
                throw new ArgumentException("chiavi non valide!");
            _apiClient = new ApiClient(publicKey, privateKey);
            _factory = factory ?? throw new ArgumentException("factory non può essere null!");
        }
        
        public async Task<List<Fondo>> ScaricaFondi()
        {
            List<Fondo> fondi = new List<Fondo>();
            try
            {
                BinanceClient binanceClient = new BinanceClient(_apiClient);
                AccountInfo ris = await binanceClient.GetAccountInfo();
                Binance.API.Csharp.Client.Models.Market.Balance[] risArray = ris.Balances.ToArray();

                for (int i = 0; i < risArray.Length; i++)
                {
                    if (risArray[i].Free != 0 || risArray[i].Locked != 0)
                    {
                        decimal quantità = Convert.ToDecimal(risArray[i].Free + risArray[i].Locked);
                        Valuta valuta = _factory.OttieniValuta(risArray[i].Asset);
                        Fondo fondo = new Fondo(valuta, quantità);
                        fondi.Add(fondo);
                    }
                }
            }
            catch(Exception e)
            {
                throw new EccezioneApi("Errore durante il collegamento: " + e.Message);
            }
            return fondi;
        }

        public override bool Equals(object obj)
        {
            var exchange = obj as BinanceExchange;
            return exchange != null &&
                   _apiClient.Equals(exchange._apiClient) &&
                   _factory.Equals(exchange._factory);
        }

        public override int GetHashCode()
        {
            var hashCode = -1016245487;
            hashCode = hashCode * -1521134295 + _apiClient.GetHashCode();
            hashCode = hashCode * -1521134295 + _factory.GetHashCode();
            return hashCode;
        }
    }
}
