using System;
using System.Collections.Generic;
using System.Text;

namespace Front.RecordatorioPag.ValidacionesR
{
    public static class ValidacionesRecordatorio
    {
        public static void ValidarFecha(DateTime? fecha)
        {
            if (fecha == null)
                throw new InvalidOperationException("Selecciona un día de inicio.");
        }

        public static TimeSpan ValidarHora(string hora)
        {
            if (!TimeSpan.TryParseExact(hora.Trim(), "hh\\:mm", null, out TimeSpan resultado))
                throw new InvalidOperationException("La hora debe estar en formato HH:mm.");
            return resultado;
        }

        public static void ValidarFrecuencia(object selectedItem)
        {
            if (selectedItem == null)
                throw new InvalidOperationException("Selecciona la frecuencia.");
        }

        public static void ValidarMedicamentoExistente(int idMed)
        {
            if (idMed == 0)
                throw new InvalidOperationException("El medicamento no existe en la base de datos.");
        }
    }
}