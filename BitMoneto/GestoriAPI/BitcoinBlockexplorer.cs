using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Criptovalute;

namespace GestoriAPI
{
    public class BitcoinBlockexplorer : Criptovalute.Blockchain
    {
        public String Indirizzo { get; }
        public BitcoinBlockexplorer(String indirizzo)
        {
            if (indirizzo == null || indirizzo == "")
                throw new ArgumentException("Indirizzo non valido");
            Indirizzo = indirizzo;
        }

        public async Task<Portafoglio> ScaricaPortafoglio()
        {
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri("https://blockexplorer.com/api/addr/");
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            HttpResponseMessage risposta = await client.GetAsync(Indirizzo + "/balance");
            if (risposta.IsSuccessStatusCode)
            {
                String contenuto = await risposta.Content.ReadAsStringAsync();
                decimal quantità;
                if (!Decimal.TryParse(contenuto, out quantità))
                {
                    throw new EccezioneApi("Valore non valido");
                }
                quantità /= 100000000; //Il valore restituito è in satoshi(0.00000001 BTC), lo riporto a valore unitario
                Portafoglio portafoglio = new Portafoglio(Indirizzo, new Valuta("Bitcoin", "BTC", quantità));
                return portafoglio;
            }
            else
            {
                throw new EccezioneApi("Errore chiamata API, codice:" + risposta.StatusCode);
            }
        }
    }
}
