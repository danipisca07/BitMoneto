using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Criptovalute
{
    public class Fondo
    {
        private const decimal MARGINEQUANTITAEQUALS = (decimal)0.00000000001;

        public Valuta Valuta { get; }
        public decimal Quantità { get; }

        public Fondo(Valuta valuta, decimal quantità)
        {
            Valuta = valuta ?? throw new ArgumentNullException(nameof(valuta));
            Quantità = quantità;
        }

        public override bool Equals(object obj)
        {
            var fondo = obj as Fondo;
            return fondo != null &&
                   EqualityComparer<Valuta>.Default.Equals(Valuta, fondo.Valuta) &&
                   (Math.Abs(Quantità - fondo.Quantità) < MARGINEQUANTITAEQUALS);
        }

        public override int GetHashCode()
        {
            var hashCode = 814187495;
            hashCode = hashCode * -1521134295 + EqualityComparer<Valuta>.Default.GetHashCode(Valuta);
            hashCode = hashCode * -1521134295 + Quantità.GetHashCode();
            return hashCode;
        }
    }
}
