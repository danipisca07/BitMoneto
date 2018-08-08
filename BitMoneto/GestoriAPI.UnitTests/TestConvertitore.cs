using Criptovalute;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bitmoneto.UnitTests
{
    class TestConvertitore : IConvertitore
    {
        public string[] SimboliConversioni { get { return null; } }

        public async Task<string> NomeValutaDaSimbolo(string simbolo)
        {
            await Task.Run(() => { });
            return simbolo;
        }

        public async Task<Dictionary<string, decimal>> ScaricaCambi(string simboloValuta)
        {
            await Task.Run(() => { });
            return null;
        }
    }
}
