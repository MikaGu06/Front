using System;
using System.Collections.Generic;
using System.IO.Packaging;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Front
{
    /// <summary>
    /// Lógica de interacción para CentrosMedicos.xaml
    /// </summary>
    public partial class CentrosMedicos : Page
    {

        public CentrosMedicos()
        {
            InitializeComponent();
        }

        private void BtnVolver_Click(object sender, RoutedEventArgs e)
        {
            ((MainWindow)Application.Current.MainWindow).MainFrame.Navigate(new Servicios());
        }

        internal class CentroDeSalud {
            private int id_centro;
            private string categoria;
            private string institucion;
            private string direccion;
            private int telefono;
            private Uri link;
            private Uri linkT;
            private int Id_centro
            {
                get { return id_centro; }
                set { id_centro = value; }
            }
            private string Categoria
            {
                get { return categoria; }
                set { categoria = value; }
            }
            private string Institucion
            {
                get { return institucion; }
                set { institucion = value; }
            }
            private string Direccion
            {
                get { return direccion; }
                set { direccion = value; }
            }
            private int Telefono
            {
                get { return telefono;}
                set { telefono = value; }
            }
            private Uri Link
            {
                get { return  link; }
                set { link = value; }
            }
            private Uri LinkT
            {
                get { return LinkT; }
                set {  linkT = value; }
            }
            public CentroDeSalud (int pId_centro, string pCategoria, string pInstitución, string pDireccion, int pTelefono, Uri pLink, Uri pLinkT)
            {
                id_centro= pId_centro;
                categoria= pCategoria;
                institucion = pInstitución;
                direccion= pDireccion;
                telefono = pTelefono;
                link = pLink;
                linkT = pLinkT;
            }
        }

    }
}
