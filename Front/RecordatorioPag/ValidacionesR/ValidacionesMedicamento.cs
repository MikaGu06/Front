using System;
using System.Collections.Generic;
using System.Text;

using System;

namespace Front.RecordatorioPag.ValidacionesR
{
    public static class ValidacionesMedicamento
    {
        public static void ValidarNombre(string nombre)
        {
            if (string.IsNullOrWhiteSpace(nombre))
                throw new InvalidOperationException("El nombre del medicamento es obligatorio.");
        }

        public static void ValidarDosis(string dosis)
        {
            if (string.IsNullOrWhiteSpace(dosis))
                throw new InvalidOperationException("La dosis es obligatoria.");
            if (!decimal.TryParse(dosis, out _))
                throw new InvalidOperationException("La dosis debe ser un número válido.");
        }

        public static void ValidarUnidad(string unidad)
        {
            if (string.IsNullOrWhiteSpace(unidad))
                throw new InvalidOperationException("Debes seleccionar la unidad de dosis.");
        }
    }
}
