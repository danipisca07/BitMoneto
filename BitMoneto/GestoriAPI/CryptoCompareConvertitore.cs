using Criptovalute;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web.Helpers;

namespace GestoriAPI
{
    public class CryptoCompareConvertitore : IConvertitore
    {
        private static readonly String INDIRIZZO = "https://min-api.cryptocompare.com/data/";
        private String[] _simboliConversioniDefault;
        private dynamic _listaSimboli = null;

        public CryptoCompareConvertitore()
        {
            _simboliConversioniDefault = new String[] { "BTC", "ETH", "EUR", "USD" };
        }

        public CryptoCompareConvertitore(String[] simboliConversioniDefault)
        {
            _simboliConversioniDefault = simboliConversioniDefault;
        }

        public async Task<Dictionary<String, decimal>> ScaricaCambi(String simboloValuta)
        {
            return await ScaricaCambi(simboloValuta, _simboliConversioniDefault);
        }
        public async Task<Dictionary<String, decimal>> ScaricaCambi(String simboloValuta, String[] simboliConversioni)
        {
            HttpClient client = new HttpClient()
            {
                BaseAddress = new Uri(INDIRIZZO)
            };
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            String listaConversioni = simboliConversioni.Select(a => a.ToString()).Aggregate((i, j) => i + "," + j);
            HttpResponseMessage risposta = await client.GetAsync("price?tsyms="+listaConversioni+"&fsym="+ simboloValuta);
            if (risposta.IsSuccessStatusCode)
            {
                Dictionary<String, decimal> cambi = new Dictionary<string, decimal>();
                String contenuto = await risposta.Content.ReadAsStringAsync();
                var json = Json.Decode(contenuto);
                
                foreach(String valuta in simboliConversioni)
                {
                    if (!Decimal.TryParse(json[valuta].ToString(), out decimal cambio))
                    {
                        throw new EccezioneApi("Valore non valido");
                    }
                    cambi.Add(valuta, cambio);
                }

                return cambi;
            }
            else
            {
                throw new EccezioneApi("Errore chiamata API, codice:" + risposta.StatusCode);
            }
        }

        private async Task ScaricaSimboli()
        {
            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri(INDIRIZZO);
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                HttpResponseMessage risposta = await client.GetAsync("all/coinlist");
                if (risposta.IsSuccessStatusCode)
                {
                    String contenuto = await risposta.Content.ReadAsStringAsync();
                    var json = Json.Decode(contenuto);

                    _listaSimboli = json["Data"];
                }
                else
                {
                    throw new EccezioneApi("Errore chiamata API, codice:" + risposta.StatusCode);
                }
            }
        }

        public async Task<String> NomeValutaDaSimbolo(String simbolo)
        {
            if (_listaSimboli == null)
                await ScaricaSimboli();
            dynamic metadata = _listaSimboli[simbolo];

            //A volte il simbolo delle criptovalute anziche di 4 lettere è stato tagliato a 3 per rispettare lo standard ISO, quindi se non trovo corrispondenza con il simbolo intero provo a ridurlo a tre lettere
            if (metadata == null && simbolo.Length > 3 && _listaSimboli[simbolo.Substring(0, 3)] != null)
                metadata = _listaSimboli[simbolo.Substring(0, 3)];

            if (metadata == null)
                throw new EccezioneApi("Errore: valuta sconosciuta");

            return metadata["CoinName"];
        }


    }
}
