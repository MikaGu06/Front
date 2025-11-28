using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace Front.SigVitalesPag
{
    public partial class SignosVitales : Page
    {
        private SignosVitalesServicio servicio;

        public SignosVitales()
        {
            InitializeComponent();
            servicio = new SignosVitalesServicio();

            CargarHistorial();
        }

        private void CargarHistorial()
        {
            try
            {
                var listaSignos = servicio.ListarSignos();

                // Mapear para ItemsControl
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
                MessageBox.Show($"Error al cargar historial: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Acceder_Click(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            try
            {
                // Validar datos
                var valores = ValidacionesSignosVitales.Validar(
                    txtRitmoCardiaco.Text,
                    txtPresionArterial.Text,
                    txtOxigenacion.Text,
                    txtTemperatura.Text
                );

                // Crear modelo de signo vital
                var nuevoSigno = new ModeloSignosVitales(
                    servicio.ObtenerNuevoId(),
                    0, // ci_paciente temporalmente 0 o NULL si la tabla lo permite
                    DateTime.Now.Date,
                    DateTime.Now.TimeOfDay,
                    valores.ritmo,
                    int.Parse(txtPresionArterial.Text.Split('-')[0]),
                    valores.temperatura,
                    valores.oxigeno
                );

                // Guardar signo en DB
                servicio.AgregarSigno(nuevoSigno);

                // Aquí se registra en la tabla intermedia, opcionalmente con un paciente temporal
                int pacienteIdTemporal = 0; // o usar un paciente real si existe
                servicio.AgregarPacSigno(pacienteIdTemporal, nuevoSigno.IdSigno);

                MessageBox.Show("Signo vital agregado correctamente.", "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);

                // Limpiar campos
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
