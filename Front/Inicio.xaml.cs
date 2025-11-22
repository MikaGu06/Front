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
            const int LONGITUD_MINIMA = 10;
            int letras = username.Count(char.IsLetter);

            // 1. Validación de campos vacíos (se usa el mensaje directo para el usuario)
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                MessageBox.Show("ERROR: Los campos Usuario y Contraseña deben ser rellenados.", "Intente nuevamente", MessageBoxButton.OK, MessageBoxImage.Stop);
                return;
            }
            /////////USUARIO LOGIN////////////
            try
            {
                // 2. Validación de Longuitud de nombre usuario 
                if (username.Length < 10 || username.Length > 15)
                {
                    throw new ArgumentException("El nombre de usuario debe tener entre 10 y 15 caracteres.");
                }
                foreach (char c in username)
                {
                    if (!char.IsLetter(c) && !char.IsDigit(c))
                    {
                        throw new ArgumentException("El nombre de usuario solo puede contener letras y números (sin caracteres especiales).");
                    }
                }

                // 2.1  validacion de Sin caracter especiales 
                if (!Regex.IsMatch(password, @"^[a-zA-Z0-9]+$"))
                {
                    throw new ArgumentException(
                        "La contraseña no puede tener espacios ni caracteres especiales.");
                }
                // 2.2 Minimo de letras Obli
                if (letras < 3)
                {
                    throw new ArgumentException("El nombre de usuario debe contener al menos 3 letras.");
                }
                // 2.3 No solo numero
                if (username.All(char.IsDigit))
                {
                    throw new ArgumentException("El nombre de usuario no puede estar compuesto solo por números.");
                }

                //////////CONTRASEÑA/////////
                //3. Validación de longitud mínima de contraseña
                if (password.Length < 10 || password.Length > 15)
                {
                    throw new ArgumentException("La contraseña debe tener entre 10 y 15 caracteres.");
                }
                //3.1 Contra sin caracter especial
                foreach (char c in password)
                {
                    if (!char.IsLetter(c) && !char.IsDigit(c))
                    {
                        throw new ArgumentException("La contraseña solo puede contener letras y números (sin caracteres especiales).");
                    }
                }
                // INICIO DE SESIÓN EXITOSA
                string successMessageLogin = "Bienvenido: " + username;
                MessageBox.Show(successMessageLogin, "Inicio de sesión exitoso", MessageBoxButton.OK, MessageBoxImage.Information);

                // NAVEGAR SOLO SI TODO ESTÁ CORRECTO
                this.NavigationService.Navigate(new Servicios());
            }
            catch (RegexMatchTimeoutException obj1)
            {
                // Mensaje Abreviado para el USUARIO.
                MessageBox.Show(
                    "Error de validación: Entrada de datos excesivamente larga. Inténtelo de nuevo.\n\n" + "Código Técnico: " + obj1.Message, "Error de Rendimiento", MessageBoxButton.OK, MessageBoxImage.Warning
                );
            }
            // Captura ArgumentException y usa su mensaje detallado para el usuario
            catch (ArgumentException obj2)
            {
                MessageBox.Show("ERROR de Validación: " + obj2.Message, "Datos Inválidos", MessageBoxButton.OK, MessageBoxImage.Warning);

            }
            // Captura cualquier otra excepción crítica
            catch (Exception obj3)
            {
                MessageBox.Show("ERROR DE SISTEMA CRÍTICO NO ANTICIPADO. El sistema ha fallado. Reporte el código de error: " + obj3.Message, "Fallo Crítico de Runtime", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnRegisterAttempt_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            string username = txtUsuarioRegistro.Text.Trim();
            string phone = txtTelefonoRegistro.Text.Trim();
            string password = pbContrasenaRegistro.Password;
            string confirmPassword = pbRepetirContrasena.Password;
            int letrasReg = username.Count(char.IsLetter);

            // 1. Validación de campos vacíos
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(phone) ||
                string.IsNullOrEmpty(password) || string.IsNullOrEmpty(confirmPassword))
            {
                MessageBox.Show("ERROR: Todos los campos del formulario de registro deben ser completados.", "Validación Requerida", MessageBoxButton.OK, MessageBoxImage.Stop);
                return;
            }

            // 2. Validación de coincidencia de contraseñas
            if (password != confirmPassword)
            {
                MessageBox.Show("ERROR: La 'Contraseña' y 'Repetir contraseña' no coinciden. Por favor, verifica ambos campos.", "Error de Contraseña", MessageBoxButton.OK, MessageBoxImage.Stop);
                return;
            }

            try
            {
                // 3. Minimos de caracteres user
                if (username.Length < 10 || username.Length > 15)
                {
                    throw new ArgumentException("El nombre de usuario debe tener entre 10 y 15 caracteres.");
                }
                // 3.1 El nombre solo puede tener letras y numeros 
                foreach (char c in username)
                {
                    if (!char.IsLetter(c) && !char.IsDigit(c))
                    {
                        throw new ArgumentException("El nombre de usuario solo puede contener letras y números.");
                    }
                }
                // 3.2 User minimo 3 letras
                if (letrasReg < 3)
                {
                    throw new ArgumentException("El nombre de usuario debe contener al menos 3 letras.");
                }
                // 3.3 User no puede tener solo numero
                if (username.All(char.IsDigit))
                {
                    throw new ArgumentException("El nombre de usuario no puede estar compuesto solo por números.");
                }

                // SIMULACIÓN: Lógica de Registro exitosa
                string successMessageRegister = "Registro exitoso. ¡Bienvenido, " + username + "!";
                MessageBox.Show(successMessageRegister, "Registro Exitoso", MessageBoxButton.OK, MessageBoxImage.Information);

                // NAVEGAR SOLO SI TODO ESTÁ CORRECTO
                this.NavigationService.Navigate(new Servicios());
            }
            catch (RegexMatchTimeoutException obj4)
            {
                MessageBox.Show("Error de rendimiento en la validación de datos." + obj4.Message, "Por favor, inténtelo de nuevo.", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            // Captura ArgumentException y usa su mensaje detallado para el usuario
            catch (ArgumentException obj5)
            {
                MessageBox.Show("ERROR de Validación: " + obj5.Message, "Datos Inválidos", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            // Captura cualquier otra excepción crítica
            catch (Exception obj6)
            {
                MessageBox.Show("ERROR DE SISTEMA CRÍTICO NO ANTICIPADO. El sistema ha fallado. Reporte el código de error: " + obj6.Message, "Fallo Crítico de Runtime", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

    }
}
