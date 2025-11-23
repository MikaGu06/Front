using System;
using System.Collections.Generic;
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
using Microsoft.Data.SqlClient;
using System.Configuration;





namespace Front
{
    /// <summary>
    /// Lógica de interacción para Medicamentos.xaml
    /// </summary>

   
    public partial class Medicamentos : Page
    {
        private SqlConnection GetConnection()
        {
            return new SqlConnection(
                ConfigurationManager.ConnectionStrings["MiConexion"].ConnectionString
            );
        }
        internal class Medicamento
        {
            private int id_medicamento;
            private string nombre;
            private string descripcion;
            private string dosis;
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
            public string Dosis
            {
                get { return dosis; }
                set { dosis = value; }
            }
            public Medicamento(int pId_medicamento, string pNombre, string pDescripcion,
                string pDosis)
            {
                id_medicamento= pId_medicamento;
                nombre= pNombre;
                descripcion = pDescripcion;
                dosis= pDosis;
            }


        }

        internal class Recordatorio
        {
            private int id_recordatorio;
            private DateTime fecha;
            private DateTime hora_inicio;
            private int frecuencia;
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
                set{ hora_inicio = value; }
            }
            public int Frecuencia
            {
                get { return frecuencia; }
                set {  frecuencia = value; }
            }
            public Recordatorio(int pId, DateTime pFecha, DateTime pHoraInicio, int pFrecuencia)
            {
                id_recordatorio = pId;
                fecha = pFecha;
                hora_inicio = pHoraInicio;
                frecuencia = pFrecuencia;
            }
        }



        SqlConnection cn;
        public Medicamentos()
        {
            InitializeComponent();
            
        }

        private void btnAgregarMedicamento_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

        }

        private void BtnVolver_Click(object sender, RoutedEventArgs e)
        {
            ((MainWindow)Application.Current.MainWindow).MainFrame.Navigate(new Servicios());
        }
    }
}
