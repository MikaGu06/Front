using Front.Data__bd_;
using Front.Helpers;
using Front.INICIO;
using Front.MiCuenta;
using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Front.INICIO
{
    /// Lógica de interacción para Inicio.xaml
    public partial class Inicio : Page
    {
        public Inicio()
        {
            InitializeComponent();
        }

        private void Acceder_Click(object sender, MouseButtonEventArgs e)
        {
            SeccionLogin.BringIntoView();
        }

        private void IniciarSesion_Click(object sender, MouseButtonEventArgs e)
        {
            SeccionLogin.BringIntoView();
        }

        private void BtnLoginAttempt_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            string username = txtUsuarioInicioSesion.Text.Trim();
            string password = pbContrasenaInicioSesion.Password;
            int letras = username.Count(char.IsLetter);

            try
            {
                // 1. VALIDAR FORMATO
                ValidacionesInicio validador = new ValidacionesInicio();
                validador.ValidarLogin(username, password);

                // 2. VALIDAR EN BD
                BaseDatos db = new BaseDatos();
                bool loginExitoso = db.VerificarLogin(username, password);

                if (loginExitoso)
                {
                    string successMessageLogin = "Bienvenido: " + username;
                    MessageBox.Show(successMessageLogin, "Inicio de sesión exitoso",
                                    MessageBoxButton.OK, MessageBoxImage.Information);

                    // SESIÓN: guardar datos básicos
                    SesionUsuario.NombreUsuario = username;
                    SesionUsuario.Contrasena = password;

                    // 3. REVISAR SI TIENE PACIENTE ASOCIADO
                    int? ciPaciente = db.ObtenerCiPaciente(username);

                    if (ciPaciente.HasValue)
                    {
                        // Tiene Paciente -> puede ir directo a Servicios
                        this.NavigationService.Navigate(new Front.MiCuenta.MiCuenta());
                    }
                    else
                    {
                        // NO tiene Paciente -> lo obligamos a completar MiCuenta
                        MessageBox.Show("Completa tus datos de cuenta para continuar.",
                                        "Perfil incompleto",
                                        MessageBoxButton.OK,
                                        MessageBoxImage.Information);

                        this.NavigationService.Navigate(new Front.MiCuenta.MiCuenta());
                    }
                }
                else
                {
                    MessageBox.Show("Usuario o contraseña incorrectos.",
                                    "Acceso denegado",
                                    MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
            catch (RegexMatchTimeoutException obj1)
            {
                MessageBox.Show(
                    "Error de validación: Entrada de datos excesivamente larga. Inténtelo de nuevo.\n\n" +
                    "Código Técnico: " + obj1.Message,
                    "Error de Rendimiento",
                    MessageBoxButton.OK, MessageBoxImage.Warning
                );
            }
            catch (ArgumentException obj2)
            {
                MessageBox.Show("ERROR de Validación: " + obj2.Message,
                                "Datos Inválidos",
                                MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            catch (Exception obj3)
            {
                MessageBox.Show("ERROR DE SISTEMA CRÍTICO NO ANTICIPADO. El sistema ha fallado. " +
                                "Reporte el código de error: " + obj3.Message,
                                "Fallo Crítico de Runtime",
                                MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// REGISTRO 
        private void BtnRegisterAttempt_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            string username = txtUsuarioRegistro.Text.Trim();
            string phone = txtTelefonoRegistro.Text.Trim();
            string password = pbContrasenaRegistro.Password;
            string confirmPassword = pbRepetirContrasena.Password;
            int letrasReg = username.Count(char.IsLetter);

            try
            {
                // 1. VALIDACIONES
                ValidacionesInicio validador = new ValidacionesInicio();
                validador.ValidarRegistro(username, phone, password, confirmPassword);

                // 2. CREAR Usuario (ya no crea Paciente básico)
                BaseDatos db = new BaseDatos();
                db.RegistrarUsuario(username, phone, password);

                // 3. SESIÓN
                SesionUsuario.NombreUsuario = username;
                SesionUsuario.Telefono = phone;
                SesionUsuario.Contrasena = password;

                // 4. MENSAJE Y NAVEGAR DIRECTO A MiCuenta (para completar todo)
                string successMessageRegister = "Registro exitoso. ¡Bienvenido, " + username + "!";
                MessageBox.Show(successMessageRegister, "Registro Exitoso",
                                MessageBoxButton.OK, MessageBoxImage.Information);

                // Cuenta nueva: solo queremos usuario/teléfono/contraseña rellenados
                this.NavigationService.Navigate(new Front.MiCuenta.MiCuenta(esRegistroNuevo: true));
            }
            catch (RegexMatchTimeoutException obj4)
            {
                MessageBox.Show("Error de rendimiento en la validación de datos." + obj4.Message,
                                "Por favor, inténtelo de nuevo.",
                                MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (ArgumentException obj5)
            {
                MessageBox.Show("ERROR de Validación: " + obj5.Message,
                                "Datos Inválidos",
                                MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            catch (Exception obj6)
            {
                MessageBox.Show("ERROR DE SISTEMA CRÍTICO NO ANTICIPADO. El sistema ha fallado. " +
                                "Reporte el código de error: " + obj6.Message,
                                "Fallo Crítico de Runtime",
                                MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
