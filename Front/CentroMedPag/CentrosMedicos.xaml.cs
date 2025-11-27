using Front.CentroMedPag; // Ajusta si tu namespace es diferente
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;

namespace Front
{
    public partial class CentrosMedicos : Page
    {
        private ServicioCM service;

        public CentrosMedicos()
        {
            InitializeComponent();
            service = new ServicioCM(); // tu clase de conexión SQL

            // Evento para cambio de categoría
            ListaCategorias.SelectionChanged += ListaCategorias_SelectionChanged;
        }

        private void ListaCategorias_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ListaCategorias.SelectedItem == null) return;

            string categoria = (ListaCategorias.SelectedItem as ListBoxItem)?.Content.ToString();

            int idCategoria = categoria switch
            {
                "Farmacias" => 1,
                "Hospitales" => 2,
                "Clínicas" => 3,
                "Laboratorios" => 4,
                "Consultorios" => 5,
                _ => 0
            };

            if (idCategoria == 0) return;

            service.CargarCentrosPorCategoria(idCategoria);

            MostrarCentros(service.ListarCentros());
        }

        private void MostrarCentros(List<ModeloCM> centros)
        {
            spCentros.Children.Clear();

            foreach (var centro in centros)
            {
                Grid row = new Grid { Margin = new Thickness(6), MinHeight = 64 };
                row.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(2, GridUnitType.Star) });
                row.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(2, GridUnitType.Star) });
                row.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(3, GridUnitType.Star) });
                row.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(2, GridUnitType.Star) });

                TextBlock txtInstitucion = new TextBlock { Text = centro.Institucion, FontWeight = FontWeights.Bold };
                Grid.SetColumn(txtInstitucion, 0);
                row.Children.Add(txtInstitucion);

                TextBlock txtCategoria = new TextBlock { Text = centro.Categoria };
                Grid.SetColumn(txtCategoria, 1);
                row.Children.Add(txtCategoria);

                TextBlock txtDireccion = new TextBlock { Text = centro.Direccion };
                Grid.SetColumn(txtDireccion, 2);
                row.Children.Add(txtDireccion);

                Button btnTelefono = new Button { Content = centro.Telefono.ToString(), Width = 100, Height = 30 };
                btnTelefono.Click += (s, e) =>
                {
                    string tel = (s as Button)?.Content.ToString();
                    if (!string.IsNullOrEmpty(tel))
                    {
                        try { Process.Start(new ProcessStartInfo($"tel:{tel}") { UseShellExecute = true }); }
                        catch (Exception ex) { MessageBox.Show("No se pudo iniciar la llamada: " + ex.Message); }
                    }
                };
                Grid.SetColumn(btnTelefono, 3);
                row.Children.Add(btnTelefono);

                spCentros.Children.Add(row);
            }
        }

        private void BtnVolver_Click(object sender, RoutedEventArgs e)
        {
            ((MainWindow)Application.Current.MainWindow).MainFrame.Navigate(new Servicios());
        }
    }
}
