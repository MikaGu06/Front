using Front.CentroMedPag.ModelosCM;
using Front.CentroMedPag.ServiciosCM;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;

namespace Front
{
    public partial class CentrosMedicos : Page
    {
        private CentroDeSaludServicio servicio = new CentroDeSaludServicio();

        // ObservableCollection para ItemsControl
        public ObservableCollection<dynamic> CentrosUI { get; set; } = new ObservableCollection<dynamic>();

        public CentrosMedicos()
        {
            InitializeComponent();

            // Asociar la colección al ItemsControl
            FilasContainer.ItemsSource = CentrosUI;

            CargarCategorias();
        }

        private void CargarCategorias()
        {
            ListaCategorias.ItemsSource = new List<string>
            {
                "AMBULANCIA",
                "CENTRO DE DIAGNOSTICO",
                "CENTRO DE FISIOTERAPIA Y",
                "CENTRO DE FISIOTERAPIA Y REHABILITACION",
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

            // Para simplificar: como tus items son strings, asignamos directamente un ID simulado
            int idCat = ListaCategorias.SelectedIndex + 1;

            // Obtener centros desde la base de datos
            List<CentrosDeSalud> centros = servicio.ObtenerCentrosPorCategoria(idCat);

            // Limpiar la ObservableCollection antes de agregar
            CentrosUI.Clear();

            // Preparar lista para la UI
            foreach (var centro in centros)
            {
                foreach (var tel in centro.Telefonos)
                {
                    CentrosUI.Add(new
                    {
                        Institucion = centro.Institucion,
                        Direccion = centro.Direccion,
                        Telefono = tel.Telefono,
                        LinkUbi = centro.Link,   // string
                        LinkTelf = tel.LinkT     // string
                    });
                }
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
            if (this.NavigationService != null && this.NavigationService.CanGoBack)
                this.NavigationService.GoBack();
        }
    }
}
