using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace Front.Data__bd_
{
    public class Database
    {
        private readonly string _connectionString;

        public Database()
        {
            _connectionString = ConfigurationManager
                .ConnectionStrings["cnHealthyU"].ConnectionString;
        }

        // Para probar la conexión 
        public void ProbarConexion()
        {
            using (SqlConnection cn = new SqlConnection(_connectionString))
            {
                cn.Open();
            }
        }

        // SELECT -> devuelve DataTable
        public DataTable EjecutarConsulta(string query)
        {
            DataTable dt = new DataTable();

            using (SqlConnection cn = new SqlConnection(_connectionString))
            using (SqlCommand cmd = new SqlCommand(query, cn))
            using (SqlDataAdapter da = new SqlDataAdapter(cmd))
            {
                cn.Open();
                da.Fill(dt);
            }

            return dt;
        }

        // INSERT / UPDATE / DELETE
        public int EjecutarComando(string query)
        {
            using (SqlConnection cn = new SqlConnection(_connectionString))
            using (SqlCommand cmd = new SqlCommand(query, cn))
            {
                cn.Open();
                return cmd.ExecuteNonQuery();
            }
        }
    }
}
