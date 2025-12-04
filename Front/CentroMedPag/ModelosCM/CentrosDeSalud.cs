using System;
using System.Collections.Generic;
using System.Text;

namespace Front.CentroMedPag.ModelosCM
{
    public class CentrosDeSalud
    {
        private int id_centro;
        private CategoriaCentro id_categoria;
        private string institucion;
        private string direccion;
        private string link;

        public List<TelefonosCentro> Telefonos { get; set; }
        public bool EsFavorito { get; set; } = false;


        public int Id_centro
        {
            get { return id_centro; }
            set { id_centro = value; }
        }
        public CategoriaCentro Id_Categoria
        {
            get { return id_categoria; }
            set { id_categoria = value; }
        }
        public string Institucion
        {
            get { return institucion; }
            set { institucion = value; }
        }
        public string Direccion
        {
            get { return direccion; }
            set { direccion = value; }
        }
        public string Link
        {
            get { return link; }
            set { link = value; }
        }

        public CentrosDeSalud(int pId_centro, CategoriaCentro pid_Categoria, string pInstitucion, string pDireccion, string pLink)
        {
            id_centro = pId_centro;
            id_categoria = pid_Categoria;
            institucion = pInstitucion;
            direccion = pDireccion;
            link = pLink;
            Telefonos = new List<TelefonosCentro>();
        }
    }
}
