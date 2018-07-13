using System;
using System.Collections.Generic;
using System.Text;

namespace Criptovalute
{
    public class Portafoglio
    {
        public Portafoglio(string indirizzo, Valuta valuta)
        {
            Indirizzo = indirizzo ?? throw new ArgumentNullException(nameof(indirizzo));
            Valuta = valuta ?? throw new ArgumentNullException(nameof(valuta));
        }

        public String Indirizzo { get; }
        public Valuta Valuta { get; }

        public override bool Equals(object obj)
        {
            var portafoglio = obj as Portafoglio;
            return portafoglio != null &&
                   Indirizzo == portafoglio.Indirizzo &&
                   EqualityComparer<Valuta>.Default.Equals(Valuta, portafoglio.Valuta);
        }

        public override int GetHashCode()
        {
            var hashCode = -883361895;
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Indirizzo);
            hashCode = hashCode * -1521134295 + EqualityComparer<Valuta>.Default.GetHashCode(Valuta);
            return hashCode;
        }
    }

}
