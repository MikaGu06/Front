using System;

namespace Front.Helpers
{
    public static class SesionUsuario
    {
        // Datos básicos de login
        public static string NombreUsuario { get; set; }
        public static string Contrasena { get; set; }

        // Datos de Mi Cuenta
        public static string Telefono { get; set; }
        public static string NombreCompleto { get; set; }
        public static string Correo { get; set; }
        public static string Direccion { get; set; }
        public static string Edad { get; set; }
        public static DateTime? FechaNacimiento { get; set; }
        public static string Genero { get; set; }
        public static string TipoSangre { get; set; }

        // 👉 ESTA ES LA QUE FALTABA
        public static string CI { get; set; }

        // Foto en memoria mientras dure la sesión
        public static byte[] FotoPerfil { get; set; }

        // (Opcional) si quieres guardar también el vínculo:
        // public static int? CiPaciente { get; set; }
    }
}
