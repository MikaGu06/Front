using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing;
using System.IO.Packaging;
using System.Media;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace Front
{
    /// <summary>
    /// Lógica de interacción para Medicamentos.xaml
    /// </summary>
    public partial class Medicamentos : Page
    {
        // Conexión a la base de datos
        private SqlConnection GetConnection()
        {
            return new SqlConnection(
                ConfigurationManager.ConnectionStrings["cnHealthyU"].ConnectionString
            );
        }

        // Listas en memoria temporales
        private List<Medicamento> medicamentos = new List<Medicamento>();
        private List<Recordatorio> recordatorios = new List<Recordatorio>();

        private DispatcherTimer timer;

        public Medicamentos()
        {
            InitializeComponent();

            // Inicializar timer de recordatorios
            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(10);
            timer.Tick += Timer_Tick;
            timer.Start();

            // Cargar medicamentos y recordatorios desde la base de datos
            CargarMedicamentos();
            CargarRecordatorios();
        }

        #region Timer

        private void Timer_Tick(object sender, EventArgs e)
        {
            RevisarRecordatorios();
        }

        private void RevisarRecordatorios()
        {
            DateTime ahora = DateTime.Now;

            foreach (var rec in recordatorios)
            {
                if (!rec.Estado) continue;

                DateTime primerEvento = rec.Fecha.Date + rec.Hora_inicio.TimeOfDay;

                DateTime next = primerEvento;
                while (next < ahora)
                    next = next.AddHours(rec.Frecuencia);

                if (Math.Abs((next - ahora).TotalSeconds) <= 10 && rec.LastFired < next)
                {
                    rec.LastFired = ahora;

                    // Sonido
                    SystemSounds.Exclamation.Play();

                    // Notificación
                    MostrarNotificacion($"Es hora de tomar tu medicamento: {rec.MedicamentoNombre}");
                }
            }
        }

        #endregion

        #region Notificación Toast

        private async void MostrarNotificacion(string mensaje)
        {
            NotificationText.Text = mensaje;
            NotificationPanel.Visibility = Visibility.Visible;

            var fadeIn = new System.Windows.Media.Animation.DoubleAnimation(0, 1, TimeSpan.FromMilliseconds(200));
            var slideIn = new System.Windows.Media.Animation.DoubleAnimation(-20, 0, TimeSpan.FromMilliseconds(200));

            NotificationPanel.BeginAnimation(OpacityProperty, fadeIn);
            NotifTransform.BeginAnimation(System.Windows.Media.TranslateTransform.YProperty, slideIn);

            await Task.Delay(2500);

            var fadeOut = new System.Windows.Media.Animation.DoubleAnimation(1, 0, TimeSpan.FromMilliseconds(200));
            var slideOut = new System.Windows.Media.Animation.DoubleAnimation(0, -20, TimeSpan.FromMilliseconds(200));

            NotificationPanel.BeginAnimation(OpacityProperty, fadeOut);
            NotifTransform.BeginAnimation(System.Windows.Media.TranslateTransform.YProperty, slideOut);

            await Task.Delay(200);
            NotificationPanel.Visibility = Visibility.Collapsed;
        }

        private void BtnCerrarToast_Click(object sender, RoutedEventArgs e)
        {
            NotificationPanel.Visibility = Visibility.Collapsed;
        }

        #endregion

        #region Botones Navegación

        private void BtnVolver_Click(object sender, RoutedEventArgs e)
        {
            ((MainWindow)Application.Current.MainWindow).MainFrame.Navigate(new Servicios());
        }

        #endregion

        #region Medicamentos

        private void btnAgregarMedicamento_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string nombre = txtNombreMed.Text.Trim();
                string descripcion = txtDescMed.Text.Trim();
                string dosis = txtDosisMed.Text.Trim();
                string unidad = (cmbDosisUnidad.SelectedItem as ComboBoxItem)?.Content.ToString() ?? "";

                if (string.IsNullOrWhiteSpace(nombre))
                    throw new InvalidOperationException("El nombre del medicamento es obligatorio.");
                if (string.IsNullOrWhiteSpace(dosis))
                    throw new InvalidOperationException("La dosis es obligatoria.");
                if (!decimal.TryParse(dosis, out decimal dosisValor))
                    throw new InvalidOperationException("La dosis debe ser un número válido.");
                if (string.IsNullOrWhiteSpace(unidad))
                    throw new InvalidOperationException("Debes seleccionar la unidad de dosis.");

                // Guardar en BD
                using (var con = GetConnection())
                {
                    con.Open();
                    string insertQuery = @"
                     INSERT INTO Medicamento (id_medicamento, nombre, descripcion, dosis, unidad)
                     VALUES (@id, @nombre, @descripcion, @dosis, @unidad);";
                    using (var cmd = new SqlCommand(insertQuery, con))
                    {
                        int nuevoID = ObtenerNuevoIdMedicamento();
                        cmd.Parameters.AddWithValue("@id", nuevoID);
                        cmd.Parameters.AddWithValue("@nombre", nombre);
                        cmd.Parameters.AddWithValue("@descripcion", descripcion);
                        cmd.Parameters.AddWithValue("@dosis", dosisValor);
                        cmd.Parameters.AddWithValue("@unidad", unidad);
                        cmd.ExecuteNonQuery();

                        // Añadir a lista local
                        medicamentos.Add(new Medicamento(nuevoID, nombre, descripcion, dosisValor, unidad));
                    }
                }


                MostrarNotificacion("Medicamento guardado correctamente.");

                // Limpiar campos
                txtNombreMed.Clear();
                txtDescMed.Clear();
                txtDosisMed.Clear();
                cmbDosisUnidad.SelectedIndex = -1;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private int ObtenerNuevoIdMedicamento()
        {
            if (medicamentos.Count == 0) return 1;
            return medicamentos[medicamentos.Count - 1].Id_medicamento + 1;
        }

        private void CargarMedicamentos()
        {
            try
            {
                using (var con = GetConnection())
                {
                    con.Open();
                    string query = "SELECT id_medicamento, nombre, descripcion, dosis, unidad FROM Medicamento";
                    using (var cmd = new SqlCommand(query, con))
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            medicamentos.Add(new Medicamento(
                                reader.GetInt32(0),
                                reader.GetString(1),
                                reader.IsDBNull(2) ? "" : reader.GetString(2),
                                reader.GetDecimal(3),
                                reader.GetString(4)
                            ));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error cargando medicamentos: " + ex.Message);
            }
        }


        #endregion

        #region Recordatorios

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (dpRecDia.SelectedDate == null)
                    throw new InvalidOperationException("Selecciona un día de inicio.");
                if (!TimeSpan.TryParseExact(txtRecHoraInicio.Text.Trim(), "hh\\:mm", null, out TimeSpan hora))
                    throw new InvalidOperationException("La hora debe estar en formato HH:mm.");
                if (string.IsNullOrWhiteSpace(txtRecMedicamento.Text))
                    throw new InvalidOperationException("Debes ingresar el nombre del medicamento.");
                if (cmbRecFrecuencia.SelectedItem == null)
                    throw new InvalidOperationException("Selecciona la frecuencia.");

                int frecuencia = int.Parse(((ComboBoxItem)cmbRecFrecuencia.SelectedItem).Tag.ToString());

                // Buscar id del medicamento en BD
                int idMed = ObtenerIdMedicamentoPorNombre(txtRecMedicamento.Text.Trim());
                if (idMed == 0)
                    throw new InvalidOperationException("El medicamento no existe en la base de datos.");

                DateTime fecha = dpRecDia.SelectedDate.Value;
                DateTime horaInicio = DateTime.Today + hora;

                using (var con = GetConnection())
                {
                    con.Open();
                    string insertQuery = @"
                        INSERT INTO Recordatorio (id_recordatorio, id_medicamento, fecha, hora_inicio, frecuencia, estado)
                        VALUES (@id, @idMed, @fecha, @hora, @frecuencia, 1)";
                    using (var cmd = new SqlCommand(insertQuery, con))
                    {
                        int nuevoID = ObtenerNuevoIdRecordatorio();
                        cmd.Parameters.AddWithValue("@id", nuevoID);
                        cmd.Parameters.AddWithValue("@idMed", idMed);
                        cmd.Parameters.AddWithValue("@fecha", fecha);
                        cmd.Parameters.AddWithValue("@hora", horaInicio.TimeOfDay);
                        cmd.Parameters.AddWithValue("@frecuencia", frecuencia);
                        cmd.ExecuteNonQuery();

                        recordatorios.Add(new Recordatorio(nuevoID, fecha, horaInicio, frecuencia, true, txtRecMedicamento.Text.Trim()));
                    }
                }

                MostrarNotificacion("Recordatorio creado correctamente.");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private int ObtenerIdMedicamentoPorNombre(string nombre)
        {
            foreach (var med in medicamentos)
            {
                if (med.Nombre.Equals(nombre, StringComparison.OrdinalIgnoreCase))
                    return med.Id_medicamento;
            }
            return 0;
        }

        private int ObtenerNuevoIdRecordatorio()
        {
            if (recordatorios.Count == 0) return 1;
            return recordatorios[recordatorios.Count - 1].Id_recordatorio + 1;
        }

        private void CargarRecordatorios()
        {
            try
            {
                using (var con = GetConnection())
                {
                    con.Open();
                    string query = @"
                        SELECT r.id_recordatorio, r.id_medicamento, r.fecha, r.hora_inicio, r.frecuencia, r.estado, m.nombre
                        FROM Recordatorio r
                        JOIN Medicamento m ON r.id_medicamento = m.id_medicamento";
                    using (var cmd = new SqlCommand(query, con))
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            recordatorios.Add(new Recordatorio(
                                reader.GetInt32(0),
                                reader.GetDateTime(2),
                                reader.GetDateTime(3),
                                reader.GetInt32(4),
                                reader.GetBoolean(5),
                                reader.GetString(6)
                            ));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error cargando recordatorios: " + ex.Message);
            }
        }

        #endregion

        #region Clases internas

        internal class Medicamento
        {
            private int id_medicamento;
            private string nombre;
            private string descripcion;
            private decimal dosis;
            private string unidad;

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
            public decimal Dosis
            {
                get { return dosis; }
                set { dosis = value; }
            }
            public string Unidad
            {
                get { return unidad; }
                set { unidad = value; }
            }
            



            public Medicamento(int pId_medicamento, string pNombre, string pDescripcion, decimal pDosis, string pUnidad)
            {
                id_medicamento = pId_medicamento;
                nombre = pNombre;
                descripcion = pDescripcion;
                dosis = pDosis;
                unidad = pUnidad;
            }
            public override string ToString()
            {
                return $"{dosis} {unidad}";
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
        #endregion
    }
}
