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
            Indirizzo = indirizzo;
            _factory = factory ?? throw new ArgumentException("factory non può essere null!");
        }

        public async Task<Portafoglio> ScaricaPortafoglio()
        {
            HttpClient client = new HttpClient() { BaseAddress = new Uri("https://api.etherscan.io/api") };
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            HttpResponseMessage risposta = await client.GetAsync("?module=account&action=balance&address=" + Indirizzo);
            if (risposta.IsSuccessStatusCode)
            {
                String contenuto = await risposta.Content.ReadAsStringAsync();
                var json = Json.Decode(contenuto);
                
                if (!Decimal.TryParse(json["result"], out decimal quantità))
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
                throw new EccezioneApi("EthereumEtherscan(ScaricaPortafoglio()):Errore chiamata API, codice:" + risposta.StatusCode);
            }
        }

        public override bool Equals(object obj)
        {
            var etherscan = obj as EthereumEtherscan;
            return etherscan != null &&
                   Indirizzo == etherscan.Indirizzo &&
                   Nome == etherscan.Nome;
        }

        public override int GetHashCode()
        {
            var hashCode = 1748475525;
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Indirizzo);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Nome);
            return hashCode;
        }
    }
}
