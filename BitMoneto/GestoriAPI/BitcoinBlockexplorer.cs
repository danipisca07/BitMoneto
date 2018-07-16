using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Criptovalute;

namespace GestoriAPI
{
    public class BitcoinBlockexplorer : Criptovalute.IBlockchain
    {
        private readonly ValutaFactory _factory;
        public String Indirizzo { get; }

        public string Nome { get { return "Bitcoin(" + Indirizzo + ")"; } }

        public BitcoinBlockexplorer(String indirizzo,ValutaFactory factory)
        {
            if (indirizzo == null || indirizzo == "")
                throw new ArgumentException("Indirizzo non valido");
            if(factory == null)
                throw new ArgumentException("factory non può essere null");
            Indirizzo = indirizzo;
            _factory = factory;
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
                Valuta valuta = _factory.OttieniValuta("BTC");
                Portafoglio portafoglio = new Portafoglio(Indirizzo, new Fondo(valuta, quantità));
                return portafoglio;
            }
            else
            {
                throw new EccezioneApi("Errore chiamata API, codice:" + risposta.StatusCode);
            }
        }

        public override bool Equals(object obj)
        {
            var blockexplorer = obj as BitcoinBlockexplorer;
            return blockexplorer != null && Indirizzo == blockexplorer.Indirizzo;
        }

        public override int GetHashCode()
        {
            var hashCode = -904650435;
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Indirizzo);
            return hashCode;
        }
    }
}
