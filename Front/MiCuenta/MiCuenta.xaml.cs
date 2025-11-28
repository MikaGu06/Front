using Front.Helpers;
using Microsoft.Win32;
using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;

namespace Front.MiCuenta
{
    /// Página de edición de datos de la cuenta del usuario.
    public partial class MiCuenta : Page
    {
        // Cadena de conexión (igual que Database.cs)
        private readonly string _connectionString =
            ConfigurationManager.ConnectionStrings["cnHealthyU"].ConnectionString;

        private string rutaImagenSeleccionada = null; // ruta de la imagen seleccionada
        private byte[] fotoPerfilActual = null;       // bytes de la foto actual
        private string usuarioActual;                 // nombre de usuario en sesión

        public MiCuenta()
        {
            InitializeComponent();

            CargarDesdeSesion();
            CargarFotoDesdeBaseDeDatos();
        }

        /// Rellena el formulario con los datos de la sesión.
        private void CargarDesdeSesion()
        {
            usuarioActual = SesionUsuario.NombreUsuario;

            if (!string.IsNullOrEmpty(usuarioActual))
                TxtUsuario.Text = usuarioActual;

            if (!string.IsNullOrEmpty(SesionUsuario.Telefono))
                TxtTelefono.Text = SesionUsuario.Telefono;

            if (!string.IsNullOrEmpty(SesionUsuario.Contrasena))
                PsBoxContrasena.Password = SesionUsuario.Contrasena;

            if (!string.IsNullOrEmpty(SesionUsuario.NombreCompleto))
                TxtNombre.Text = SesionUsuario.NombreCompleto;

            if (!string.IsNullOrEmpty(SesionUsuario.Correo))
                TxtCorreo.Text = SesionUsuario.Correo;

            if (!string.IsNullOrEmpty(SesionUsuario.Direccion))
                TxtDireccion.Text = SesionUsuario.Direccion;

            if (!string.IsNullOrEmpty(SesionUsuario.Edad))
                TxtEdad.Text = SesionUsuario.Edad;

            if (SesionUsuario.FechaNacimiento.HasValue)
                DpFechaNacimiento.SelectedDate = SesionUsuario.FechaNacimiento.Value;

            if (!string.IsNullOrEmpty(SesionUsuario.Genero))
            {
                foreach (ComboBoxItem item in CbGenero.Items)
                {
                    if (item.Content.ToString() == SesionUsuario.Genero)
                    {
                        CbGenero.SelectedItem = item;
                        break;
                    }
                }
            }

            if (!string.IsNullOrEmpty(SesionUsuario.TipoSangre))
            {
                foreach (ComboBoxItem item in CbTipoSangre.Items)
                {
                    if (item.Content.ToString() == SesionUsuario.TipoSangre)
                    {
                        CbTipoSangre.SelectedItem = item;
                        break;
                    }
                }
            }

            if (!string.IsNullOrEmpty(SesionUsuario.CI))
                TxtCI.Text = SesionUsuario.CI;

            if (SesionUsuario.FotoPerfil != null && SesionUsuario.FotoPerfil.Length > 0)
            {
                fotoPerfilActual = SesionUsuario.FotoPerfil;
                FotoPerfil.Source = ConvertirBytesAImagen(fotoPerfilActual);
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            // Volver a Servicios
            ((MainWindow)Application.Current.MainWindow).MainFrame.Navigate(new Servicios());
        }

        /// Activa el modo edición del formulario.
        private void BtnModificar_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                FormularioDatos.IsEnabled = true;
                BtnGuardar.IsEnabled = true;
                BtnModificar.IsEnabled = false;

                MessageBox.Show(
                    "Modo edición activado.",
                    "Modificación",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Error al activar el modo edición: {ex.Message}",
                    "Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        /// Guarda cambios de MiCuenta en sesión y BD (Paciente + Usuario.ci_paciente).
        private void BtnGuardar_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // 1) Crear modelo con los datos del formulario
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
                    rutaImagenSeleccionada,
                    TxtCI.Text
                );

                // 2) Validar datos
                ValidacionesMiCuenta.CuentaValidar(usuario);

                // 3) Guardar en sesión
                SesionUsuario.NombreUsuario = usuario.Usuario;
                SesionUsuario.Contrasena = usuario.Contrasena;
                SesionUsuario.Telefono = usuario.Telefono;
                SesionUsuario.NombreCompleto = usuario.Nombre;
                SesionUsuario.Correo = usuario.Correo;
                SesionUsuario.Direccion = usuario.Direccion;
                SesionUsuario.Edad = usuario.Edad;
                SesionUsuario.FechaNacimiento = usuario.FechaNacimiento;
                SesionUsuario.Genero = usuario.Genero;
                SesionUsuario.TipoSangre = usuario.TipoSangre;
                SesionUsuario.CI = usuario.CI;
                SesionUsuario.FotoPerfil = fotoPerfilActual;

                // 4) Crear o actualizar Paciente
                GuardarPacienteDesdeMiCuenta(usuario);

                // 5) Bloquear de nuevo el formulario
                FormularioDatos.IsEnabled = false;
                BtnGuardar.IsEnabled = false;
                BtnModificar.IsEnabled = true;

                MessageBox.Show(
                    "Datos guardados correctamente.",
                    "Guardado exitoso",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);
            }
            catch (ArgumentException ex)
            {
                MessageBox.Show(
                    "ERROR de validación: " + ex.Message,
                    "Datos inválidos",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Error inesperado: {ex.Message}",
                    "Error del sistema",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        /// Permite seleccionar y actualizar la foto de perfil.
        private void BtnCambiarFoto_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog
            {
                Title = "Seleccionar foto de perfil",
                Filter = "Imágenes (*.jpg;*.jpeg;*.png)|*.jpg;*.jpeg;*.png",
                FilterIndex = 1,
                Multiselect = false
            };

            if (ofd.ShowDialog() == true)
            {
                rutaImagenSeleccionada = ofd.FileName;

                try
                {
                    // Mostrar imagen
                    BitmapImage foto = new BitmapImage();
                    foto.BeginInit();
                    foto.UriSource = new Uri(rutaImagenSeleccionada);
                    foto.CacheOption = BitmapCacheOption.OnLoad;
                    foto.EndInit();
                    foto.Freeze();
                    FotoPerfil.Source = foto;

                    // Guardar bytes en memoria y en sesión
                    fotoPerfilActual = File.ReadAllBytes(rutaImagenSeleccionada);
                    SesionUsuario.FotoPerfil = fotoPerfilActual;

                    // Guardar en BD si ya tiene Paciente asociado
                    GuardarFotoEnBaseDeDatos();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(
                        "Error al cargar la imagen: " + ex.Message,
                        "Error de imagen",
                        MessageBoxButton.OK,
                        MessageBoxImage.Error);
                }
            }
        }

        /// Carga la foto de perfil desde la BD (Paciente.foto_perfil).
        private void CargarFotoDesdeBaseDeDatos()
        {
            if (string.IsNullOrEmpty(usuarioActual))
                return;

            try
            {
                using (SqlConnection conn = new SqlConnection(_connectionString))
                using (SqlCommand cmd = new SqlCommand(
                    @"SELECT p.foto_perfil
                      FROM Usuario u
                      INNER JOIN Paciente p ON u.ci_paciente = p.ci_paciente
                      WHERE u.nombre_usuario = @user", conn))
                {
                    cmd.Parameters.AddWithValue("@user", usuarioActual);

                    conn.Open();
                    object result = cmd.ExecuteScalar();

                    if (result != DBNull.Value && result != null)
                    {
                        fotoPerfilActual = (byte[])result;
                        SesionUsuario.FotoPerfil = fotoPerfilActual;
                        FotoPerfil.Source = ConvertirBytesAImagen(fotoPerfilActual);
                    }
                }
            }
            catch (SqlException ex)
            {
                MessageBox.Show(
                    "No se pudo cargar la foto de perfil (SQL): " + ex.Message,
                    "Error SQL",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "No se pudo cargar la foto de perfil: " + ex.Message,
                    "Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);
            }
        }


        /// Guarda la foto en la tabla Paciente.foto_perfil
        private void GuardarFotoEnBaseDeDatos()
        {
            if (fotoPerfilActual == null || fotoPerfilActual.Length == 0)
                return;

            if (string.IsNullOrEmpty(usuarioActual))
                return;

            try
            {
                using (SqlConnection conn = new SqlConnection(_connectionString))
                {
                    conn.Open();

                    int? ciPaciente = null;

                    // Obtener CI del Paciente vinculado
                    using (SqlCommand cmdGet = new SqlCommand(
                        "SELECT ci_paciente FROM Usuario WHERE nombre_usuario = @user", conn))
                    {
                        cmdGet.Parameters.AddWithValue("@user", usuarioActual);
                        object result = cmdGet.ExecuteScalar();
                        if (result != DBNull.Value && result != null)
                            ciPaciente = Convert.ToInt32(result);
                    }

                    if (!ciPaciente.HasValue)
                    {
                        MessageBox.Show(
                            "Este usuario aún no tiene Paciente vinculado.\n" +
                            "Primero guarda los datos de Mi Cuenta.",
                            "Sin paciente asociado",
                            MessageBoxButton.OK,
                            MessageBoxImage.Warning);
                        return;
                    }

                    // Actualizar foto
                    using (SqlCommand cmdUpdate = new SqlCommand(
                        @"UPDATE Paciente
                          SET foto_perfil = @foto
                          WHERE ci_paciente = @ci", conn))
                    {
                        cmdUpdate.Parameters.Add("@foto", SqlDbType.VarBinary).Value = fotoPerfilActual;
                        cmdUpdate.Parameters.AddWithValue("@ci", ciPaciente.Value);

                        cmdUpdate.ExecuteNonQuery();
                    }
                }
            }
            catch (SqlException ex)
            {
                MessageBox.Show(
                    "No se pudo guardar la foto en la BD (SQL): " + ex.Message,
                    "Error SQL",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "No se pudo guardar la foto en la BD: " + ex.Message,
                    "Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        /// Crea o actualiza el registro de Paciente y vincula Usuario.ci_paciente.
        private void GuardarPacienteDesdeMiCuenta(ModeloUsuario u)
        {
            if (string.IsNullOrEmpty(usuarioActual))
                return;

            // CI numérico
            if (!int.TryParse(u.CI, out int ciNumerico))
                throw new ArgumentException("CI: Debe ser numérico.");

            int edadInt = 0;
            int.TryParse(u.Edad, out edadInt);

            byte[] bytesFoto = fotoPerfilActual ?? SesionUsuario.FotoPerfil;

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                using (SqlTransaction tx = conn.BeginTransaction())
                {
                    try
                    {
                        // 1) Resolver id_tipo_sangre
                        int idTipo;
                        using (SqlCommand cmdTipo = new SqlCommand(
                            "SELECT id_tipo_sangre FROM Tipo_sangre WHERE codigo = @codigo",
                            conn, tx))
                        {
                            cmdTipo.Parameters.AddWithValue("@codigo", u.TipoSangre);
                            object resTipo = cmdTipo.ExecuteScalar();

                            if (resTipo != null && resTipo != DBNull.Value)
                            {
                                idTipo = Convert.ToInt32(resTipo);
                            }
                            else
                            {
                                using (SqlCommand cmdNuevoId = new SqlCommand(
                                    "SELECT ISNULL(MAX(id_tipo_sangre),0) + 1 FROM Tipo_sangre",
                                    conn, tx))
                                {
                                    idTipo = Convert.ToInt32(cmdNuevoId.ExecuteScalar());
                                }

                                using (SqlCommand cmdInsertTipo = new SqlCommand(
                                    "INSERT INTO Tipo_sangre(id_tipo_sangre, codigo) VALUES(@id, @codigo)",
                                    conn, tx))
                                {
                                    cmdInsertTipo.Parameters.AddWithValue("@id", idTipo);
                                    cmdInsertTipo.Parameters.AddWithValue("@codigo", u.TipoSangre);
                                    cmdInsertTipo.ExecuteNonQuery();
                                }
                            }
                        }

                        bool esMasculino = (u.Genero == "Masculino");

                        // 2) Verificar si el Paciente ya existe por CI
                        bool existePaciente;
                        using (SqlCommand cmdExiste = new SqlCommand(
                            "SELECT COUNT(*) FROM Paciente WHERE ci_paciente = @ci",
                            conn, tx))
                        {
                            cmdExiste.Parameters.AddWithValue("@ci", ciNumerico);
                            existePaciente = (int)cmdExiste.ExecuteScalar() > 0;
                        }

                        if (!existePaciente)
                        {
                            // Insertar nuevo Paciente
                            using (SqlCommand cmdInsertPac = new SqlCommand(
                                @"INSERT INTO Paciente
                                  (ci_paciente, id_tipo_sangre, id_centro, correo, nombre_completo,
                                   celular, direccion, sexo, foto_perfil, fecha_nacimiento, edad)
                                  VALUES (@ci, @idTipo, NULL, @correo, @nombre,
                                          @cel, @dir, @sexo, @foto, @fecha, @edad)",
                                conn, tx))
                            {
                                cmdInsertPac.Parameters.AddWithValue("@ci", ciNumerico);
                                cmdInsertPac.Parameters.AddWithValue("@idTipo", idTipo);
                                cmdInsertPac.Parameters.AddWithValue("@correo", u.Correo);
                                cmdInsertPac.Parameters.AddWithValue("@nombre", u.Nombre);
                                cmdInsertPac.Parameters.AddWithValue("@cel", (object)u.Telefono ?? DBNull.Value);
                                cmdInsertPac.Parameters.AddWithValue("@dir", (object)u.Direccion ?? DBNull.Value);

                                cmdInsertPac.Parameters.AddWithValue(
                                    "@sexo",
                                    u.Genero == null ? (object)DBNull.Value : (esMasculino ? 1 : 0));

                                if (bytesFoto != null && bytesFoto.Length > 0)
                                    cmdInsertPac.Parameters.Add("@foto", SqlDbType.VarBinary).Value = bytesFoto;
                                else
                                    cmdInsertPac.Parameters.Add("@foto", SqlDbType.VarBinary).Value = DBNull.Value;

                                cmdInsertPac.Parameters.AddWithValue("@fecha", u.FechaNacimiento.Value);
                                cmdInsertPac.Parameters.AddWithValue("@edad", edadInt);

                                cmdInsertPac.ExecuteNonQuery();
                            }
                        }
                        else
                        {
                            // Actualizar Paciente existente
                            using (SqlCommand cmdUpdPac = new SqlCommand(
                                @"UPDATE Paciente
                                  SET id_tipo_sangre   = @idTipo,
                                      correo          = @correo,
                                      nombre_completo = @nombre,
                                      celular         = @cel,
                                      direccion       = @dir,
                                      sexo            = @sexo,
                                      foto_perfil     = @foto,
                                      fecha_nacimiento = @fecha,
                                      edad            = @edad
                                  WHERE ci_paciente = @ci",
                                conn, tx))
                            {
                                cmdUpdPac.Parameters.AddWithValue("@ci", ciNumerico);
                                cmdUpdPac.Parameters.AddWithValue("@idTipo", idTipo);
                                cmdUpdPac.Parameters.AddWithValue("@correo", u.Correo);
                                cmdUpdPac.Parameters.AddWithValue("@nombre", u.Nombre);
                                cmdUpdPac.Parameters.AddWithValue("@cel", (object)u.Telefono ?? DBNull.Value);
                                cmdUpdPac.Parameters.AddWithValue("@dir", (object)u.Direccion ?? DBNull.Value);

                                cmdUpdPac.Parameters.AddWithValue(
                                    "@sexo",
                                    u.Genero == null ? (object)DBNull.Value : (esMasculino ? 1 : 0));

                                if (bytesFoto != null && bytesFoto.Length > 0)
                                    cmdUpdPac.Parameters.Add("@foto", SqlDbType.VarBinary).Value = bytesFoto;
                                else
                                    cmdUpdPac.Parameters.Add("@foto", SqlDbType.VarBinary).Value = DBNull.Value;

                                cmdUpdPac.Parameters.AddWithValue("@fecha", u.FechaNacimiento.Value);
                                cmdUpdPac.Parameters.AddWithValue("@edad", edadInt);

                                cmdUpdPac.ExecuteNonQuery();
                            }
                        }

                        // 3) Asegurar que Usuario.ci_paciente apunte a este CI
                        using (SqlCommand cmdUpdUsr = new SqlCommand(
                            "UPDATE Usuario SET ci_paciente = @ci WHERE nombre_usuario = @user",
                            conn, tx))
                        {
                            cmdUpdUsr.Parameters.AddWithValue("@ci", ciNumerico);
                            cmdUpdUsr.Parameters.AddWithValue("@user", usuarioActual);
                            cmdUpdUsr.ExecuteNonQuery();
                        }

                        tx.Commit();
                    }
                    catch
                    {
                        tx.Rollback();
                        throw;
                    }
                }
            }
        }


        /// Convierte un arreglo de bytes en BitmapImage.

        private BitmapImage ConvertirBytesAImagen(byte[] imageData)
        {
            if (imageData == null || imageData.Length == 0)
                return null;

            BitmapImage imagen = new BitmapImage();
            using (MemoryStream ms = new MemoryStream(imageData))
            {
                imagen.BeginInit();
                imagen.CacheOption = BitmapCacheOption.OnLoad;
                imagen.StreamSource = ms;
                imagen.EndInit();
                imagen.Freeze();
            }
            return imagen;
        }
    }
}
