using System;
using System.Collections.Generic;
using System.Text;

namespace Front.CentroMedPag.ModelosCM
{
    internal class TelefonosCentro
    {
        private int id_telefono;
        private int id_centro;
        private string telefono;
        private bool tienew;
        private string linkT;
        public int Id_telefono
        {
            get { return id_telefono; }
            set { id_telefono = value; }
        }
        public int Id_centro
        {
            get { return id_centro; }
            set { id_centro = value; }
        }
        public string Telefono
        {
            get { return telefono; }
            set { telefono = value; }
        }
        public bool Tienew
        {
            get { return tienew; }
            set { tienew = value; }
        }
        public string LinkT
        {
            get { return linkT; }
            set { linkT = value; }
        }
        public TelefonosCentro(int pId_telefono, int pId_centro, string pTelefono, bool pTienew, string pLinkT)
        {
            id_telefono = pId_telefono;
            id_centro = pId_centro;
            telefono = pTelefono;
            tienew = pTienew;
            linkT = pLinkT;
        }

    }
}
