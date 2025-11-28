using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace Front.MiCuenta
{
    public class ValidacionesMiCuenta
    {
        /// <summary>
        /// Valida todos los campos de MiCuenta y lanza excepciones con mensajes claros.
        /// </summary>
        public static void CuentaValidar(ModeloUsuario u)
        {
            // Campos obligatorios
            if (string.IsNullOrWhiteSpace(u.Nombre) ||
                string.IsNullOrWhiteSpace(u.Contrasena) ||
                string.IsNullOrWhiteSpace(u.Correo) ||
                string.IsNullOrWhiteSpace(u.Telefono) ||
                string.IsNullOrWhiteSpace(u.Direccion) ||
                string.IsNullOrWhiteSpace(u.Edad) ||
                string.IsNullOrWhiteSpace(u.CI) ||
                !u.FechaNacimiento.HasValue ||
                string.IsNullOrEmpty(u.Genero) ||
                string.IsNullOrEmpty(u.TipoSangre))
            {
                throw new ArgumentException("Todos los campos deben estar completos.");
            }

            // Nombre completo
            if (u.Nombre.Length < 3 || u.Nombre.Length > 60 ||
                !Regex.IsMatch(u.Nombre, @"^[a-zA-Z\s]+$") ||
                u.Nombre.Trim().Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries).Length < 2)
            {
                throw new ArgumentException("Nombre: 2+ palabras, 3-60 caracteres y solo letras/espacios.");
            }

            // Contraseña
            if (u.Contrasena.Length < 6 || u.Contrasena.Length > 20)
            {
                throw new ArgumentException("La contraseña debe tener entre 6 y 20 caracteres.");
            }

            // Correo
            string emailPattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
            if (!Regex.IsMatch(u.Correo, emailPattern) || u.Correo.Length > 100)
            {
                throw new ArgumentException("Correo: formato inválido o más de 100 caracteres.");
            }

            // Teléfono
            if (!Regex.IsMatch(u.Telefono, @"^\d+$") || u.Telefono.Length < 7 || u.Telefono.Length > 15)
            {
                throw new ArgumentException("Teléfono: solo números, 7-15 dígitos.");
            }

            // Dirección
            if (u.Direccion.Length > 100)
            {
                throw new ArgumentException("Dirección: máximo 100 caracteres.");
            }

            // Edad
            if (!int.TryParse(u.Edad, out int edadIngresada) || edadIngresada < 1 || edadIngresada > 120)
            {
                throw new ArgumentException("Edad: solo números, entre 1 y 120.");
            }

            // CI
            if (!Regex.IsMatch(u.CI, @"^\d+$"))
            {
                throw new ArgumentException("CI: solo números.");
            }
            if (u.CI.Length < 5 || u.CI.Length > 15)
            {
                throw new ArgumentException("CI: entre 5 y 15 dígitos.");
            }

            // Género
            string[] validosGenero = { "Masculino", "Femenino" };
            if (!validosGenero.Contains(u.Genero))
            {
                throw new ArgumentException("Género: debe ser Masculino o Femenino.");
            }

            // Tipo de sangre
            string[] validosSangre = { "A+", "A-", "B+", "B-", "AB+", "AB-", "O+", "O-" };
            if (!validosSangre.Contains(u.TipoSangre))
            {
                throw new ArgumentException("Tipo de sangre inválido (ej: A+, O-).");
            }

            // Fecha de nacimiento y coherencia con edad
            if (u.FechaNacimiento.Value.Date >= DateTime.Today)
            {
                throw new ArgumentException("Fecha de nacimiento debe ser menor a hoy.");
            }

            int edadCalculada = DateTime.Today.Year - u.FechaNacimiento.Value.Year;
            if (u.FechaNacimiento.Value.Date > DateTime.Today.AddYears(-edadCalculada))
            {
                edadCalculada--;
            }

            if (edadCalculada != edadIngresada || edadCalculada < 1 || edadCalculada > 120)
            {
                throw new ArgumentException(
                    $"La edad calculada ({edadCalculada}) no coincide con la ingresada ({edadIngresada}).");
            }
        }
    }
}
