using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Front.Data__bd_;
using Front.Helpers;
using Front.INICIO; 

namespace Front.INICIO 
{
    /// <summary>
    /// Lógica de interacción para Inicio.xaml
    /// </summary>
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

            /////////USUARIO LOGIN////////////
            try
            {
                // 1. LLAMA A LA VALIDACIONES
                ValidacionesInicio validador = new ValidacionesInicio();
                validador.ValidarLogin(username, password);

                // 2. LLAMADA BASE DE DATOS
                BaseDatos db = new BaseDatos();
                bool loginExitoso = db.VerificarLogin(username, password);
                
                // 3. MANEJO DE RESULTADOS BIENVENIDA Y NO COINCIDENCIA
                if (loginExitoso) 
                {
                    string successMessageLogin = "Bienvenido: " + username;
                    MessageBox.Show(successMessageLogin, "Inicio de sesión exitoso", MessageBoxButton.OK, MessageBoxImage.Information);

                    // NAVEGAR SOLO SI EL LOGIN ES CORRECTO
                    this.NavigationService.Navigate(new Servicios());
                }
                else
                {
                    // MENSAJE SI EL USUARIO O CONTRASEÑA NO COINCIDEN EN LA DB
                    MessageBox.Show("Usuario o contraseña incorrectos.", "Acceso denegado", MessageBoxButton.OK, MessageBoxImage.Warning); 
                }
            }

            catch (RegexMatchTimeoutException obj1)
            {
                // Mensaje Abreviado para el USUARIO.
                MessageBox.Show(
                    "Error de validación: Entrada de datos excesivamente larga. Inténtelo de nuevo.\n\n" + "Código Técnico: " + obj1.Message, "Error de Rendimiento", MessageBoxButton.OK, MessageBoxImage.Warning
                );
            }
            catch (ArgumentException obj2) 
            {
                MessageBox.Show("ERROR de Validación: " + obj2.Message, "Datos Inválidos", MessageBoxButton.OK, MessageBoxImage.Warning);

            }
            catch (Exception obj3)
            {
                MessageBox.Show("ERROR DE SISTEMA CRÍTICO NO ANTICIPADO. El sistema ha fallado. Reporte el código de error: " + obj3.Message, "Fallo Crítico de Runtime", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        ///REGISTRARSE//////////////
        private void BtnRegisterAttempt_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            string username = txtUsuarioRegistro.Text.Trim();
            string phone = txtTelefonoRegistro.Text.Trim();
            string password = pbContrasenaRegistro.Password;
            string confirmPassword = pbRepetirContrasena.Password;
            int letrasReg = username.Count(char.IsLetter);
            try
            {
                // 1. llama a las VALIDACIONES
                ValidacionesInicio validador = new ValidacionesInicio();
                validador.ValidarRegistro(username, phone, password, confirmPassword);

                // 2. LLAMADA A LA CAPA DE DATOS 
                BaseDatos db = new BaseDatos();
                db.RegistrarUsuario(username, password); 

                // 3. MENSAJE DE ÉXITO Y NAVEGACIÓN 
                string successMessageRegister = "Registro exitoso. ¡Bienvenido, " + username + "!";
                MessageBox.Show(successMessageRegister, "Registro Exitoso", MessageBoxButton.OK, MessageBoxImage.Information);

                // NAVEGAR
                this.NavigationService.Navigate(new Servicios());
            }
            catch (RegexMatchTimeoutException obj4)
            {
                MessageBox.Show("Error de rendimiento en la validación de datos." + obj4.Message, "Por favor, inténtelo de nuevo.", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (ArgumentException obj5)
            {
                MessageBox.Show("ERROR de Validación: " + obj5.Message, "Datos Inválidos", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            catch (Exception obj6)
            {
                MessageBox.Show("ERROR DE SISTEMA CRÍTICO NO ANTICIPADO. El sistema ha fallado. Reporte el código de error: " + obj6.Message, "Fallo Crítico de Runtime", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}

