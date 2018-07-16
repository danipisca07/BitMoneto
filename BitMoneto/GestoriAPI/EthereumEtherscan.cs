using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web.Helpers;
using Criptovalute;

namespace GestoriAPI
{
    public class EthereumEtherscan : Criptovalute.IBlockchain
    {
        private ValutaFactory _factory;
        public String Indirizzo { get; }
        public string Nome { get { return "Ethereum(" + Indirizzo + ")"; } }

        public EthereumEtherscan(String indirizzo, ValutaFactory factory)
        {
            if (indirizzo == null || indirizzo == "")
                throw new ArgumentException("Indirizzo non valido");
            if(factory == null)
                throw new ArgumentException("factory non può essere null!");
            Indirizzo = indirizzo;
            _factory = factory;
        }

        public async Task<Portafoglio> ScaricaPortafoglio()
        {
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri("https://api.etherscan.io/api");
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            HttpResponseMessage risposta = await client.GetAsync("?module=account&action=balance&address=" + Indirizzo);
            if (risposta.IsSuccessStatusCode)
            {
                String contenuto = await risposta.Content.ReadAsStringAsync();
                var json = Json.Decode(contenuto);

                decimal quantità;
                if (!Decimal.TryParse(json["result"], out quantità))
                {
                    throw new EccezioneApi("Valore non valido");
                }
                quantità /= (decimal)Math.Pow(10,18); //Il valore restituito è multiplo, lo riporto a valore unitario
                Valuta valuta = _factory.OttieniValuta("ETH");
                Fondo fondo = new Fondo(valuta, quantità);
                Portafoglio portafoglio = new Portafoglio(Indirizzo, fondo);
                return portafoglio;
            }
            else
            {
                throw new EccezioneApi("Errore chiamata API, codice:" + risposta.StatusCode);
            }
        }
    }
}
