using System;
using System.Collections.Generic;
using Microsoft.Data.SqlClient;
using Front.CentroMedPag.ModelosCM;
using System.Configuration;

namespace Front.CentroMedPag.ServiciosCM
{
    internal class CategoriaCentroServicio
    {
        private SqlConnection GetConnection()
        {
            return new SqlConnection(ConfigurationManager.ConnectionStrings["cnHealthyU"].ConnectionString);
        }

        public List<CategoriaCentro> ObtenerCategorias()
        {
            List<CategoriaCentro> categorias = new List<CategoriaCentro>();

            using (SqlConnection conn = GetConnection())
            {
                conn.Open();
                string query = "SELECT id_categoria, nombre FROM dbo.Categoria_Centro";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        categorias.Add(new CategoriaCentro(
                            reader.GetInt32(0),
                            reader.GetString(1)
                        ));
                    }
                }
            }

            return categorias;
        }
    }
}
