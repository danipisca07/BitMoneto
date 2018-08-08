using Criptovalute;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bitmoneto.UnitTests
{
    class TestExchange : IExchange
    {
        public TestExchange()
        {
            Nome = "TestExchange";
            ChiavePrivata = "APIPRIVATA";
            ChiavePubblica = "APIPUBBLICA";
        }

        public string Nome { get; }

        public string ChiavePubblica { get; }

        public string ChiavePrivata { get; }

        private List<Fondo> _fondi = null;

        public List<Fondo> Fondi { get { return _fondi; } }

        public async Task<List<Fondo>> ScaricaFondi()
        {
            await Task.Run(() =>
            {
                _fondi = new List<Fondo>();
                Fondo tmp = new Fondo(new Valuta("Bitcoin", "BTC"), 1);
                _fondi.Add(tmp);
                tmp = new Fondo(new Valuta("Ethereum", "ETH"), 10);
                _fondi.Add(tmp);
            });
            return _fondi;
        }        
    }
}
