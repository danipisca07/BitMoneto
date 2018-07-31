using System;
using System.Collections.Generic;
using System.Text;

namespace Criptovalute
{
    public class Portafoglio
    {
        public Portafoglio(string indirizzo)
        {
            Indirizzo = indirizzo ?? throw new ArgumentNullException(nameof(indirizzo));
        }

        public Portafoglio(string indirizzo, Fondo fondo)
        {
            Indirizzo = indirizzo ?? throw new ArgumentNullException(nameof(indirizzo));
            Fondo = fondo ?? throw new ArgumentNullException(nameof(fondo));
        }

        public String Indirizzo { get; }
        public Fondo Fondo { get; set; }

        public override bool Equals(object obj)
        {
            var portafoglio = obj as Portafoglio;
            return portafoglio != null &&
                   Indirizzo == portafoglio.Indirizzo &&
                   Fondo.Equals(portafoglio.Fondo);
        }

        public override int GetHashCode()
        {
            var hashCode = -883361895;
            hashCode = hashCode * -1521134295 + Indirizzo.GetHashCode();
            hashCode = hashCode * -1521134295 + Fondo.GetHashCode();
            return hashCode;
        }
    }

}
