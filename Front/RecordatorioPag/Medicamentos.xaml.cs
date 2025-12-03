using Front.Helpers;
using Front.RecordatorioPag.ModelosR;
using Front.RecordatorioPag.ServicioR;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Media;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Threading;

namespace Front
{
    public partial class Medicamentos : Page
    {
        private readonly MedicamentoServicio medServicio;
        private readonly RecordatorioServicio recServicio;
        private List<Recordatorio> recordatorios;
        private DispatcherTimer timer;

        public ObservableCollection<Recordatorio> ListaRecordatorios { get; set; } = new ObservableCollection<Recordatorio>();

        private int ciPaciente;

        public Medicamentos()
        {
            InitializeComponent();

            medServicio = new MedicamentoServicio();
            recServicio = new RecordatorioServicio();
            recordatorios = new List<Recordatorio>();

            RecordatoriosList.ItemsSource = ListaRecordatorios;

            timer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(10) };
            timer.Tick += Timer_Tick;
            timer.Start();

            // Obtener CI del paciente desde sesión
            if (!int.TryParse(SesionUsuario.CI, out ciPaciente))
            {
                MessageBox.Show("Debes registrar tu CI en MI CUENTA para usar los recordatorios.", "CI inválido", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            medServicio.CargarMedicamentos();
            CargarRecordatorios();
            CargarRecordatoriosEnLista();
        }

        #region Timer
        private void Timer_Tick(object sender, EventArgs e) => RevisarRecordatorios();

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
                    SystemSounds.Exclamation.Play();
                    MostrarNotificacion($"Es hora de tomar tu medicamento: {rec.MedicamentoNombre}");
                }
            }
        }
        #endregion

        #region Notificación
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
            if (NavigationService.CanGoBack)
                NavigationService.GoBack();
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

                int nuevoID = medServicio.ObtenerNuevoId();
                var med = new Medicamento(nuevoID, nombre, descripcion, dosisValor, unidad);
                medServicio.AgregarMedicamento(med);

                MostrarNotificacion("Medicamento guardado correctamente.");

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

                int idMed = medServicio.ObtenerIdPorNombre(txtRecMedicamento.Text.Trim());
                if (idMed == 0)
                    throw new InvalidOperationException("El medicamento no existe en la base de datos.");

                DateTime fecha = dpRecDia.SelectedDate.Value;
                DateTime horaInicio = DateTime.Today + hora;

                int nuevoID = recServicio.ObtenerNuevoId();
                var nuevoRec = new Recordatorio(nuevoID, fecha, horaInicio, frecuencia, true, txtRecMedicamento.Text.Trim(), ciPaciente);

                recServicio.AgregarRecordatorioConPaciente(nuevoRec, idMed, ciPaciente);

                recordatorios.Add(nuevoRec);
                ListaRecordatorios.Add(nuevoRec);

                MostrarNotificacion("Recordatorio creado correctamente.");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al agregar recordatorio: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void CargarRecordatorios()
        {
            try
            {
                recordatorios = recServicio.ListarRecordatoriosPorPaciente(ciPaciente);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error cargando recordatorios: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void CargarRecordatoriosEnLista()
        {
            ListaRecordatorios.Clear();
            foreach (var rec in recordatorios)
                ListaRecordatorios.Add(rec);
        }

        private void ToggleButton_Checked(object sender, RoutedEventArgs e)
        {
            if (sender is ToggleButton toggle && toggle.DataContext is Recordatorio rec)
            {
                rec.Estado = true;
                recServicio.ActualizarEstadoEnBD(rec);
                RecordatoriosList.Items.Refresh();
            }
        }

        private void ToggleButton_Unchecked(object sender, RoutedEventArgs e)
        {
            if (sender is ToggleButton toggle && toggle.DataContext is Recordatorio rec)
            {
                rec.Estado = false;
                recServicio.ActualizarEstadoEnBD(rec);
                RecordatoriosList.Items.Refresh();
            }
        }
        #endregion
    }
}
