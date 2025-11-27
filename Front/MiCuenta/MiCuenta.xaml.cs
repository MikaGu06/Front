using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Drawing;
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


namespace Front.MiCuenta
{
    /// <summary>
    /// Lógica de interacción para MiCuenta.xaml
    /// </summary>
    public partial class MiCuenta : Page
    {
        private string rutaImagenSeleccionada = null; //valor por defecto de la ruta de la imagen
        public MiCuenta()
        {
            InitializeComponent();
        }

        //recibe el usuario y teléfono. REGISTRAR Y LUEGO MI CUENTA
        public MiCuenta(string usuario, string telefono, string contrasena = null) : this()
        {
            TxtUsuario.Text = usuario;
            TxtTelefono.Text = telefono;

            if (!string.IsNullOrEmpty(contrasena))
            {
                PsBoxContrasena.Password = contrasena;
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            ((MainWindow)Application.Current.MainWindow).MainFrame.Navigate(new Servicios());
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
                // 1️ Crear modelo
                ModeloUsuario usuario = new ModeloUsuario(
                    TxtUsuario.Text,
                    PsBoxContrasena.Password,
                    TxtTelefono.Text,
                    TxtNombre.Text,
                    TxtCorreo.Text,
                    TxtDireccion.Text,
                    TxtEdad.Text,
                    DpFechaNacimiento.SelectedDate,
                    (CbGenero.SelectedItem as ComboBoxItem)?.Content?.ToString(),
                    (CbTipoSangre.SelectedItem as ComboBoxItem)?.Content?.ToString(),
                    rutaImagenSeleccionada
                );

                // 2️ Enviar a validaciones
                ValidacionesMiCuenta.CuentaValidar(usuario);

                // 3️⃣ Guardar (cuando tu compañero haga BD)
                // servicio.ActualizarUsuario(usuario);


                // SI TODAS LAS VALIDACIONES PASARON BLOQUEAR FORMULARIO
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
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Title = "Seleccionar foto de perfil";
            ofd.Filter = "Imágenes (*.jpg;*.jpeg;*.png)|*.jpg;*.jpeg;*.png";
            ofd.FilterIndex = 1;
            ofd.Multiselect = false;

            if (ofd.ShowDialog() == true)
            {
                string rutaImagen = ofd.FileName; //guardando la ruta de imagen seleccionada

                try
                {
                    BitmapImage foto = new BitmapImage();
                    foto.BeginInit();
                    foto.UriSource = new Uri(rutaImagen);
                    foto.EndInit();
                    foto.Freeze();

                    FotoPerfil.Source = foto;
                }
                catch (Exception)
                {
                    MessageBox.Show("Ocurrió un error al cargar la imagen seleccionada:", "Error de Imagen", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
    }
}