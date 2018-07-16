using Criptovalute;
using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace BitMoneto
{
    internal class CambioConverter : IValueConverter
    {
        string _simboloCambio;
        public CambioConverter(string simboloCambio)
        {
            _simboloCambio = simboloCambio;
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Fondo fondo = value as Fondo;
            if (fondo.Valuta.Cambi != null && fondo.Valuta.Cambi.TryGetValue(_simboloCambio, out decimal cambio))
                return fondo.Quantità * cambio;
            else
                return "Cambio Non Disponibile";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}