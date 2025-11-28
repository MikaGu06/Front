using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using Front.Helpers;   

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

        private void CargarHistorial()
        {
            try
            {
                // Si no hay CI en sesión, no cargamos nada
                if (string.IsNullOrWhiteSpace(SesionUsuario.CI))
                    return;

                if (!int.TryParse(SesionUsuario.CI, out int ciPaciente))
                    return;

                var listaSignos = servicio.ListarSignosDePaciente(ciPaciente);

                var listaMostrar = new List<object>();
                foreach (var s in listaSignos)
                {
                    listaMostrar.Add(new
                    {
                        Fecha = s.Fecha.ToString("dd/MM/yyyy HH:mm"),
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
                MessageBox.Show($"Error al cargar historial: {ex.Message}",
                                "Error",
                                MessageBoxButton.OK,
                                MessageBoxImage.Error);
            }
        }

        private void Acceder_Click(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            try
            {
                // 1) Validar datos
                var valores = ValidacionesSignosVitales.Validar(
                    txtRitmoCardiaco.Text,
                    txtPresionArterial.Text,
                    txtOxigenacion.Text,
                    txtTemperatura.Text
                );

                // 2) Verificar CI en sesión
                if (string.IsNullOrWhiteSpace(SesionUsuario.CI))
                {
                    MessageBox.Show(
                        "Para registrar signos vitales primero debes completar tus datos en 'Mi Cuenta' (CI obligatorio).",
                        "Paciente no configurado",
                        MessageBoxButton.OK,
                        MessageBoxImage.Warning);
                    return;
                }

                if (!int.TryParse(SesionUsuario.CI, out int ciPaciente))
                {
                    MessageBox.Show(
                        "El CI guardado en la sesión no es válido. Vuelve a 'Mi Cuenta' y corrige el CI.",
                        "CI inválido",
                        MessageBoxButton.OK,
                        MessageBoxImage.Warning);
                    return;
                }

                // 3) Nuevo id_signo
                int nuevoId = servicio.ObtenerNuevoId();

                int sistolica = int.Parse(txtPresionArterial.Text.Split('-')[0]);

                // 4) Crear modelo con CI real del paciente
                var nuevoSigno = new ModeloSignosVitales
                {
                    IdSigno = nuevoId,
                    CiPaciente = ciPaciente,
                    Fecha = DateTime.Now,
                    Hora = DateTime.Now.TimeOfDay,
                    RitmoCardiaco = valores.ritmo,
                    PresionArterial = sistolica,
                    Temperatura = valores.temperatura,
                    Oxigenacion = valores.oxigeno
                };

                // 5) Guardar en BD
                servicio.AgregarSigno(nuevoSigno);

                // Si usas tabla pac_signos, descomenta:
                // servicio.AgregarPacSigno(ciPaciente, nuevoId);

                MessageBox.Show("Signos vitales registrados correctamente.",
                                "Éxito",
                                MessageBoxButton.OK,
                                MessageBoxImage.Information);

                // 6) Limpiar campos
                txtRitmoCardiaco.Clear();
                txtPresionArterial.Clear();
                txtOxigenacion.Clear();
                txtTemperatura.Clear();

                // 7) Recargar historial del paciente
                CargarHistorial();
            }
            catch (ArgumentException ex)
            {
                MessageBox.Show(ex.Message,
                                "Datos inválidos",
                                MessageBoxButton.OK,
                                MessageBoxImage.Warning);
            }
            catch (InvalidOperationException ex)
            {
                MessageBox.Show(ex.Message,
                                "Paciente no válido",
                                MessageBoxButton.OK,
                                MessageBoxImage.Warning);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al agregar signo: {ex.Message}",
                                "Error",
                                MessageBoxButton.OK,
                                MessageBoxImage.Error);
            }
        }

        private void BtnVolver_Click(object sender, RoutedEventArgs e)
        {
            if (NavigationService != null && NavigationService.CanGoBack)
                NavigationService.GoBack();
        }
    }
}
