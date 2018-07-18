using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Criptovalute
{
    public interface IConvertitore
    {
        String[] SimboliConversioni { get; }
        Task<Dictionary<String, decimal>> ScaricaCambi(String simboloValuta);
        Task<String> NomeValutaDaSimbolo(String simbolo);
    }
}