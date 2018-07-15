using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Criptovalute
{
    public class ValutaFactory
    {
        private Convertitore _convertitore;
        private Dictionary<String, Valuta> _valute;

        public ValutaFactory(Convertitore convertitore)
        {
            _convertitore = convertitore;
            _valute = new Dictionary<string, Valuta>();
        }


        public Valuta OttieniValuta(String simbolo)
        {
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
