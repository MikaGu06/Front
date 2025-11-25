using System;
using System.Collections.Generic;
using System.Text;

namespace Front.RecordatorioPag.ModelosR
{
    public class Medicamento
    {
        private int id_medicamento;
        private string nombre;
        private string descripcion;
        private decimal dosis;
        private string unidad;
        public int Id_medicamento
        {
            get { return id_medicamento; }
            set { id_medicamento = value; }
        }
        public string Nombre
        {
            get { return nombre; }
            set { nombre = value; }
        }
        public string Descripcion
        {
            get { return descripcion; }
            set { descripcion = value; }
        }
        public decimal Dosis
        {
            get { return dosis; }
            set { dosis = value; }
        }
        public string Unidad
        {
            get { return unidad; }
            set { unidad = value; }
        }

        public Medicamento(int pid, string pnombre, string pdescripcion, decimal pdosis, string punidad)
        {
            Id_medicamento = pid;
            Nombre = pnombre;
            Descripcion = pdescripcion;
            Dosis = pdosis;
            Unidad = punidad;
        }

        public override string ToString() => $"{Dosis} {Unidad}";

    }
}
