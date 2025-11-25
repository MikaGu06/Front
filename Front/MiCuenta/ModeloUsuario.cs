using System;
using System.Collections.Generic;
using System.Text;

namespace Front.MiCuenta
{
    public class ModeloUsuario
    {
        private string usuario;
        private string contrasena;
        private string telefono;
        private string nombre;
        private string correo;
        private string direccion;
        private string edad;
        private DateTime? fechaNacimiento; //DateTime tipo Nunable para que permita validar si no se selecciona fecha
        private string genero;
        private string tipoSangre;
        private string rutaFoto;

        public string Usuario
        {
            get { return usuario;  } 
            set { usuario = value; }
        }
        public string Contrasena
        {
            get { return contrasena; }
            set { contrasena = value; }
        }
        public string Telefono
        {
            get { return telefono; }
            set { telefono = value; }
        }
        public string Nombre
        {
            get { return nombre; }
            set { nombre = value; }
        }
        public string Correo
        {
            get { return correo; }
            set { correo = value; }
        }
        public string Direccion
        {
            get { return direccion; }
            set { direccion = value; }
        }
        public string Edad
        {
            get { return edad; }
            set { edad = value; }
        }
        public DateTime? FechaNacimiento
        {
            get { return fechaNacimiento; }
            set { fechaNacimiento = value; }
        }
        public string Genero
        {
            get { return genero; }
            set { genero = value; }
        }
        public string TipoSangre
        {
            get { return tipoSangre; }
            set { tipoSangre = value; }
        }
        public string RutaFoto
        {
            get { return rutaFoto; }
            set { rutaFoto = value; }
        }
        public ModeloUsuario(string pUsuario, string pContrasena, string pTelefono, string pNombre,string pCorreo, string pDireccion, string pEdad, DateTime? pFechaNacimiento, string pGenero, string pTipoSangre, string pRutaFoto)
        {
            usuario = pUsuario;
            contrasena = pContrasena;
            telefono = pTelefono;
            nombre = pNombre;
            correo = pCorreo;
            direccion = pDireccion;
            edad = pEdad;
            fechaNacimiento = pFechaNacimiento;
            genero = pGenero;
            tipoSangre = pTipoSangre;
            rutaFoto = pRutaFoto;
        }
            
        
    }
}
