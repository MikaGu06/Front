using System;
using System.Collections.Generic;
using System.Text;

namespace Front.CentroMedPag.ModelosCM
{
    public class CategoriaCentro
    {
        private int id_categoria;
        private string nombre;
        public int Id_categoria
        {
            get { return id_categoria; }
            set { id_categoria = value; }
        }
        public string Nombre
        {
            get { return nombre; }
            set { nombre = value; }
        }
        public CategoriaCentro(int pId_categoria, string pNombre)
        {
            id_categoria = pId_categoria;
            nombre = pNombre;
        }
    }
}
