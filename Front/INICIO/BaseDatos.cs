using System;
using System.Data;
using Front.Data__bd_;
using Front.Helpers;

namespace Front.INICIO
{
    internal class BaseDatos
    {
        /// <summary>
        /// Valida usuario y contraseña en la tabla Usuario.
        /// </summary>
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

        /// <summary>
        /// Devuelve el CI del paciente vinculado al usuario, o null.
        /// </summary>
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

        /// <summary>
        /// Crea un registro básico en Paciente y devuelve su CI.
        /// </summary>
        private int CrearPacienteBasico(string username, string phone)
        {
            Database db = new Database();

            // Siguiente CI
            string sqlNuevoCi = "SELECT ISNULL(MAX(ci_paciente), 0) + 1 FROM Paciente;";
            DataTable tablaCi = db.EjecutarConsulta(sqlNuevoCi);
            int nuevoCi = Convert.ToInt32(tablaCi.Rows[0][0]);

            string correoTemporal = username + "@temp.com";

            string insertPaciente = $@"
                INSERT INTO Paciente
                    (ci_paciente, id_tipo_sangre, id_centro, correo, nombre_completo, celular,
                     direccion, sexo, foto_perfil, fecha_nacimiento)
                VALUES
                    ({nuevoCi}, 1, NULL, '{correoTemporal}', '{username}', '{phone}',
                     NULL, NULL, NULL, GETDATE());
            ";

            db.EjecutarComando(insertPaciente);

            return nuevoCi;
        }

        /// <summary>
        /// Registra un nuevo Usuario y crea su Paciente vinculado.
        /// </summary>
        public void RegistrarUsuario(string username, string phone, string password)
        {
            // Generar hash de la contraseña
            byte[] hash = PasswordHelper.HashPassword(password);
            string hashHex = PasswordHelper.HashToHex(hash);

            // ID de usuario simple (para demo)
            Random r = new Random();
            int nuevoID = r.Next(200, 999);

            // Crear Paciente básico
            int ciPaciente = CrearPacienteBasico(username, phone);

            // Insertar Usuario
            string insertQuery = $@"
                INSERT INTO Usuario (id_usuario, ci_paciente, nombre_usuario, contrasena_hash, estado)
                VALUES ({nuevoID}, {ciPaciente}, '{username}', {hashHex}, 1);
            ";

            Database db = new Database();
            db.EjecutarComando(insertQuery);
        }
    }
}
