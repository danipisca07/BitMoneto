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
        private List<Fondo> _fondi = null;

        public string Nome { get { return "Binance"; } }

        public string ChiavePubblica { get { return _apiClient._apiKey; } }   

        public string ChiavePrivata { get { return _apiClient._apiSecret; } }

        public List<Fondo> Fondi { get { return _fondi; } }

        public Dictionary<string,decimal> Totali
        {
            get
            {
                Dictionary<string, decimal> totali = new Dictionary<string, decimal>();
                foreach(Fondo fondo in Fondi)
                {
                    foreach(KeyValuePair<string,decimal> cambio in fondo.Cambi)
                    {
                        if (!totali.TryGetValue(cambio.Key, out decimal totale))
                            totale = 0;
                        totali.Add(cambio.Key, totale + cambio.Value);
                    }
                }
                return totali;
            }
        }

        public BinanceExchange(string publicKey, string privateKey, ValutaFactory factory)
        {
            if (publicKey == null || publicKey.Length != 64 || privateKey == null || privateKey.Length != 64)
                throw new ArgumentException("Binance: chiavi non valide!");
            _apiClient = new ApiClient(publicKey, privateKey);
            _factory = factory ?? throw new ArgumentException("factory non può essere null!");
        }
        
        public async Task<List<Fondo>> ScaricaFondi()
        {
            try
            {
                _fondi = new List<Fondo>();
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
                        _fondi.Add(fondo);
                    }
                }
            }
            catch(Exception e)
            {
                throw new EccezioneApi("BinanceExchange(ScaricaFondi()): Errore durante il collegamento: " + e.Message);
            }
            return _fondi;
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
