using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Criptovalute
{
    public class ValutaFactory
    {
        private IConvertitore _convertitore;
        private Dictionary<String, Valuta> _valute;

        public ValutaFactory(IConvertitore convertitore)
        {
            _convertitore = convertitore;
            _valute = new Dictionary<string, Valuta>();
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public Valuta OttieniValuta(String simbolo)
        {
            simbolo = simbolo.ToUpper();
            if (_valute.TryGetValue(simbolo, out Valuta valuta))
            {
                return valuta;
            }
            else
            {
                valuta = TrovaValuta(simbolo).Result;
                _valute.Add(simbolo, valuta);
                return valuta;
            }
        }

        private async Task<Valuta> TrovaValuta(String simbolo)
        {
            try
            {
                String nome = await _convertitore.NomeValutaDaSimbolo(simbolo);
                Dictionary<String, decimal> cambi = await _convertitore.ScaricaCambi(simbolo);
                return new Valuta(nome, simbolo, cambi);
            }
            catch (EccezioneApi)
            {
                return new Valuta(simbolo, simbolo);
            }
        }

    }
}
