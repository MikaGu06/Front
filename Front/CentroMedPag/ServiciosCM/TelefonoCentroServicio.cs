using System;
using System.Collections.Generic;
using Microsoft.Data.SqlClient;
using Front.CentroMedPag.ModelosCM;
using System.Configuration;

namespace Front.CentroMedPag.ServiciosCM
{
    internal class TelefonoCentroServicio
    {
        private SqlConnection GetConnection()
        {
            return new SqlConnection(ConfigurationManager.ConnectionStrings["cnHealthyU"].ConnectionString);
        }

        public void CargarTelefonos(List<CentrosDeSalud> centros)
        {
            using (SqlConnection conn = GetConnection())
            {
                conn.Open();
                string query = "SELECT id_telefono, id_centro, telefono, es_whatsapp, link_whatsapp FROM dbo.Telefono_Centro";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        int id_centro = reader.GetInt32(1);
                        CentrosDeSalud centro = centros.Find(c => c.Id_centro == id_centro);

                        if (centro != null)
                        {
                            centro.Telefonos.Add(new TelefonosCentro(
                                reader.GetInt32(0),
                                id_centro,
                                reader.GetString(2),
                                reader.GetBoolean(3),
                                reader.IsDBNull(4) ? "" : reader.GetString(4)
                            ));
                        }
                    }
                }
            }
        }
    }
}
