using System;
using System.Text.RegularExpressions;

namespace Front.SigVitalesPag
{
    internal class ValidacionesSignosVitales
    {
        public static (int ritmo, int sistolica, int diastolica, int oxigeno, decimal temperatura)
            Validar(string ritmo, string presion, string oxigeno, string temperatura)
        {
            // 1. Ningún campo vacío
            if (string.IsNullOrWhiteSpace(ritmo) ||
                string.IsNullOrWhiteSpace(presion) ||
                string.IsNullOrWhiteSpace(oxigeno) ||
                string.IsNullOrWhiteSpace(temperatura))
                throw new ArgumentException("Todos los campos deben ser completados.");

            // 2. No espacios internos
            if (ritmo.Contains(" ") || presion.Contains(" ") || oxigeno.Contains(" ") || temperatura.Contains(" "))
                throw new ArgumentException("Ningún campo puede contener espacios internos.");

            // 3. Validación de formato numérico
            if (!Regex.IsMatch(ritmo, "^[0-9]+$"))
                throw new ArgumentException("Ritmo Cardíaco debe ser numérico.");

            if (!Regex.IsMatch(oxigeno, "^[0-9]+$"))
                throw new ArgumentException("Oxigenación debe ser numérica.");

            if (!Regex.IsMatch(temperatura, "^[0-9]+(\\.[0-9]+)?$"))
                throw new ArgumentException("Temperatura debe ser un número válido (puede usar punto).");

            // 4. Formato de presión "120-80"
            if (!Regex.IsMatch(presion, "^[0-9]+-[0-9]+$"))
                throw new ArgumentException("La Presión Arterial debe tener el formato 120-80.");

            string[] partes = presion.Split('-');
            int sistolica = int.Parse(partes[0]);
            int diastolica = int.Parse(partes[1]);

            // 5. Conversión
            int ritmoCardiaco = int.Parse(ritmo);
            int oxigenacion = int.Parse(oxigeno);
            decimal temperaturaValor = decimal.Parse(temperatura, System.Globalization.CultureInfo.InvariantCulture);

            // 6. Validación de rango
            if (ritmoCardiaco <= 30 || ritmoCardiaco >= 250)
                throw new ArgumentException("Ritmo Cardíaco fuera de rango (31–249).");

            if (sistolica <= diastolica)
                throw new ArgumentException("La Sistólica debe ser mayor que la Diastólica.");

            if (oxigenacion <= 50 || oxigenacion > 100)
                throw new ArgumentException("Oxigenación fuera de rango (51–100).");

            if (temperaturaValor <= 32 || temperaturaValor >= 42)
                throw new ArgumentException("Temperatura fuera de rango (32.1–41.9).");

            return (ritmoCardiaco, sistolica, diastolica, oxigenacion, temperaturaValor);
        }
    }
}
