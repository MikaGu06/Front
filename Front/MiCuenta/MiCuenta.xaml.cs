using Front.Helpers;
using Microsoft.Win32;
using System;
using System.Configuration;
using System.Data;
using Microsoft.Data.SqlClient;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using Front.CentroMedPag.ModelosCM;

namespace Front.MiCuenta
{
    public partial class MiCuenta : Page
    {
        // Cadena de conexión (igual que Database.cs)
        private readonly string _connectionString =
            ConfigurationManager.ConnectionStrings["cnHealthyU"].ConnectionString;

        // campo de clase
        private readonly Front.CentroMedPag.ServiciosCM.CentroDeSaludServicio centroServicio = new Front.CentroMedPag.ServiciosCM.CentroDeSaludServicio();


        private string rutaImagenSeleccionada = string.Empty; // ruta de la imagen seleccionada
        private byte[] fotoPerfilActual = Array.Empty<byte>();       // bytes de la foto actual
        private string usuarioActual = string.Empty;                  // nombre de usuario en sesión
        private readonly bool _esRegistroNuevo;                       // indica si venimos de un registro nuevo

        public MiCuenta(bool esRegistroNuevo = false)
        {
            InitializeComponent();
            

            _esRegistroNuevo = esRegistroNuevo;

            // 1) Tomamos el usuario que inició sesión
            usuarioActual = SesionUsuario.NombreUsuario ?? string.Empty;


            if (string.IsNullOrEmpty(usuarioActual))
            {
                MessageBox.Show("No hay usuario en sesión.", "Sesión",
                                MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // 2) Comportamiento según origen
            if (_esRegistroNuevo)
            {
                // Cuenta recién creada: solo usuario, teléfono y contraseña rellenados.
                CargarSoloDatosBasicosDesdeSesion();
            }
            else
            {
                // Login normal: llenar todo desde la BD y luego desde Sesión.
                CargarUsuarioDesdeBD();
                CargarDesdeSesion();
                CargarFotoDesdeBaseDeDatos();
                CargarFavoritos();
            }
        }

        private void CargarFavoritos()
        {
            try
            {
                
                if (string.IsNullOrWhiteSpace(SesionUsuario.CI) || !int.TryParse(SesionUsuario.CI, out int ciUsuario))
                {
                    
                    ListaFavoritos.ItemsSource = null;
                    return;
                }

                var favoritos = centroServicio.ObtenerFavoritosUsuario(ciUsuario);

                
                var ui = favoritos.Select(c => new
                {
                    Id_centro = c.Id_centro,
                    Institucion = c.Institucion,
                    Direccion = c.Direccion,
                    Telefonos = c.Telefonos != null && c.Telefonos.Any()
                                ? string.Join(" - ", c.Telefonos.Select(t => t.Telefono))
                                : "No disponible",
                    Link = c.Link
                }).ToList();

                ListaFavoritos.ItemsSource = ui;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error cargando favoritos: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                ListaFavoritos.ItemsSource = null;
            }
        }



        /// Rellena solo usuario, teléfono y contraseña desde la sesión (registro nuevo).
        private void CargarSoloDatosBasicosDesdeSesion()
        {
            TxtUsuario.Text = SesionUsuario.NombreUsuario ?? string.Empty;
            TxtTelefono.Text = SesionUsuario.Telefono ?? string.Empty;
            PsBoxContrasena.Password = SesionUsuario.Contrasena ?? string.Empty;

            TxtNombre.Text = string.Empty;
            TxtCorreo.Text = string.Empty;
            TxtDireccion.Text = string.Empty;
            TxtCI.Text = string.Empty;
            DpFechaNacimiento.SelectedDate = null;
            CbGenero.SelectedIndex = -1;
            CbTipoSangre.SelectedIndex = -1;

            fotoPerfilActual = Array.Empty<byte>();
            FotoPerfil.Source = null;
        }

        /// Rellena el formulario con los datos de la sesión.
        private void CargarDesdeSesion()
        {
            usuarioActual = SesionUsuario.NombreUsuario ?? string.Empty;

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

        private void CargarUsuarioDesdeBD()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(_connectionString))
                using (SqlCommand cmd = new SqlCommand(
                    @"SELECT
                u.nombre_usuario,
                u.contrasena_hash as contrasena,
                p.celular,
                p.nombre_completo,
                p.correo,
                p.direccion,
                p.fecha_nacimiento,
                p.sexo,
                p.id_tipo_sangre,
                p.ci_paciente,
                p.foto_perfil
              FROM Usuario u
              LEFT JOIN Paciente p ON u.ci_paciente = p.ci_paciente
              WHERE u.nombre_usuario = @user", conn))
                {
                    cmd.Parameters.AddWithValue("@user", usuarioActual);
                    conn.Open();

                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        if (dr.Read())
                        {
                            // Llenamos la sesión con lo que viene de la BD
                            SesionUsuario.NombreUsuario = dr["nombre_usuario"]?.ToString() ?? string.Empty;

                            // NO sobreescribimos la contraseña de sesión con el hash binario
                            // SesionUsuario.Contrasena = dr["contrasena"]?.ToString();

                            SesionUsuario.Telefono = dr["celular"]?.ToString() ?? string.Empty;
                            SesionUsuario.NombreCompleto = dr["nombre_completo"]?.ToString() ?? string.Empty;
                            SesionUsuario.Correo = dr["correo"]?.ToString() ?? string.Empty;
                            SesionUsuario.Direccion = dr["direccion"]?.ToString() ?? string.Empty;

                            if (dr["fecha_nacimiento"] != DBNull.Value)
                                SesionUsuario.FechaNacimiento = Convert.ToDateTime(dr["fecha_nacimiento"]);
                            else
                                SesionUsuario.FechaNacimiento = null;

                            // Género: en la BD es 1 = Masculino, 0 = Femenino (según lo que ya usas)
                            if (dr["sexo"] != DBNull.Value)
                            {
                                int sexo = Convert.ToInt32(dr["sexo"]);
                                SesionUsuario.Genero = (sexo == 1) ? "Masculino" : "Femenino";
                            }
                            else
                            {
                                SesionUsuario.Genero = string.Empty;
                            }

                            // Tipo de sangre por id → lo convertimos a código (A+, O-, etc.)
                            if (dr["id_tipo_sangre"] != DBNull.Value)
                            {
                                int idTipo = Convert.ToInt32(dr["id_tipo_sangre"]);
                                SesionUsuario.TipoSangre = ObtenerCodigoTipoSangre(idTipo);
                            }
                            else
                            {
                                SesionUsuario.TipoSangre = string.Empty;
                            }

                            SesionUsuario.CI = dr["ci_paciente"]?.ToString() ?? string.Empty;

                            // Foto de perfil
                            if (dr["foto_perfil"] != DBNull.Value)
                            {
                                SesionUsuario.FotoPerfil = (byte[])dr["foto_perfil"];
                                fotoPerfilActual = SesionUsuario.FotoPerfil;
                                FotoPerfil.Source = ConvertirBytesAImagen(fotoPerfilActual);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar datos desde la base de datos: " + ex.Message,
                                "Error al cargar usuario",
                                MessageBoxButton.OK,
                                MessageBoxImage.Warning);
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            ModeloUsuario usuario = new ModeloUsuario(
                TxtUsuario.Text ?? string.Empty,
                PsBoxContrasena.Password ?? string.Empty,
                TxtTelefono.Text ?? string.Empty,
                TxtNombre.Text ?? string.Empty,
                TxtCorreo.Text ?? string.Empty,
                TxtDireccion.Text ?? string.Empty,
                DpFechaNacimiento.SelectedDate,
                (CbGenero.SelectedItem as ComboBoxItem)?.Content?.ToString() ?? string.Empty,
                (CbTipoSangre.SelectedItem as ComboBoxItem)?.Content?.ToString() ?? string.Empty,
                rutaImagenSeleccionada,
                TxtCI.Text ?? string.Empty
            );

            //Verificando si faltan datos obligatorios
            if (!ValidacionesMiCuenta.CamposCompletos(usuario))
            {
                MessageBox.Show("Debe completar todos los datos antes de salir.", "Datos incompletos", MessageBoxButton.OK, MessageBoxImage.Warning);
                return; // NO permite salir
            }

            // Volver a Servicios si todo está completo
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
                    TxtUsuario.Text ?? string.Empty,
                    PsBoxContrasena.Password ?? string.Empty,
                    TxtTelefono.Text ?? string.Empty,
                    TxtNombre.Text ?? string.Empty,
                    TxtCorreo.Text ?? string.Empty,
                    TxtDireccion.Text ?? string.Empty,
                    DpFechaNacimiento.SelectedDate,
                    (CbGenero.SelectedItem as ComboBoxItem)?.Content?.ToString() ?? string.Empty,
                    (CbTipoSangre.SelectedItem as ComboBoxItem)?.Content?.ToString() ?? string.Empty,
                    rutaImagenSeleccionada,
                    TxtCI.Text ?? string.Empty
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
                rutaImagenSeleccionada = ofd.FileName ?? string.Empty;

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
                    object? result = cmd.ExecuteScalar();

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
                        object? result = cmdGet.ExecuteScalar();
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

            if (!u.FechaNacimiento.HasValue)
                throw new ArgumentException("Debe seleccionar una fecha de nacimiento.");

            // Usar la foto actual si existe, en caso contrario la de sesión, y si no, arreglo vacío.
            byte[] bytesFoto = fotoPerfilActual.Length > 0
                ? fotoPerfilActual
                : (SesionUsuario.FotoPerfil ?? Array.Empty<byte>());

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
                            object? resTipo = cmdTipo.ExecuteScalar();

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

                        // Valores sin nulls
                        string correo = string.IsNullOrWhiteSpace(u.Correo) ? "" : u.Correo;
                        string nombre = string.IsNullOrWhiteSpace(u.Nombre) ? "" : u.Nombre;
                        string telefono = string.IsNullOrWhiteSpace(u.Telefono) ? "" : u.Telefono;
                        string direccion = string.IsNullOrWhiteSpace(u.Direccion) ? "" : u.Direccion;
                        int sexoInt = esMasculino ? 1 : 0; // 1 = Masculino, 0 = Femenino

                        if (!existePaciente)
                        {
                            // Insertar nuevo Paciente
                            using (SqlCommand cmdInsertPac = new SqlCommand(
                                @"INSERT INTO Paciente
                                  (ci_paciente, id_tipo_sangre, id_centro, correo, nombre_completo,
                                   celular, direccion, sexo, foto_perfil, fecha_nacimiento)
                                  VALUES (@ci, @idTipo, @idCentro, @correo, @nombre,
                                          @cel, @dir, @sexo, @foto, @fecha)",
                                conn, tx))
                            {
                                cmdInsertPac.Parameters.AddWithValue("@ci", ciNumerico);
                                cmdInsertPac.Parameters.AddWithValue("@idTipo", idTipo);
                                // id_centro por defecto = 1 (ajustar según tu BD)
                                cmdInsertPac.Parameters.AddWithValue("@idCentro", 1);
                                cmdInsertPac.Parameters.AddWithValue("@correo", correo);
                                cmdInsertPac.Parameters.AddWithValue("@nombre", nombre);
                                cmdInsertPac.Parameters.AddWithValue("@cel", telefono);
                                cmdInsertPac.Parameters.AddWithValue("@dir", direccion);
                                cmdInsertPac.Parameters.AddWithValue("@sexo", sexoInt);
                                cmdInsertPac.Parameters.Add("@foto", SqlDbType.VarBinary).Value = bytesFoto;
                                cmdInsertPac.Parameters.AddWithValue("@fecha", u.FechaNacimiento.Value);

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
                                      fecha_nacimiento = @fecha
                                  WHERE ci_paciente = @ci",
                                conn, tx))
                            {
                                cmdUpdPac.Parameters.AddWithValue("@ci", ciNumerico);
                                cmdUpdPac.Parameters.AddWithValue("@idTipo", idTipo);
                                cmdUpdPac.Parameters.AddWithValue("@correo", correo);
                                cmdUpdPac.Parameters.AddWithValue("@nombre", nombre);
                                cmdUpdPac.Parameters.AddWithValue("@cel", telefono);
                                cmdUpdPac.Parameters.AddWithValue("@dir", direccion);
                                cmdUpdPac.Parameters.AddWithValue("@sexo", sexoInt);
                                cmdUpdPac.Parameters.Add("@foto", SqlDbType.VarBinary).Value = bytesFoto;
                                cmdUpdPac.Parameters.AddWithValue("@fecha", u.FechaNacimiento.Value);

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

        private string ObtenerCodigoTipoSangre(int idTipo)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(_connectionString))
                using (SqlCommand cmd = new SqlCommand(
                    "SELECT codigo FROM Tipo_sangre WHERE id_tipo_sangre = @id",
                    conn))
                {
                    cmd.Parameters.AddWithValue("@id", idTipo);
                    conn.Open();
                    object? result = cmd.ExecuteScalar();
                    return result?.ToString() ?? string.Empty;
                }
            }
            catch
            {
                // Si algo falla, devolvemos vacío y ya
                return string.Empty;
            }
        }

        /// Convierte un arreglo de bytes en BitmapImage.

        private BitmapImage? ConvertirBytesAImagen(byte[] imageData)
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
