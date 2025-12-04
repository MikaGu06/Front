using Front.CentroMedPag.ModelosCM;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Configuration;

namespace Front.CentroMedPag.ServiciosCM
{
    public class CentroDeSaludServicio
    {
        private SqlConnection GetConnection()
        {
            return new SqlConnection(ConfigurationManager.ConnectionStrings["cnHealthyU"].ConnectionString);
        }

       
        public List<CentrosDeSalud> ObtenerCentrosPorCategoria(int idCategoria, int ciUsuario)
        {
            List<CentrosDeSalud> lista = new List<CentrosDeSalud>();

            using (var con = GetConnection())
            {
                con.Open();

                string query = @"
                        SELECT c.id_centro, c.id_categoria, c.institucion, c.direccion, c.gps_link,
                                t.id_telefono, t.telefono, t.es_whatsapp, t.link_whatsapp,
                                f.ci_paciente AS es_favorito  -- <-- CORREGIDO: ci_paciente
                        FROM dbo.Centro_de_Salud c
                        LEFT JOIN dbo.Telefono_Centro t ON c.id_centro = t.id_centro
                        LEFT JOIN dbo.pac_centro f ON c.id_centro = f.id_centro AND f.ci_paciente = @ciUsuario 
                        -- <-- CORREGIDO: pac_centro y f.ci_paciente
                        WHERE c.id_categoria = @idCategoria";

                using (var cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@idCategoria", idCategoria);
                    cmd.Parameters.AddWithValue("@ciUsuario", ciUsuario);

                    using (var reader = cmd.ExecuteReader())
                    {
                        Dictionary<int, CentrosDeSalud> dict = new Dictionary<int, CentrosDeSalud>();

                        while (reader.Read())
                        {
                            int idCentro = reader.GetInt32(0);

                            if (!dict.ContainsKey(idCentro))
                            {
                                var cat = new CategoriaCentro(reader.GetInt32(1), "");
                                var centro = new CentrosDeSalud(
                                    idCentro,
                                    cat,
                                    reader.GetString(2),
                                    reader.GetString(3),
                                    reader.GetString(4)
                                );

                                centro.EsFavorito = !reader.IsDBNull(9); 
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

        public bool EsFavorito(int ciUsuario, int idCentro)
        {
            using (var con = GetConnection())
            {
                con.Open();
                string query = "SELECT COUNT(*) FROM pac_centro WHERE ci_paciente = @ci AND id_centro = @id"; // <-- CORREGIDO: pac_centro y ci_paciente
                using (var cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@ci", ciUsuario);
                    cmd.Parameters.AddWithValue("@id", idCentro);
                    int count = (int)cmd.ExecuteScalar();
                    return count > 0;
                }
            }
        }

        public void AgregarFavorito(int ciUsuario, int idCentro)
        {
            using (var con = GetConnection())
            {
                con.Open();

 
                string query = "IF NOT EXISTS (SELECT 1 FROM pac_centro WHERE ci_paciente = @ci AND id_centro = @id) " +

                       "INSERT INTO pac_centro (ci_paciente, id_centro, fecha) VALUES (@ci, @id, @fecha)";

                using (var cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@ci", ciUsuario);
                    cmd.Parameters.AddWithValue("@id", idCentro);
                    cmd.Parameters.AddWithValue("@fecha", DateTime.Now);

                    cmd.ExecuteNonQuery();
                }
            }
        }

        public void QuitarFavorito(int ciUsuario, int idCentro)
        {
            using (var con = GetConnection())
            {
                con.Open();
                string query = "DELETE FROM pac_centro WHERE ci_paciente = @ci AND id_centro = @id"; // <-- CORREGIDO: pac_centro y ci_paciente
                                                                                                     // ... (resto del código)
                using (var cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@ci", ciUsuario);
                    cmd.Parameters.AddWithValue("@id", idCentro);
                    cmd.ExecuteNonQuery();
                }
            }
        }
    }
}
