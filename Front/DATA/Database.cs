using System;
using System.Data;
using System.Configuration;
using Microsoft.Data.SqlClient;

namespace Front.Data__bd_
{
    internal class Database
    {
        // Cadena de conexión tomada del app.config / cnHealthyU
        private readonly string _connectionString;

        public Database()
        {
            _connectionString = ConfigurationManager
                .ConnectionStrings["cnHealthyU"]
                .ConnectionString;
        }

        /// Ejecuta una consulta SELECT y devuelve un DataTable.
        public DataTable EjecutarConsulta(string query)
        {
            DataTable tabla = new DataTable();

            using (SqlConnection conn = new SqlConnection(_connectionString))
            using (SqlCommand cmd = new SqlCommand(query, conn))
            using (SqlDataAdapter da = new SqlDataAdapter(cmd))
            {
                conn.Open();
                da.Fill(tabla);
            }

            return tabla;
        }

        /// Ejecuta un comando que no devuelve filas (INSERT, UPDATE, DELETE).
        public int EjecutarComando(string query)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                conn.Open();
                int filasAfectadas = cmd.ExecuteNonQuery();
                return filasAfectadas;
            }
        }
    }
}
