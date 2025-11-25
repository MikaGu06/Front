using Front.RecordatorioPag.ModelosR;
using Front.RecordatorioPag.ServicioR;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Configuration;
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
        private MedicamentoServicio medServicio = new MedicamentoServicio();
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

            // Cargar datos
            medServicio.CargarMedicamentos();
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

                // Crear medicamento y guardar con servicio
                int nuevoID = medServicio.ObtenerNuevoId();
                var med = new Medicamento(nuevoID, nombre, descripcion, dosisValor, unidad);
                medServicio.AgregarMedicamento(med);

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

                // Buscar id del medicamento con el servicio
                int idMed = medServicio.ObtenerIdPorNombre(txtRecMedicamento.Text.Trim());
                if (idMed == 0)
                    throw new InvalidOperationException("El medicamento no existe en la base de datos.");

                DateTime fecha = dpRecDia.SelectedDate.Value;
                DateTime horaInicio = DateTime.Today + hora;

                using (var con = new SqlConnection(ConfigurationManager.ConnectionStrings["cnHealthyU"].ConnectionString))
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

        private int ObtenerNuevoIdRecordatorio()
        {
            int nuevoID = 1;
            try
            {
                using (var con = new SqlConnection(ConfigurationManager.ConnectionStrings["cnHealthyU"].ConnectionString))
                {
                    con.Open();
                    string query = "SELECT ISNULL(MAX(id_recordatorio), 0) FROM Recordatorio";
                    using (var cmd = new SqlCommand(query, con))
                    {
                        nuevoID = (int)cmd.ExecuteScalar() + 1;
                    }
                }
            }
            catch
            {
                // Si hay un error, se deja el ID como 1
            }
            return nuevoID;
        }

        private void CargarRecordatorios()
        {
            recordatorios.Clear();
            try
            {
                using (var con = new SqlConnection(ConfigurationManager.ConnectionStrings["cnHealthyU"].ConnectionString))
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
                                DateTime.Today + reader.GetTimeSpan(3),
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
    }
}
