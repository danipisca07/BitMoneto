using System;
using System.Collections.Generic;
using System.Text;

namespace Criptovalute
{
    public class Valuta
    {
        private const decimal MARGINEQUANTITAEQUALS = (decimal)0.00000000001;

        public string Nome { get; }

        public string Simbolo { get; }

        public decimal Quantità { get; }

        public Valuta(string nome, string simbolo, decimal quantità)
        {
            Nome = nome ?? throw new ArgumentNullException(nameof(nome));
            Simbolo = simbolo ?? throw new ArgumentNullException(nameof(simbolo));
            Quantità = quantità;
        }

        public override bool Equals(object obj)
        {
            var valuta = obj as Valuta;
            return valuta != null &&
                   Nome == valuta.Nome &&
                   Simbolo == valuta.Simbolo &&
                   (Math.Abs(Quantità - valuta.Quantità) < MARGINEQUANTITAEQUALS);
        }

        public override int GetHashCode()
        {
            var hashCode = -1169125267;
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Nome);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Simbolo);
            hashCode = hashCode * -1521134295 + Quantità.GetHashCode();
            return hashCode;
        }
    }
}
