using System;
using System.Collections.Generic;
using System.Text;
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
    /// Lógica de interacción para Medicamentos.xaml
    /// </summary>
    public partial class Medicamentos : Page
    {
        public Medicamentos()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string dia = txtDia.Text.Trim();
                string hora = txtHora.Text.Trim();
                string medicamento = txtMedicamento.Text.Trim();

                // 2. Todos los campos rellenados
                if (string.IsNullOrWhiteSpace(dia) ||
                    string.IsNullOrWhiteSpace(hora) ||
                    string.IsNullOrWhiteSpace(medicamento))
                {
                    throw new ArgumentException("Todos los campos (Día, Hora, Medicamento) son obligatorios y no deben estar vacíos.");
                }

                // 3. Validación de formato y lógica detallada/////
                // 3.1. Día fecha real
                if (!DateTime.TryParse(dia, out DateTime fechaRecordatorio))
                {
                    throw new ArgumentException("El formato del Día es inválido. Ingrese una fecha real (ej: AAAA-MM-DD).");
                }

                // 3.2. Formato de Hora
                if (!DateTime.TryParse(hora, out DateTime horaObtenida))
                {
                    throw new ArgumentException("El formato de la Hora es inválido. Use el formato HH:MM o similar.");
                }

                // 3.3. Medicamento minimo 3 letras
                if (medicamento.Length < 3)
                {
                    throw new ArgumentException("El nombre del Medicamento debe tener al menos 3 letras.");
                }

                // 3.4. E
                bool esActivo = rbActivo.IsChecked ?? false;

                // 4. Lógica de Negocio y Guardado
                DateTime fechaHoraFinal = fechaRecordatorio.Date.Add(horaObtenida.TimeOfDay);
                MessageBox.Show("Recordatorio guardado correctamente.", "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);

                ////AL MOMENTO DE GUARDAR REINICIA TODO 
                txtDia.Text = string.Empty;
                txtHora.Text = string.Empty;
                txtMedicamento.Text = string.Empty;
            }
            catch (ArgumentException ex)
            {
                MessageBox.Show("ERROR de Validación: " + ex.Message, "Datos Inválidos", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            catch (Exception ex)
            {
                MessageBox.Show("ERROR DE SISTEMA. No se pudo completar la operación. Detalle: " + ex.Message, "Fallo Crítico", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
    }

