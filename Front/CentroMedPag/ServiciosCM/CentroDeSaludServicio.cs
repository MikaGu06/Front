using Front.CentroMedPag.ModelosCM;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Configuration;

namespace Front.CentroMedPag.ServiciosCM
{
    internal class CentroDeSaludServicio
    {
        private SqlConnection GetConnection()
        {
            return new SqlConnection(ConfigurationManager.ConnectionStrings["cnHealthyU"].ConnectionString);
        }

        public List<CentrosDeSalud> ObtenerCentrosPorCategoria(int idCategoria)
        {
            List<CentrosDeSalud> lista = new List<CentrosDeSalud>();

            using (var con = GetConnection())
            {
                con.Open();

                string query = @"
                SELECT c.id_centro, c.id_categoria, c.institucion, c.direccion, c.gps_link,
                       t.id_telefono, t.telefono, t.es_whatsapp, t.link_whatsapp
                FROM dbo.Centro_de_Salud c
                LEFT JOIN dbo.Telefono_Centro t ON c.id_centro = t.id_centro
                WHERE c.id_categoria = @idCategoria";

                using (var cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@idCategoria", idCategoria);

                    using (var reader = cmd.ExecuteReader())
                    {
                        Dictionary<int, CentrosDeSalud> dict = new Dictionary<int, CentrosDeSalud>();

                        while (reader.Read())
                        {
                            int idCentro = reader.GetInt32(0);

                            if (!dict.ContainsKey(idCentro))
                            {
                                var cat = new CategoriaCentro(reader.GetInt32(1), ""); // nombre opcional
                                var centro = new CentrosDeSalud(
                                    idCentro,
                                    cat,
                                    reader.GetString(2),
                                    reader.GetString(3),
                                    reader.GetString(4)
                                );
                                dict[idCentro] = centro;
                            }

                            if (!reader.IsDBNull(5))
                            {
                                var tel = new TelefonosCentro(
                                    reader.GetInt32(5),
                                    idCentro,
                                    reader.GetString(6),
                                    reader.GetBoolean(7),
                                    reader.IsDBNull(8) ? "" : reader.GetString(8)
                                );
                                dict[idCentro].Telefonos.Add(tel);
                            }
                        }

                        lista.AddRange(dict.Values);
                    }
                }
            }

            return lista;
        }
    }
}
