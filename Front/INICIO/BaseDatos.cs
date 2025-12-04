using System;
using Front.Data__bd_;
using Front.Helpers;

namespace Front.INICIO
{
    internal class BaseDatos
    {
        /// Valida usuario y contraseña en la tabla Usuario.
        public bool VerificarLogin(string username, string password)
        {
            // Generar hash de la contraseña
            byte[] hash = PasswordHelper.HashPassword(password);
            string hashHex = PasswordHelper.HashToHex(hash);

            // Consulta de validación
            string query = $@"
                SELECT COUNT(*)
                FROM Usuario
                WHERE nombre_usuario = '{username}'
                  AND contrasena_hash = {hashHex};
            ";

            Database db = new Database();
            var tabla = db.EjecutarConsulta(query);
            int count = Convert.ToInt32(tabla.Rows[0][0]);

            return count == 1;
        }

        /// Devuelve el CI del paciente vinculado al usuario, o null.
        public int? ObtenerCiPaciente(string username)
        {
            string query = $@"
                SELECT ci_paciente
                FROM Usuario
                WHERE nombre_usuario = '{username}';
            ";

            Database db = new Database();
            var tabla = db.EjecutarConsulta(query);

            if (tabla.Rows.Count == 0)
                return null;

            object valor = tabla.Rows[0][0];
            if (valor == null || valor == DBNull.Value)
                return null;

            return Convert.ToInt32(valor);
        }

        /// Registra un nuevo Usuario SIN crear Paciente temporal.
        public void RegistrarUsuario(string username, string phone, string password)
        {
            // Generar hash de la contraseña
            byte[] hash = PasswordHelper.HashPassword(password);
            string hashHex = PasswordHelper.HashToHex(hash);

            // ID de usuario simple (para demo)
            Random r = new Random();
            int nuevoID = r.Next(200, 999);

            // Insertar solo Usuario, ci_paciente queda NULL hasta que complete MiCuenta
            string insertQuery = $@"
                INSERT INTO Usuario (id_usuario, ci_paciente, nombre_usuario, contrasena_hash, estado)
                VALUES ({nuevoID}, NULL, '{username}', {hashHex}, 1);
            ";

            Database db = new Database();
            db.EjecutarComando(insertQuery);
        }
    }
}
