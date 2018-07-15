using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Criptovalute
{
    public interface Convertitore
    {
        Task<Dictionary<String, decimal>> ScaricaCambi(String simboloValuta);
        Task<Dictionary<String, decimal>> ScaricaCambi(String simboloValuta, String[] simboliConversioni);
        Task<String> NomeValutaDaSimbolo(String simbolo);
    }
}