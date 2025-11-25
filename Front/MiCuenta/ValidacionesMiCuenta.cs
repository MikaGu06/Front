using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Front.MiCuenta
{
    public class ValidacionesMiCuenta
    {
        public static void CuentaValidar(ModeloUsuario u)
        {
            if (string.IsNullOrWhiteSpace(u.Nombre) ||
                string.IsNullOrWhiteSpace(u.Contrasena) ||
                    string.IsNullOrWhiteSpace(u.Correo) ||
                    string.IsNullOrWhiteSpace(u.Telefono) ||
                    string.IsNullOrWhiteSpace(u.Direccion) ||
                    string.IsNullOrWhiteSpace(u.Edad) ||
                    !u.FechaNacimiento.HasValue ||
                    string.IsNullOrEmpty(u.Genero) ||
                    string.IsNullOrEmpty(u.TipoSangre))
            {
                throw new ArgumentException("Validación General: Todos los campos del formulario deben ser completados.");
            }

            // 1. VALIDACIÓN : Nombre Completo (solo formato y longitud, ya se verificó que no está vacío)
            if (u.Nombre.Length < 3 || u.Nombre.Length > 60 ||
                !Regex.IsMatch(u.Nombre, @"^[a-zA-Z\s]+$") ||
                u.Nombre.Trim().Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries).Length < 2)
            {
                throw new ArgumentException("Nombre Completo: Debe tener 2+ palabras, 3-60 caracteres y solo letras/espacios.");
            }
            if (u.Contrasena.Length < 6 || u.Contrasena.Length > 20)
            {
                throw new ArgumentException("La contraseña debe tener entre 6 y 20 caracteres.");
            }
            // 2. VALIDACIÓN: Correo Electronico
            string emailPattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";

            if (!Regex.IsMatch(u.Correo, emailPattern) || u.Correo.Length > 100)
            {
                throw new ArgumentException("Correo Electrónico: Debe cumplir formato estándar (ej: univalle@gmail.com) y no superar los 100 caracteres.");
            }

            // 3. VALIDACIÓN: Telefono
            if (!Regex.IsMatch(u.Telefono, @"^\d+$") || u.Telefono.Length < 7 || u.Telefono.Length > 15)
            {
                throw new ArgumentException("Teléfono: Solo números, entre 7 y 15 dígitos.");
            }

            // 4. VALIDACIÓN : Dirección
            if (u.Direccion.Length > 100)
            {
                throw new ArgumentException("Dirección: Debe tener como máximo 100 caracteres.");
            }

            // 5. VALIDACIÓN: Edad
            int edadIngresada = 0;
            if (!int.TryParse(u.Edad, out edadIngresada) || edadIngresada < 1 || edadIngresada > 120)
            {
                throw new ArgumentException("Edad: Solo números, rango 1 a 120 años.");
            }

            // 6. VALIDACIÓN CONCISA: Género y Tipo de Sangre 
            string[] validosGenero = { "Masculino", "Femenino" };
            if (!validosGenero.Contains(u.Genero))
            {
                throw new ArgumentException("Género: Debe seleccionar un valor válido ('Masculino' o 'Femenino').");
            }

            string[] validosSangre = { "A+", "A-", "B+", "B-", "AB+", "AB-", "O+", "O-" };
            if (!validosSangre.Contains(u.TipoSangre))
            {
                throw new ArgumentException("Tipo de Sangre: Debe seleccionar un valor válido (ej: A+, O-).");
            }

            // 7. VALIDACIÓN : Fecha de Nacimiento 
            if (u.FechaNacimiento.Value.Date >= DateTime.Today)
            {
                throw new ArgumentException("Fecha de Nacimiento: Debe ser menor a la fecha actual.");
            }

            int edadCalculada = DateTime.Today.Year - u.FechaNacimiento.Value.Year;
            if (u.FechaNacimiento.Value.Date > DateTime.Today.AddYears(-edadCalculada))
            {
                edadCalculada = edadCalculada - 1;
            }

            if (edadCalculada != edadIngresada || edadCalculada < 1 || edadCalculada > 120)
            {
                throw new ArgumentException(string.Format("Fecha de Nacimiento: La edad calculada no es coherente con la Edad ingresada .", edadCalculada, edadIngresada));
            }
        }
    }
}
