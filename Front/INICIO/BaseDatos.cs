using System;
using Front.Data__bd_;
using Front.Helpers;

namespace Front.INICIO
{
    internal class BaseDatos
    {
        ///LOGIN////////////////
        public bool VerificarLogin(string username, string password)
        {
            // Hash contraseña
            byte[] hash = PasswordHelper.HashPassword(password);
            string hashHex = PasswordHelper.HashToHex(hash);

            // Consulta SQL
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


        ///REGISTRO///////////////////
        public void RegistrarUsuario(string username, string password)
        {
            // Hash contraseña
            byte[] hash = PasswordHelper.HashPassword(password);
            string hashHex = PasswordHelper.HashToHex(hash);

            // Generar ID
            Random r = new Random();
            int nuevoID = r.Next(200, 999);

            // Insert SQL
            string insertQuery = $@"
                INSERT INTO Usuario (id_usuario, ci_paciente, nombre_usuario, contrasena_hash, estado)
                VALUES ({nuevoID}, NULL, '{username}', {hashHex}, 1);
            ";

            Database db = new Database();
            db.EjecutarComando(insertQuery);

        }
    }
}

