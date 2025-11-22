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

namespace Front
{
    /// <summary>
    /// Lógica de interacción para Medicamentos.xaml
    /// </summary>
    public partial class Medicamentos : Page
    {
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


        }

        internal class Recordatorio
        {
            private int id_recordatorio;
            private string fecha;
            private int frecuencia;
        }




        public Medicamentos()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
