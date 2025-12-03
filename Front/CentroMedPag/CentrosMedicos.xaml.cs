using Front.CentroMedPag.ModelosCM;
using Front.CentroMedPag.ServiciosCM;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq; // Necesario para la función .Select() y .FirstOrDefault()
using System.Windows;
using System.Windows.Controls;

namespace Front
{
    public partial class CentrosMedicos : Page
    {
        // Se mantiene la inicialización del servicio
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
            // Se mantiene la lista hardcodeada por solicitud
            ListaCategorias.ItemsSource = new List<string>
            {
                "AMBULANCIA",
                "CENTRO DE DIAGNOSTICO",
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

            int idCat = ListaCategorias.SelectedIndex + 1;
            List<CentrosDeSalud> centros = servicio.ObtenerCentrosPorCategoria(idCat);

            CentrosUI.Clear();

            foreach (var centro in centros)
            {
                string telefonosConcatenados = centro.Telefonos.Any()
                    ? string.Join(Environment.NewLine, centro.Telefonos.Select(t => t.Telefono))
                    : "No disponible";

                
                var telefonoConLink = centro.Telefonos
            
                    .FirstOrDefault(t => t.Tienew && !string.IsNullOrEmpty(t.LinkT));

                // 3. Establecer las propiedades para el objeto de la UI
                string linkWhatsApp = telefonoConLink?.LinkT;

                
                bool mostrarBoton = telefonoConLink != null;

                CentrosUI.Add(new
                {
                    Institucion = centro.Institucion,
                    Direccion = centro.Direccion,
                    Telefono = telefonosConcatenados,
                    LinkUbi = centro.Link,
                    LinkTelf = linkWhatsApp,      
                    MostrarBotonWA = mostrarBoton 
                });
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