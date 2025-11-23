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
    /// Lógica de interacción para MiCuenta.xaml
    /// </summary>
    public partial class MiCuenta : Page
    {
        public MiCuenta()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Navegación", "Servicio", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        /// Habilita la edición del formulario cuando se presiona el botón Modificar.
        private void BtnModificar_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                FormularioDatos.IsEnabled = true;
                BtnGuardar.IsEnabled = true;
                BtnModificar.IsEnabled = false;

                MessageBox.Show("Modo de edición activado. Puede proceder a modificar sus datos.", "Modificación", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show(string.Format("Ocurrió un error al activar el modo de edición:", ex.Message), "Error General", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnGuardar_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // 1. OBTENER VALORES
                string nombre = TxtNombre.Text;
                string correo = TxtCorreo.Text;
                string telefono = TxtTelefono.Text;
                string direccion = TxtDireccion.Text;
                string edadString = TxtEdad.Text;
                DateTime? fechaNacimiento = DpFechaNacimiento.SelectedDate;

                string genero = (CbGenero.SelectedItem as ComboBoxItem)?.Content?.ToString();
                string tipoSangre = (CbTipoSangre.SelectedItem as ComboBoxItem)?.Content?.ToString();

                int edadIngresada = 0;

                // 2. NO PUEDE S VER CAMPOS VACIOS 
                if (string.IsNullOrWhiteSpace(nombre) ||
                    string.IsNullOrWhiteSpace(correo) ||
                    string.IsNullOrWhiteSpace(telefono) ||
                    string.IsNullOrWhiteSpace(direccion) ||
                    string.IsNullOrWhiteSpace(edadString) ||
                    !fechaNacimiento.HasValue ||
                    string.IsNullOrEmpty(genero) ||
                    string.IsNullOrEmpty(tipoSangre))
                {
                    throw new ArgumentException("Validación General: Todos los campos del formulario deben ser completados.");
                }

                // 3. VALIDACIÓN CONCISA: Nombre Completo (solo formato y longitud, ya se verificó que no está vacío)
                if (nombre.Length < 3 || nombre.Length > 60 ||
                    !Regex.IsMatch(nombre, @"^[a-zA-Z\s]+$") ||
                    nombre.Trim().Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries).Length < 2)
                {
                    throw new ArgumentException("Nombre Completo: Debe tener 2+ palabras, 3-60 caracteres y solo letras/espacios.");
                }

                // 4. VALIDACIÓN: Correo Electronico
                string emailPattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";

                if (!Regex.IsMatch(correo, emailPattern) || correo.Length > 100)
                {
                    throw new ArgumentException("Correo Electrónico: Debe cumplir formato estándar (ej: usuario@dominio.com) y no superar los 100 caracteres.");
                }

                // 5. VALIDACIÓN: Telefono
                if (!Regex.IsMatch(telefono, @"^\d+$") || telefono.Length < 7 || telefono.Length > 15)
                {
                    throw new ArgumentException("Teléfono: Solo números, entre 7 y 15 dígitos.");
                }

                // 6. VALIDACIÓN : Dirección
                if (direccion.Length > 100)
                {
                    throw new ArgumentException("Dirección: Debe tener como máximo 100 caracteres.");
                }

                // 7. VALIDACIÓN: Edad
                if (!int.TryParse(edadString, out edadIngresada) || edadIngresada < 1 || edadIngresada > 120)
                {
                    throw new ArgumentException("Edad: Solo números, rango 1 a 120 años.");
                }

                // 8. VALIDACIÓN CONCISA: Género y Tipo de Sangre 
                string[] validosGenero = { "Masculino", "Femenino" };
                if (!validosGenero.Contains(genero))
                {
                    throw new ArgumentException("Género: Debe seleccionar un valor válido ('Masculino' o 'Femenino').");
                }

                string[] validosSangre = { "A+", "A-", "B+", "B-", "AB+", "AB-", "O+", "O-" };
                if (!validosSangre.Contains(tipoSangre))
                {
                    throw new ArgumentException("Tipo de Sangre: Debe seleccionar un valor válido (ej: A+, O-).");
                }

                // 9. VALIDACIÓN : Fecha de Nacimiento 
                if (fechaNacimiento.Value.Date >= DateTime.Today)
                {
                    throw new ArgumentException("Fecha de Nacimiento: Debe ser menor a la fecha actual.");
                }

                int edadCalculada = DateTime.Today.Year - fechaNacimiento.Value.Year;
                if (fechaNacimiento.Value.Date > DateTime.Today.AddYears(-edadCalculada))
                {
                    edadCalculada = edadCalculada - 1;
                }

                if (edadCalculada != edadIngresada || edadCalculada < 1 || edadCalculada > 120)
                {
                    throw new ArgumentException(string.Format("Fecha de Nacimiento: La edad calculada no es coherente con la Edad ingresada .", edadCalculada, edadIngresada));
                }


                // SI TODAS LAS VALIDACIONES PASARON
                FormularioDatos.IsEnabled = false;
                BtnGuardar.IsEnabled = false;
                BtnModificar.IsEnabled = true;

                MessageBox.Show("¡SE GUARDÓ CORRECTAMENTE!",
                                "Guardado Exitoso",
                                MessageBoxButton.OK,
                                MessageBoxImage.Information);
            }
            catch (ArgumentException ex)
            {
                // CAPTURA ÚNICA DE ERRORES DE VALIDACIÓN
                MessageBox.Show("ERROR de Validación: " + ex.Message,"Datos Inválidos",MessageBoxButton.OK,MessageBoxImage.Warning);
            }
            catch (Exception ex)
            {
                // CAPTURA ERRORES CRÍTICOS NO ANTICIPADOS
                MessageBox.Show(string.Format("ERROR DE SISTEMA CRÍTICO NO ANTICIPADO. El sistema ha fallado. Reporte el código de error:", ex.Message),"Fallo Crítico de Runtime",MessageBoxButton.OK,MessageBoxImage.Error);
            }
        }

        private void BtnCambiarFoto_Click(object sender, RoutedEventArgs e)
        {
           
        }
    }
}