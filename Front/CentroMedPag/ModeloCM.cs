using System;
using System.Collections.Generic;
using System.Text;

namespace Front.CentroMedPag
{
    public class ModeloCM
    {
        
            private int id_centro;
            private string categoria;
            private string institucion;
            private string direccion;
            private string telefono;
            private Uri link;

            public int Id_centro
            {
                get { return id_centro; }
                set { id_centro = value; }
            }
            public string Categoria
            {
                get { return categoria; }
                set { categoria = value; }
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
            public string Telefono
            {
                get { return telefono; }
                set { telefono = value; }
            }
            public Uri Link
            {
                get { return link; }
                set { link = value; }
            }

            public ModeloCM(int pId_centro, string pCategoria, string pInstitución, string pDireccion, string pTelefono, Uri pLink)
            {
                id_centro = pId_centro;
                categoria = pCategoria;
                institucion = pInstitución;
                direccion = pDireccion;
                telefono = pTelefono;
                link = pLink;

            }
        }
    }

