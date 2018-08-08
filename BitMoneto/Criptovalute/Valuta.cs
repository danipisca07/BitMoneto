using System;
using System.Collections.Generic;
using System.Text;

namespace Criptovalute
{
    public class Valuta
    {
        public string Nome { get; }

        public string Simbolo { get; }

        public Dictionary<String, decimal> Cambi { get; set; }
        
        public Valuta(string nome, string simbolo)
        {
            Nome = nome ?? throw new ArgumentNullException(nameof(nome));
            Simbolo = simbolo ?? throw new ArgumentNullException(nameof(simbolo));
        }

        public Valuta(string nome, string simbolo, Dictionary<String, decimal> cambi) 
            : this(nome,simbolo)
        {
            Cambi = cambi;
        }

        public override bool Equals(object obj)
        {
            var valuta = obj as Valuta;
            return valuta != null &&
                   Nome == valuta.Nome &&
                   Simbolo == valuta.Simbolo;
        }

        public override int GetHashCode()
        {
            var hashCode = -1169125267;
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Nome);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Simbolo);
            return hashCode;
        }
    }
}
