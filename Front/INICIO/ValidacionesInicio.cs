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
using Front.Data__bd_;
using Front.Helpers;

namespace Front.INICIO
{
    internal class ValidacionesInicio
    {
        public void ValidarLogin(string username, string password)
        {
            int letras = username.Count(char.IsLetter);
            bool mayus = false, num = false, puntoGuion = false;

            // Campos vacíos
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
                throw new Exception("ERROR: Los campos Usuario y Contraseña deben ser rellenados.");

            // Longitud de usuario
            if (username.Length < 3 || username.Length > 20)
                throw new Exception("El nombre de usuario debe tener entre 3 y 20 caracteres.");

            // Usuario solo letras o números
            foreach (char c in username)
                if (!char.IsLetter(c) && !char.IsDigit(c))
                    throw new Exception("El nombre de usuario solo puede contener letras y números.");

            // Usuario debe tener al menos 3 letras
            if (letras < 3)
                throw new Exception("El nombre de usuario debe contener al menos 3 letras.");

            // Usuario no puede ser solo número
            if (username.All(char.IsDigit))
                throw new Exception("El nombre de usuario no puede estar compuesto solo por números.");

            // Longitud contraseña
            if (password.Length < 6 || password.Length > 20)
                throw new Exception("La contraseña debe tener entre 6 y 20 caracteres.");

            // Validaciones de caracteres de contraseña
            foreach (char c in password)
            {
                if (!char.IsLetter(c) && !char.IsDigit(c) && c != '.' && c != '_')
                    throw new Exception("La contraseña solo puede tener letras, números, . o _. ");

                if (char.IsUpper(c)) mayus = true;
                if (char.IsDigit(c)) num = true;
                if (c == '.' || c == '_') puntoGuion = true;
            }

            if (!mayus)
                throw new Exception("Debe contener al menos una mayúscula.");

            if (!num)
                throw new Exception("Debe contener al menos un número.");

            if (!puntoGuion)
                throw new Exception("Debe contener al menos un punto (.) o guion bajo (_).");
        }


        public void ValidarRegistro(string username, string phone, string password, string confirmPassword)
        {
            int letrasReg = username.Count(char.IsLetter);
            bool mayus = false, num = false, puntoGuion = false;

            // Campos vacíos
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(phone) ||
                string.IsNullOrEmpty(password) || string.IsNullOrEmpty(confirmPassword))
                throw new Exception("ERROR: Todos los campos del formulario deben ser completados.");

            // Coincidencia de contraseñas
            if (password != confirmPassword)
                throw new Exception("ERROR: Las contraseñas no coinciden.");

            // Longitud contraseña
            if (password.Length < 6 || password.Length > 20)
                throw new Exception("La contraseña debe tener entre 6 y 20 caracteres.");

            // Validación de caracteres
            foreach (char c in password)
            {
                if (!char.IsLetter(c) && !char.IsDigit(c) && c != '.' && c != '_')
                    throw new Exception("La contraseña solo puede tener letras, números, . o _. ");

                if (char.IsUpper(c)) mayus = true;
                if (char.IsDigit(c)) num = true;
                if (c == '.' || c == '_') puntoGuion = true;
            }

            if (!mayus)
                throw new Exception("LA CONTRASEÑA debe contener al menos una mayúscula.");

            if (!num)
                throw new Exception("LA CONTRASEÑA debe contener al menos un número.");

            if (!puntoGuion)
                throw new Exception("LA CONTRASEÑA debe contener al menos un punto (.) o guion bajo (_).");

            // Longitud usuario
            if (username.Length < 3 || username.Length > 20)
                throw new Exception("El nombre de usuario debe tener entre 3 y 20 caracteres.");

            // Usuario solo letras y números
            foreach (char c in username)
                if (!char.IsLetter(c) && !char.IsDigit(c))
                    throw new Exception("El nombre de usuario solo puede contener letras y números.");

            // Usuario mínimo 3 letras
            if (letrasReg < 3)
                throw new Exception("El nombre de usuario debe contener al menos 3 letras.");

            // Usuario no puede ser solo números
            if (username.All(char.IsDigit))
                throw new Exception("El nombre de usuario no puede estar compuesto solo por números.");
        }
    }
}


