using System;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Front.INICIO
{
    public class ValidacionesInicio
    {
        public void ValidarLogin(string username, string password)
        {
            }


        public void ValidarRegistro(string username, string phone, string password, string confirmPassword)
        {
            int letrasReg = username.Count(char.IsLetter);
            bool mayus = false, num = false, puntoGuion = false;

            // Campos vacíos
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(phone) ||
        string.IsNullOrEmpty(password) || string.IsNullOrEmpty(confirmPassword))
                throw new ArgumentException("ERROR: Todos los campos del formulario deben ser completados.");
            // Teléfono
            if (!Regex.IsMatch(phone, @"^\d+$") || phone.Length < 7 || phone.Length > 15)
            {
                throw new ArgumentException("Teléfono: solo números, 7-15 dígitos.");
            }

            // Coincidencia de contraseñas
            if (password != confirmPassword)
                throw new ArgumentException("ERROR: Las contraseñas no coinciden.");

            // Longitud contraseña
            if (password.Length < 6 || password.Length > 20)
                throw new ArgumentException("La contraseña debe tener entre 6 y 20 caracteres.");

            // Validación de caracteres
            foreach (char c in password)
            {
                if (!char.IsLetter(c) && !char.IsDigit(c) && c != '.' && c != '_')
                    throw new ArgumentException("La contraseña solo puede tener letras, números, . o _. ");

                if (char.IsUpper(c)) mayus = true;
                if (char.IsDigit(c)) num = true;
                if (c == '.' || c == '_') puntoGuion = true;
            }

            if (!mayus)
                throw new ArgumentException("LA CONTRASEÑA debe contener al menos una mayúscula.");

            if (!num)
                throw new ArgumentException("LA CONTRASEÑA debe contener al menos un número.");

            if (!puntoGuion)
                throw new ArgumentException("LA CONTRASEÑA debe contener al menos un punto (.) o guion bajo (_).");

            // Longitud usuario
            if (username.Length < 3 || username.Length > 20)
                throw new ArgumentException("El nombre de usuario debe tener entre 3 y 20 caracteres.");

            // Usuario solo letras y números
            foreach (char c in username)
                if (!char.IsLetter(c) && !char.IsDigit(c))
                    throw new ArgumentException("El nombre de usuario solo puede contener letras y números.");

            // Usuario mínimo 3 letras
            if (letrasReg < 3)
                throw new ArgumentException("El nombre de usuario debe contener al menos 3 letras.");

            // Usuario no puede ser solo números
            if (username.All(char.IsDigit))
                throw new ArgumentException("El nombre de usuario no puede estar compuesto solo por números.");
        }
    }
}