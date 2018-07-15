using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Criptovalute
{
    public interface Exchange
    {
        Task<List<Fondo>> ScaricaFondi();
    }
}
