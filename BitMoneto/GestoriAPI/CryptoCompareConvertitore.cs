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
    public static class CryptoCompareConvertitore
    {
        private static readonly String INDIRIZZO = "https://min-api.cryptocompare.com/data/";

        public async static Task<Dictionary<String, decimal>> ScaricaCambi(String simboloValuta)
        {
            return await ScaricaCambi(simboloValuta, new String[] { "BTC", "ETH", "EUR", "USD" });
        }
        public async static Task<Dictionary<String, decimal>> ScaricaCambi(String simboloValuta, String[] simboliConversioni)
        {
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri(INDIRIZZO);
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            String listaConversioni = simboliConversioni.Select(a => a.ToString()).Aggregate((i, j) => i + "," + j);
            HttpResponseMessage risposta = await client.GetAsync("price?tsyms="+listaConversioni+"&fsym="+ simboloValuta);
            if (risposta.IsSuccessStatusCode)
            {
                Dictionary<String, decimal> cambi = new Dictionary<string, decimal>();
                String contenuto = await risposta.Content.ReadAsStringAsync();
                var json = Json.Decode(contenuto);

                decimal cambio;
                foreach(String valuta in simboliConversioni)
                {
                    if (!Decimal.TryParse(json[valuta].ToString(), out cambio))
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
    }
}
