using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using System.Media;

namespace Front
{
    /// <summary>
    /// Lógica de interacción para Medicamentos.xaml
    /// </summary>
    public partial class Medicamentos : Page
    {
        private async void MostrarNotificacion(string mensaje)
        {
            NotificationText.Text = mensaje;

            NotificationPanel.Visibility = Visibility.Visible;

            // Animar entrada
            var fadeIn = new System.Windows.Media.Animation.DoubleAnimation(0, 1, TimeSpan.FromMilliseconds(200));
            var slideIn = new System.Windows.Media.Animation.DoubleAnimation(-20, 0, TimeSpan.FromMilliseconds(200));

            NotificationPanel.BeginAnimation(OpacityProperty, fadeIn);
            NotifTransform.BeginAnimation(System.Windows.Media.TranslateTransform.YProperty, slideIn);

            // Esperar 2.5 segundos
            await Task.Delay(2500);

            // Animar salida
            var fadeOut = new System.Windows.Media.Animation.DoubleAnimation(1, 0, TimeSpan.FromMilliseconds(200));
            var slideOut = new System.Windows.Media.Animation.DoubleAnimation(0, -20, TimeSpan.FromMilliseconds(200));

            NotificationPanel.BeginAnimation(OpacityProperty, fadeOut);
            NotifTransform.BeginAnimation(System.Windows.Media.TranslateTransform.YProperty, slideOut);

            await Task.Delay(200);
            NotificationPanel.Visibility = Visibility.Collapsed;
        }

        // Esto conecta a la base de datos pa cuando la pongamos, ahorita esta listo nomas y ya tiene instalado los using para sql
        private SqlConnection GetConnection()
        {
            return new SqlConnection(
                ConfigurationManager.ConnectionStrings["MiConexion"].ConnectionString
            );
        }

        //TEMPOORAL esto solo esta para probarlo, luego aqui tiene que ir la lista que se carga de SQL
        private List<Recordatorio> recordatorios = new List<Recordatorio>();

        // este timer revisa cada 10 seg la hora pa poner los recordatorios en base a la hora que se puso y asi 
        private DispatcherTimer timer;

        public Medicamentos()
        {
            InitializeComponent();

            // esto es lo del timer osea se lo inicializa y esta lo de los 10 seg
            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(10);
            timer.Tick += Timer_Tick;
            timer.Start();
        }

        // aqui indica que es lo que revisa el timer osea los recordatorios 
        private void Timer_Tick(object sender, EventArgs e)
        {
            RevisarRecordatorios();
        }


        private void RevisarRecordatorios()
        {
            DateTime ahora = DateTime.Now;

            foreach (var rec in recordatorios)
            {
                if (!rec.Estado) continue; // Si está apagado, no revisa

                DateTime primerEvento = rec.Fecha.Date + rec.Hora_inicio.TimeOfDay;

                DateTime next = primerEvento;
                while (next < ahora)
                    next = next.AddHours(rec.Frecuencia);

                // AQUÍ ES DONDE SE ACTIVA EL RECORDATORIO
                if (Math.Abs((next - ahora).TotalSeconds) <= 10 && rec.LastFired < next)
                {
                    rec.LastFired = ahora;

                    
                    // SONIDO DEL SISTEMA
                    SystemSounds.Exclamation.Play();

                    // NOTIFICACIÓN TOAST
                    MostrarNotificacion($"Es hora de tomar tu medicamento");

                }
            }
        }


        internal class Medicamento
        {
            private int id_medicamento; 
            private string nombre; 
            private string descripcion; 
            private string dosis; 
            public int Id_medicamento { 
                get { return id_medicamento; } 
                set { id_medicamento = value; } 
            }
            public string Nombre {
                get { return nombre; } 
                set { nombre = value; } 
            }
            public string Descripcion {
                get { return descripcion; } 
                set { descripcion = value; } 
            }
            public string Dosis {
                get { return dosis; } 
                set { dosis = value; } 
            }
            public Medicamento(int pId_medicamento, string pNombre, string pDescripcion, string pDosis) {
                id_medicamento = pId_medicamento; 
                nombre = pNombre; 
                descripcion = pDescripcion; 
                dosis = pDosis; 
            }
        }

      
        internal class Recordatorio
        {
            private int id_recordatorio; 
            private DateTime fecha; 
            private DateTime hora_inicio; 
            private int frecuencia; 
            private bool estado; 
            public int Id_recordatorio { 
                get { return id_recordatorio; } 
                set { id_recordatorio = value; } 
            }
            public DateTime Fecha { 
                get { return fecha; } 
                set { fecha = value; } 
            }
            public DateTime Hora_inicio { 
                get { return hora_inicio; } 
                set { hora_inicio = value; } 
            }
            public int Frecuencia { 
                get { return frecuencia; } 
                set { frecuencia = value; } 
            }
            public bool Estado { 
                get { return estado; } 
                set { estado = value; } 
            }

            // esto es pa evitar que un recordatorio suene varias veces
            public DateTime LastFired { get; set; } = DateTime.MinValue;

            public Recordatorio(int pId_recordatorio, DateTime pFecha, DateTime pHoraInicio, int pFrecuencia, bool pEstado)
            {
                id_recordatorio = pId_recordatorio;
                fecha = pFecha;
                hora_inicio = pHoraInicio;
                frecuencia = pFrecuencia;
                estado = pEstado;
            }
        }



        private void btnAgregarMedicamento_Click(object sender, RoutedEventArgs e)
        {
            //ACAAAA tengo que poner que los mediicamentos se suban a la base de datos
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            // validar que todos los campos se rellenen 
            if (dpRecDia.SelectedDate == null)
            {
                MessageBox.Show("Selecciona un día.");
                return;
            }

            if (!TimeSpan.TryParse(txtRecHoraInicio.Text, out TimeSpan hora))
            {
                MessageBox.Show("Hora inválida. Usa formato HH:mm.");
                return;
            }

            if (cmbRecFrecuencia.SelectedItem == null)
            {
                MessageBox.Show("Selecciona la frecuencia.");
                return;
            }
            //eata aca es validacion de relleno  

            // me da la frecuencia sacandola del combobox (el de las horas y eso)
            int frecuencia = int.Parse(((ComboBoxItem)cmbRecFrecuencia.SelectedItem).Tag.ToString());

            
            // aca lo agrega a la lista pero con la base de datos aca tambien lo debe añadir a la tabla de recordatorioos
            var nuevoRec = new Recordatorio(
                recordatorios.Count + 1, // finge ser id pero esto se tiene que sacar de la base de datos 
                dpRecDia.SelectedDate.Value,
                DateTime.Today + hora,
                frecuencia,
                true // Estado activo
            );

            //TEMPORAL esto nomas es pa la lista temporal luego tengo que poner algo asi : GuardarRecordatorioEnBD(nuevoRec);
            //recordatorios = ObtenerRecordatoriosDesdeBD();
            recordatorios.Add(nuevoRec);

            MostrarNotificacion("Recordatorio creado correctamente.");
        }

        
        private void BtnVolver_Click(object sender, RoutedEventArgs e)
        {
            ((MainWindow)Application.Current.MainWindow).MainFrame.Navigate(new Servicios());
        }
        private void BtnCerrarToast_Click(object sender, RoutedEventArgs e)
        {
            NotificationPanel.Visibility = Visibility.Collapsed;
        }

    }
}
