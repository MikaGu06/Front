using Front.CentroMedPag.ModelosCM;
using Front.CentroMedPag.ServiciosCM;
using Front.Helpers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace Front
{
    public partial class CentrosMedicos : Page
    {
        private readonly CentroDeSaludServicio centroServicio; // Usa solo este servicio
        private ObservableCollection<CentrosDeSalud> CentrosUI { get; set; } = new ObservableCollection<CentrosDeSalud>();
        private List<CategoriaCentro> categorias;
        private int ciUsuario;

        public CentrosMedicos()
        {
            InitializeComponent();

            centroServicio = new CentroDeSaludServicio();

            // Validar CI del usuario
            if (string.IsNullOrWhiteSpace(SesionUsuario.CI) || !int.TryParse(SesionUsuario.CI, out ciUsuario))
            {
                MessageBox.Show(
                    "Debes iniciar sesión con un CI válido para usar esta sección.",
                    "CI inválido",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);
                NavigationService?.GoBack();
                return;
            }

            FilasContainer.ItemsSource = CentrosUI;

            CargarCategorias();
        }

        private void CargarCategorias()
        {
            
            ListaCategorias.ItemsSource = new List<string>
            {
                "AMBULANCIA",
                "CENTRO DE DIAGNOSTICO",
                "CENTRO DE FISIOTERAPIA ",
                
                "CENTRO MEDICO",
                "CLINICA / HOSPITAL",
                "CLINICA ODONTOLOGICA",
                "FARMACIA",
                "LABORATORIO CLINICO",
                "OPTICA DIAGNOSTICO"
            };

            ListaCategorias.SelectionChanged += ListaCategorias_SelectionChanged;
        }

        
        private void ListaCategorias_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ListaCategorias.SelectedItem == null) return;

            int idCat = ListaCategorias.SelectedIndex + 1;

            try
            {
                var centros = centroServicio.ObtenerCentrosPorCategoria(idCat, ciUsuario);

                CentrosUI.Clear();

                foreach (var centro in centros)
                {
                    CentrosUI.Add(centro);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error cargando centros: " + ex.Message);
            }
        }

        private void AbrirMapa_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.Tag is string url && !string.IsNullOrEmpty(url))
            {
                try
                {
                    System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                    {
                        FileName = url,
                        UseShellExecute = true
                    });
                }
                catch (Exception ex)
                {
                    MessageBox.Show("No se pudo abrir el enlace: " + ex.Message);
                }
            }
        }

        private void AbrirWhatsApp_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.Tag is string url && !string.IsNullOrEmpty(url))
            {
                try
                {
                    System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                    {
                        FileName = url,
                        UseShellExecute = true
                    });
                }
                catch (Exception ex)
                {
                    MessageBox.Show("No se pudo abrir el enlace: " + ex.Message);
                }
            }
        }

        private void BtnVolver_Click(object sender, RoutedEventArgs e)
        {
            if (NavigationService != null && NavigationService.CanGoBack)
                NavigationService.GoBack();
        }

        private void CheckBoxFavorito_Checked(object sender, RoutedEventArgs e)
        {
            if (sender is CheckBox cb && cb.Tag is int idCentro)
            {
                try
                {
                    centroServicio.AgregarFavorito(ciUsuario, idCentro);
                    MessageBox.Show("Centro añadido a favoritos.", "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    cb.IsChecked = false;
                    MessageBox.Show($"No se pudo agregar favorito: {ex.Message}", "Error de BD", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void CheckBoxFavorito_Unchecked(object sender, RoutedEventArgs e)
        {
            if (sender is CheckBox cb && cb.Tag is int idCentro)
            {
                try
                {
                    centroServicio.QuitarFavorito(ciUsuario, idCentro);
                    MessageBox.Show("Centro eliminado de favoritos.", "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    cb.IsChecked = true;
                    MessageBox.Show($"No se pudo quitar favorito: {ex.Message}", "Error de BD", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
    }
}
