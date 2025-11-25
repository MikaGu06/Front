using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Media;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using static Front.Medicamentos;

namespace Front
{
    /// <summary>
    /// Lógica de interacción para Medicamentos.xaml
    /// </summary>
    public partial class Medicamentos : Page
    {
        //MOSTRAR NOTIFICACIÓN PERSONALIZADA
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

        // Conexión a la base de datos
        private SqlConnection GetConnection()
        {
            return new SqlConnection(
                ConfigurationManager.ConnectionStrings["MiConexion"].ConnectionString
            );
        }

        //TEMPORAL: lista de recordatorios en memoria
        private List<Recordatorio> recordatorios = new List<Recordatorio>();

        //TEMPORAL: lista de medicamentos en memoria
        private List<Medicamento> medicamentos = new List<Medicamento>();

        // Timer que revisa cada 10 segundos los recordatorios
        private DispatcherTimer timer;

        public Medicamentos()
        {
            InitializeComponent();

            // Inicializar timer
            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(10);
            timer.Tick += Timer_Tick;
            timer.Start();


        }

        // Evento del timer
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

                // ACTIVAR RECORDATORIO
                if (Math.Abs((next - ahora).TotalSeconds) <= 10 && rec.LastFired < next)
                {
                    rec.LastFired = ahora;

                    // SONIDO DEL SISTEMA
                    SystemSounds.Exclamation.Play();

                    // NOTIFICACIÓN
                    MostrarNotificacion($"Es hora de tomar tu medicamento: {rec.MedicamentoNombre}");
                }
            }
        }

        // CLASES INTERNAS
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
           


            public Medicamento(int pId_medicamento, string pNombre, string pDescripcion, string pDosis)
            {
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
                set { hora_inicio = value; }
            }
            public int Frecuencia
            {
                get { return frecuencia; }
                set { frecuencia = value; }
            }
            public bool Estado
            {
                get { return estado; }
                set { estado = value; }
            }

            public string MedicamentoNombre { get; set; } = "";

            // Evita que suene varias veces
            public DateTime LastFired { get; set; } = DateTime.MinValue;

            public Recordatorio(int pId_recordatorio, DateTime pFecha, DateTime pHoraInicio, int pFrecuencia, bool pEstado, string pMedicamentoNombre)
            {
                id_recordatorio = pId_recordatorio;
                fecha = pFecha;
                hora_inicio = pHoraInicio;
                frecuencia = pFrecuencia;
                estado = pEstado;
                MedicamentoNombre = pMedicamentoNombre;
            }
        }

        // BOTÓN AGREGAR MEDICAMENTO
        private void btnAgregarMedicamento_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string nombre = txtNombreMed.Text.Trim();
                string descripcion = txtDescMed.Text.Trim();
                string dosis = txtDosisMed.Text.Trim();
                string unidad = (cmbDosisUnidad.SelectedItem as ComboBoxItem)?.Content.ToString() ?? "";

                // VALIDACIONES
                if (string.IsNullOrWhiteSpace(nombre))
                    throw new InvalidOperationException("El nombre del medicamento es obligatorio.");

                if (string.IsNullOrWhiteSpace(dosis))
                    throw new InvalidOperationException("La dosis es obligatoria.");

                if (!decimal.TryParse(dosis, out decimal dosisValor))
                    throw new InvalidOperationException("La dosis debe ser un número válido.");

                if (string.IsNullOrWhiteSpace(unidad))
                    throw new InvalidOperationException("Debes seleccionar la unidad de dosis.");

                // esto temporal
                var nuevoMed = new Medicamento(
                    medicamentos.Count + 1, // id provisional
                    nombre,
                    descripcion,
                    $"{dosis} {unidad}"
                );

                medicamentos.Add(nuevoMed);

                MostrarNotificacion("Medicamento guardado correctamente (temporal).");

                // Limpiar campos para agregar otro medicamento
                txtNombreMed.Clear();
                txtDescMed.Clear();
                txtDosisMed.Clear();
                cmbDosisUnidad.SelectedIndex = -1;
            }
            catch (InvalidOperationException ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Validar día
                if (dpRecDia.SelectedDate == null)
                    throw new InvalidOperationException("Selecciona un día de inicio.");

                // Validar hora
                if (!TimeSpan.TryParseExact(txtRecHoraInicio.Text.Trim(), "hh\\:mm", null, out TimeSpan hora))
                    throw new InvalidOperationException("La hora debe estar en formato HH:mm.");

                // Validar medicamento
                if (string.IsNullOrWhiteSpace(txtRecMedicamento.Text))
                    throw new InvalidOperationException("Debes ingresar el nombre del medicamento.");

                // Validar frecuencia
                if (cmbRecFrecuencia.SelectedItem == null)
                    throw new InvalidOperationException("Selecciona la frecuencia.");

                int frecuencia = int.Parse(((ComboBoxItem)cmbRecFrecuencia.SelectedItem).Tag.ToString());

                // esto igual temporal
                var nuevoRec = new Recordatorio(
                    recordatorios.Count + 1,
                    dpRecDia.SelectedDate.Value,
                    DateTime.Today + hora,
                    frecuencia,
                    true,
                    txtRecMedicamento.Text.Trim()
                );

                recordatorios.Add(nuevoRec);

                MostrarNotificacion("Recordatorio creado correctamente.");
            }
            catch (InvalidOperationException ex)
            {
                MessageBox.Show(ex.Message);
            }
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
