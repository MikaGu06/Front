using Front.Helpers;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace Front.SigVitalesPag
{
    public partial class SignosVitales : Page
    {
        private readonly SignosVitalesServicio servicio;

        public SignosVitales()
        {
            InitializeComponent();
            servicio = new SignosVitalesServicio();

            CargarHistorial();
        }

        //      CARGAR HISTORIAL
        private void CargarHistorial()
        {
            try
            {
                // CI viene como string desde SesionUsuario
                if (string.IsNullOrWhiteSpace(SesionUsuario.CI))
                {
                    MessageBox.Show(
                        "Debes completar tus datos en MI CUENTA antes de usar Signos Vitales.",
                        "Datos incompletos",
                        MessageBoxButton.OK,
                        MessageBoxImage.Warning);
                    return;
                }

                if (!int.TryParse(SesionUsuario.CI, out int ciPaciente))
                {
                    MessageBox.Show(
                        "El CI guardado en sesión no es válido. Vuelve a guardarlo en MI CUENTA.",
                        "CI inválido",
                        MessageBoxButton.OK,
                        MessageBoxImage.Warning);
                    return;
                }

                var listaSignos = servicio.ListarSignosDePaciente(ciPaciente);

                var listaMostrar = new List<dynamic>();
                foreach (var s in listaSignos)
                {
                    listaMostrar.Add(new
                    {
                        Fecha = s.Fecha.ToString("dd/MM/yyyy"),
                        Ritmo = s.RitmoCardiaco,
                        Presion = s.PresionArterial,
                        Oxigenacion = s.Oxigenacion,
                        Temperatura = s.Temperatura
                    });
                }

                HistorialItems.ItemsSource = listaMostrar;
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Error al cargar historial: {ex.Message}",
                    "Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        //      GUARDAR SIGNO
        private void Acceder_Click(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(SesionUsuario.CI) || !int.TryParse(SesionUsuario.CI, out int ciPaciente))
                {
                    MessageBox.Show("Debes registrar un CI válido en MI CUENTA.", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                var valores = ValidacionesSignosVitales.Validar(
                    txtRitmoCardiaco.Text,
                    txtPresionArterial.Text,
                    txtOxigenacion.Text,
                    txtTemperatura.Text
                );

                int nuevoId = servicio.ObtenerNuevoId();
                var nuevoSigno = new ModeloSignosVitales(
                    nuevoId,
                    ciPaciente,
                    DateTime.Now.Date,
                    DateTime.Now.TimeOfDay,
                    valores.ritmo,
                    valores.sistolica,
                    valores.temperatura,
                    valores.oxigeno
                );

                // Guardar en ambas tablas
                servicio.AgregarSignoConPaciente(nuevoSigno);

                MessageBox.Show("Signo vital agregado correctamente.", "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);

                txtRitmoCardiaco.Clear();
                txtPresionArterial.Clear();
                txtOxigenacion.Clear();
                txtTemperatura.Clear();

                CargarHistorial();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al agregar signo: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }



        private void BtnVolver_Click(object sender, RoutedEventArgs e)
        {
            if (NavigationService.CanGoBack)
                NavigationService.GoBack();
        }
    }
}
