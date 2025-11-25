using System;
using System.Collections.Generic;
using System.Text;

namespace Front.RecordatorioPag.ModelosR
{
    public class Recordatorio
    {
        private int id_recordatorio;
        private DateTime fecha;
        private DateTime hora_inicio;
        private int frecuencia;
        private bool estado;
        private string medicamentoNombre;
        private DateTime lastFired = DateTime.MinValue;

        public int Id_recordatorio
        {
            get { return id_recordatorio; }
            set { id_recordatorio = value; }
        }

        public DateTime Fecha
        {
            get { return fecha; }
            set { fecha = value; }
        }

        public DateTime Hora_inicio
        {
            get { return hora_inicio; }
            set { hora_inicio = value; }
        }

        public int Frecuencia
        {
            get { return frecuencia; }
            set { frecuencia = value; }
        }

        public bool Estado
        {
            get { return estado; }
            set { estado = value; }
        }

        public string MedicamentoNombre
        {
            get { return medicamentoNombre; }
            set { medicamentoNombre = value; }
        }

        public DateTime LastFired
        {
            get { return lastFired; }
            set { lastFired = value; }
        }
        public string EstadoBoton => Estado ? "Activo" : "Inactivo";
        public string FrecuenciaTexto => $"Cada {Frecuencia} horas";
        public Recordatorio(int pid, DateTime pfecha, DateTime phoraInicio, int pfrecuencia, bool pestado, string pmedicamentoNombre)
        {
            Id_recordatorio = pid;
            Fecha = pfecha;
            Hora_inicio = phoraInicio;
            Frecuencia = pfrecuencia;
            Estado = pestado;
            MedicamentoNombre = pmedicamentoNombre;
        }

        public override string ToString() => $"{MedicamentoNombre} - {Fecha.ToShortDateString()} {Hora_inicio.ToShortTimeString()}";
    }
}