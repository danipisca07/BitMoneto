using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Criptovalute
{
    public interface IBlockchain
    {
        String Nome { get; }
        Task<Portafoglio> ScaricaPortafoglio();
    }
}
