using Criptovalute;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bitmoneto.UnitTests
{
    class TestBlockchain : IBlockchain
    {
        private Portafoglio _portafoglio;

        public TestBlockchain()
        {
            Nome = "TestBlockchain";
            _portafoglio = new Portafoglio("TestIndirizzo");
        }

        public string Nome { get; }

        public Portafoglio Portafoglio { get { return _portafoglio; } }

        public async Task<Portafoglio> ScaricaPortafoglio()
        {
            await Task.Run(() =>
            {
                Portafoglio.Fondo = new Fondo(new Valuta("Bitcoin", "BTC"), 1);
            });
            return Portafoglio;
        }
    }
}
