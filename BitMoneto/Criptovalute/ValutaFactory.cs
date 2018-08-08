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
            _convertitore = convertitore ?? throw new ArgumentException("Convertitore non può essere null!");
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

        public async Task AggiornaCambi()
        {
            //Effettuo una copia della lista delle chiavi sulla quale poi itero per aggiornarne i valori(non posso iterare direttamente sul dizionario
            // perchè modificando la collezione non posso iterarci sopra
            string[] chiavi = new string[_valute.Keys.Count];
            _valute.Keys.CopyTo(chiavi, 0);
            foreach (string chiave in chiavi)
            {
                _valute[chiave] = await TrovaValuta(chiave);
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
