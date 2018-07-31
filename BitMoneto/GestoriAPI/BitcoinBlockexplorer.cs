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
        public Portafoglio Portafoglio { get; }

        public string Nome { get { return "Bitcoin(" + Portafoglio.Indirizzo + ")"; } }

        public BitcoinBlockexplorer(String indirizzo,ValutaFactory factory)
        {
            if (indirizzo == null || indirizzo == "")
                throw new ArgumentException("Indirizzo non valido");
            Portafoglio = new Portafoglio(indirizzo);
            _factory = factory ?? throw new ArgumentException("factory non può essere null");
        }

        public async Task<Portafoglio> ScaricaPortafoglio()
        {
            HttpClient client = new HttpClient
            {
                BaseAddress = new Uri("https://blockexplorer.com/api/addr/")
            };
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            HttpResponseMessage risposta = await client.GetAsync(Portafoglio.Indirizzo + "/balance");
            if (risposta.IsSuccessStatusCode)
            {
                String contenuto = await risposta.Content.ReadAsStringAsync();
                if (!Decimal.TryParse(contenuto, out decimal quantità))
                {
                    throw new EccezioneApi("Valore non valido");
                }
                quantità /= 100000000; //Il valore restituito è in satoshi(0.00000001 BTC), lo riporto a valore unitario
                Valuta valuta = _factory.OttieniValuta("BTC");
                Portafoglio.Fondo =  new Fondo(valuta, quantità);
                return Portafoglio;
            }
            else
            {
                throw new EccezioneApi("BitcoinBlockExplorer(ScaricaPortafoglio()):Errore chiamata API, codice HTTP:" + risposta.StatusCode);
            }
        }

        public override bool Equals(object obj)
        {
            var blockexplorer = obj as BitcoinBlockexplorer;
            return blockexplorer != null && Portafoglio == blockexplorer.Portafoglio;
        }

        public override int GetHashCode()
        {
            var hashCode = -904650435;
            hashCode = hashCode * -1521134295 + Portafoglio.GetHashCode();
            return hashCode;
        }
    }
}
