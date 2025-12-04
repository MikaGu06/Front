using System;
using System.Text.RegularExpressions;

namespace Front.SigVitalesPag
{
    internal class ValidacionesSignosVitales
    {
        // Devuelve además 'alerta' (vacío si no hay alerta)
        public static (int ritmo, int sistolica, int diastolica, int oxigeno, decimal temperatura, string alerta)
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


            // VALIDACIONES SUAVES 

            string alerta = string.Empty;

            // Temperatura:
            if (temperaturaValor >= 38.0m && temperaturaValor <= 41.0m)
            {
                alerta = $"Fiebre detectada ({temperaturaValor:F1} °C). Considera contactar a tu médico si persiste o hay otros síntomas.";
            }

            // Ritmo cardíaco:
            if (string.IsNullOrEmpty(alerta))
            {
                if (ritmoCardiaco <= 50 && ritmoCardiaco >= 31)
                    alerta = $"Ritmo cardíaco bajo ({ritmoCardiaco} bpm). Si tienes mareos o debilidad, contacta a tu médico.";
                else if (ritmoCardiaco >= 100 && ritmoCardiaco <= 249)
                    alerta = $"Ritmo cardíaco alto ({ritmoCardiaco} bpm). Si sientes palpitaciones, mareo o dolor en el pecho, consulta con un profesional.";
            }

            // Oxigenación:
            if (string.IsNullOrEmpty(alerta))
            {
                if (oxigenacion < 94 && oxigenacion <= 51)
                    alerta = $"Oxigenación baja ({oxigenacion}%). Si notas falta de aire, considera buscar atención médica.";
            }

            // Presión arterial:
            if (string.IsNullOrEmpty(alerta))
            {
                if (sistolica >= 140 || diastolica >= 90)
                    alerta = $"Presión arterial elevada ({sistolica}/{diastolica} mmHg). Si persiste, contacta a tu médico.";
                else if (sistolica <= 90 || diastolica <= 60)
                    alerta = $"Presión arterial baja ({sistolica}/{diastolica} mmHg). Si tienes síntomas, consulta con un profesional de salud.";
            }


            // VALIDACIONES ESTRICTAS

            // 6. Validación de rango
            if (ritmoCardiaco <= 30 || ritmoCardiaco >= 250)
                throw new ArgumentException("Ritmo Cardíaco fuera de rango (31–249).");

            if (sistolica <= diastolica)
                throw new ArgumentException("La Sistólica debe ser mayor que la Diastólica.");

            if (oxigenacion <= 50 || oxigenacion > 100)
                throw new ArgumentException("Oxigenación fuera de rango (51–100).");

            if (temperaturaValor <= 32 || temperaturaValor >= 42)
                throw new ArgumentException("Temperatura fuera de rango (32.1–41.9).");

            return (ritmoCardiaco, sistolica, diastolica, oxigenacion, temperaturaValor, alerta);
        }
    }
}
