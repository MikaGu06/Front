using System;
using System.Windows;
using System.Windows.Controls;
using Front.SignosVitales;

namespace Front
{
    public partial class SignosVitales : Page
    {
        public SignosVitales()
        {
            InitializeComponent();
        }

        private void BtnVolver_Click(object sender, RoutedEventArgs e)
        {
            ((MainWindow)Application.Current.MainWindow).MainFrame.Navigate(new Servicios());
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Llamamos directamente a la clase de validación
                ValidacionSignosVitales.Validar(
                    txtRitmoCardiaco.Text,
                    txtPresionArterial.Text,
                    txtOxigenacion.Text,
                    txtTemperatura.Text
                );

                // Si no lanzó excepción → todo correcto
                MessageBox.Show("Datos añadidos correctamente.",
                                "Registro exitoso",
                                MessageBoxButton.OK,
                                MessageBoxImage.Information);
            }
            catch (ArgumentException ex)
            {
                MessageBox.Show("ERROR de Validación: " + ex.Message,
                                "Datos Inválidos",
                                MessageBoxButton.OK,
                                MessageBoxImage.Warning);
            }
            catch (FormatException)
            {
                MessageBox.Show("ERROR de Formato: Uno o más valores numéricos no son válidos.",
                                "Datos Inválidos",
                                MessageBoxButton.OK,
                                MessageBoxImage.Warning);
            }
            catch (Exception ex)
            {
                MessageBox.Show("El sistema ha fallado. Reporte el código de error: " + ex.Message,
                                "Fallo Crítico",
                                MessageBoxButton.OK,
                                MessageBoxImage.Error);
            }
        }
    }
}
