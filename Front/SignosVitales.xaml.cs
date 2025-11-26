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

        private void BtnVolver_Click(object sender, RoutedEventArgs e)
        {
            ((MainWindow)Application.Current.MainWindow).MainFrame.Navigate(new Servicios());
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string ritmo = txtRitmoCardiaco.Text;
                string presion = txtPresionArterial.Text;
                string oxigeno = txtOxigenacion.Text;
                string temperatura = txtTemperatura.Text;

                // --- 1. No puede ver espacios vacios
                if (string.IsNullOrEmpty(ritmo) || string.IsNullOrEmpty(presion) || string.IsNullOrEmpty(oxigeno) || string.IsNullOrEmpty(temperatura))
                {
                    throw new ArgumentException("Todos los campos deben ser completados.");
                }

                // --- 2. Validación de espacios internos
                if (ritmo.Contains(" ") || presion.Contains(" ") || oxigeno.Contains(" ") || temperatura.Contains(" "))
                {
                    throw new ArgumentException("Ningún campo puede contener espacios internos.");
                }

                // --- 3. Validación de Formato Numérico y Separación de Presión Arterial

                // Validaciones de Formato
                if (!Regex.IsMatch(ritmo, "^[0-9]+$") || !Regex.IsMatch(oxigeno, "^[0-9]+$") || !Regex.IsMatch(temperatura, "^[0-9]+(\\.[0-9]+)?$"))
                {
                    throw new ArgumentException("Ritmo Cardíaco, Oxigenación y Temperatura deben ser numéricos. Temperatura puede incluir un punto.");
                }

                // Separación de Presión Arterial
                if (!Regex.IsMatch(presion, "^[0-9]+-[0-9]+$"))
                {
                    throw new ArgumentException("La Presión Arterial debe escribirse en formato 120-80.");
                }
                string[] partesPresion = presion.Split('-');
                if (partesPresion.Length != 2 || !int.TryParse(partesPresion[0], out int sistolica) || !int.TryParse(partesPresion[1], out int diastolica))
                {
                    throw new ArgumentException("Error al procesar la Presión Arterial. Asegúrese de usar el formato NNN-NNN.");
                }

                // --- 4. Conversión a Tipos Numéricos
                int ritmoCardiaco = int.Parse(ritmo);
                int oxigenacion = int.Parse(oxigeno);
                double temperaturaValor = double.Parse(temperatura, System.Globalization.CultureInfo.InvariantCulture);


                // --- 5. VALIDACIONES DE RANGO 

                // Ritmo Cardiaco//////////////

                // Debe ser mayor a 30 y menor a 250.
                if (ritmoCardiaco <= 30 || ritmoCardiaco >= 250)
                {
                    throw new ArgumentException("Ritmo Cardíaco fuera de rango válido (31 a 249).");
                }

                // Presión Arterial///////////////////

                // Sistólica (la más alta): debe ser mayor a 70 y menor a 250.
                if (sistolica <= 70 || sistolica >= 250)
                {
                    throw new ArgumentException("Presión Sistólica fuera de rango válido (71 a 249).");
                }
                // Diastólica (la más baja): debe ser mayor a 40 y menor a 150.
                if (diastolica <= 40 || diastolica >= 150)
                {
                    throw new ArgumentException("Presión Diastólica fuera de rango válido (41 a 149).");
                }
                // Control adicional: La sistólica debe ser mayor que la diastólica
                if (sistolica <= diastolica)
                {
                    throw new ArgumentException("La Presión Sistólica debe ser mayor que la Diastólica.");
                }

                // Oxigenación///////////////

                // Debe ser mayor a 50 y menor o igual a 100.
                if (oxigenacion <= 50 || oxigenacion > 100)
                {
                    throw new ArgumentException("Oxigenación fuera de rango válido (51 a 100).");
                }

                // Temperatura//////////////

                // Debe ser mayor a 32 y menor a 42.
                if (temperaturaValor <= 32.0 || temperaturaValor >= 42.0)
                {
                    throw new ArgumentException("Temperatura fuera de rango válido (32.1 a 41.9).");
                }
                // Si todas las validaciones son exitosas
                MessageBox.Show("Datos añadidos correctamente.", "Registro exitoso", MessageBoxButton.OK, MessageBoxImage.Information);

            }
            catch (ArgumentException ex)
            {
                MessageBox.Show("ERROR de Validación: " + ex.Message, "Datos Inválidos", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            catch (FormatException ex)
            {
                MessageBox.Show("ERROR de Formato: Uno o más valores numéricos no son válidos.", "Datos Inválidos", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            catch (Exception ex)
            {
                MessageBox.Show("El sistema ha fallado. Reporte el código de error: " + ex.Message, "Fallo Crítico", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
