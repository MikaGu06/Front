using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Front
{
    /// <summary>
    /// Lógica de interacción para SignosVitales.xaml
    /// </summary>
    public partial class SignosVitales : Page
    {
        public SignosVitales()
        {
            InitializeComponent();
        }

        private void ButtonAñadir(object sender, RoutedEventArgs e)
        {
            try
            {
                // Obtener valores
                string ritmo = txtRitmoCardiaco.Text;
                string presion = txtPresionArterial.Text;
                string oxigeno = txtOxigenacion.Text;
                string temperatura = txtTemperatura.Text;

                // No puede ver espacios vacios
                if (string.IsNullOrEmpty(ritmo) || string.IsNullOrEmpty(presion) || string.IsNullOrEmpty(oxigeno) || string.IsNullOrEmpty(temperatura))
                {
                    throw new ArgumentException("Todos los campos deben ser completados.");
                }

                // Validación de espacios
                if (ritmo.Contains(" ") || presion.Contains(" ") || oxigeno.Contains(" ") || temperatura.Contains(" "))
                {
                    throw new ArgumentException("Ningún campo puede contener espacios internos.");
                }
                // Validación de Ritmo, Oxigenación y Temperatura
                if (!Regex.IsMatch(ritmo, "^[0-9]+$") || !Regex.IsMatch(oxigeno, "^[0-9]+$") || !Regex.IsMatch(temperatura, "^[0-9]+(\\.[0-9]+)?$"))
                {
                    throw new ArgumentException("Ritmo Cardíaco, Oxigenación y Temperatura deben ser numéricos. Temperatura puede incluir un punto.");
                }

                // Presión arterial
                if (!Regex.IsMatch(presion, "^[0-9]+-[0-9]+$"))
                {
                    throw new ArgumentException("La Presión Arterial debe escribirse en formato 120-80.");
                }

                MessageBox.Show("Datos añadidos correctamente.", "Registro exitoso", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (ArgumentException ex)
            {
                MessageBox.Show("ERROR de Validación: " + ex.Message, "Datos Inválidos", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            catch (Exception ex)
            {
                MessageBox.Show("ERROR DE SISTEMA CRÍTICO NO ANTICIPADO. El sistema ha fallado. Reporte el código de error: " + ex.Message, "Fallo Crítico de Runtime", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }

    
}
