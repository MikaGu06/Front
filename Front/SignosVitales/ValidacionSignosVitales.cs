using System;
using System.Text.RegularExpressions;

namespace Front.SignosVitales
{
    public static class ValidacionSignosVitales
    {
        public static void Validar(string ritmo, string presion, string oxigeno, string temperatura)
        {
            // 1. No vacíos
            if (string.IsNullOrEmpty(ritmo) || string.IsNullOrEmpty(presion) ||
                string.IsNullOrEmpty(oxigeno) || string.IsNullOrEmpty(temperatura))
            {
                throw new ArgumentException("Todos los campos deben ser completados.");
            }

            // 2. Sin espacios internos
            if (ritmo.Contains(" ") || presion.Contains(" ") ||
                oxigeno.Contains(" ") || temperatura.Contains(" "))
            {
                throw new ArgumentException("Ningún campo puede contener espacios internos.");
            }

            // 3. Validación de formatos
            if (!Regex.IsMatch(ritmo, "^[0-9]+$") ||
                !Regex.IsMatch(oxigeno, "^[0-9]+$") ||
                !Regex.IsMatch(temperatura, "^[0-9]+(\\.[0-9]+)?$"))
            {
                throw new ArgumentException("Ritmo Cardíaco, Oxigenación y Temperatura deben ser numéricos. Temperatura puede incluir un punto.");
            }

            // Presión arterial formato NNN-NNN
            if (!Regex.IsMatch(presion, "^[0-9]+-[0-9]+$"))
            {
                throw new ArgumentException("La Presión Arterial debe escribirse en formato 120-80.");
            }

            // Partes de presión
            string[] partesPresion = presion.Split('-');
            if (!int.TryParse(partesPresion[0], out int sistolica) ||
                !int.TryParse(partesPresion[1], out int diastolica))
            {
                throw new ArgumentException("Error al procesar la Presión Arterial. Use el formato NNN-NNN.");
            }

            // 4. Conversión
            int ritmoCardiaco = int.Parse(ritmo);
            int oxigenacion = int.Parse(oxigeno);
            double temperaturaVal = double.Parse(temperatura, System.Globalization.CultureInfo.InvariantCulture);

            // 5. Validaciones de rango
            if (ritmoCardiaco <= 30 || ritmoCardiaco >= 250)
                throw new ArgumentException("Ritmo Cardíaco fuera de rango válido (31 a 249).");

            if (sistolica <= 70 || sistolica >= 250)
                throw new ArgumentException("Presión Sistólica fuera de rango válido (71 a 249).");

            if (diastolica <= 40 || diastolica >= 150)
                throw new ArgumentException("Presión Diastólica fuera de rango válido (41 a 149).");

            if (sistolica <= diastolica)
                throw new ArgumentException("La Presión Sistólica debe ser mayor que la Diastólica.");

            if (oxigenacion <= 50 || oxigenacion > 100)
                throw new ArgumentException("Oxigenación fuera de rango válido (51 a 100).");

            if (temperaturaVal <= 32.0 || temperaturaVal >= 42.0)
                throw new ArgumentException("Temperatura fuera de rango válido (32.1 a 41.9).");
        }
    }
}
