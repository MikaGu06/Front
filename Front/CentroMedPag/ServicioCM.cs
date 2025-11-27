using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;

namespace Front.CentroMedPag
{
    public class ServicioCM
    {
        private List<ModeloCM> centros = new List<ModeloCM>();

        public List<ModeloCM> ListarCentros() => centros;

        private SqlConnection GetConnection()
        {
            return new SqlConnection(ConfigurationManager.ConnectionStrings["cnHealthyU"].ConnectionString);
        }

        public void CargarCentrosPorCategoria(int idCategoria)
        {
            centros.Clear();

            using (var con = GetConnection())
            {
                con.Open();
                string query = @"
                    SELECT 
                        c.id_centro,
                        c.id_categoria,
                        c.institucion,
                        c.direccion,
                        t.telefono,
                        c.gps_link
                    FROM Centro_de_Salud c
                    LEFT JOIN Telefono_Centro t
                        ON c.id_centro = t.id_centro
                    WHERE c.id_categoria = @idCategoria";

                using (var cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@idCategoria", idCategoria);

                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            string telefono = reader.IsDBNull(4) ? "" : reader.GetString(4);
                            Uri link = reader.IsDBNull(5) ? null : new Uri(reader.GetString(5));

                            centros.Add(new ModeloCM(
                                reader.GetInt32(0), // id_centro
                                reader.GetInt32(1) switch
                                {
                                    1 => "Farmacias",
                                    2 => "Hospitales",
                                    3 => "Clínicas",
                                    4 => "Laboratorios",
                                    5 => "Consultorios",
                                    _ => "Otros"
                                },
                                reader.GetString(2), // institucion
                                reader.GetString(3), // direccion
                                telefono,            // AHORA CORRECTO
                                link
                            ));
                        }
                    }
                }
            }
        }

        public int ObtenerNuevoId()
        {
            if (centros.Count == 0) return 1;
            return centros[centros.Count - 1].Id_centro + 1;
        }

        public void AgregarCentro(ModeloCM centro)
        {
            using (var con = GetConnection())
            {
                con.Open();

                // 1) Insertar centro
                string insertCentro = @"
                    INSERT INTO Centro_de_Salud 
                    (id_centro, id_categoria, institucion, direccion, gps_link)
                    VALUES (@id, @categoria, @institucion, @direccion, @gps)";

                using (var cmd = new SqlCommand(insertCentro, con))
                {
                    cmd.Parameters.AddWithValue("@id", centro.Id_centro);
                    cmd.Parameters.AddWithValue("@categoria", centro.Categoria switch
                    {
                        "Farmacias" => 1,
                        "Hospitales" => 2,
                        "Clínicas" => 3,
                        "Laboratorios" => 4,
                        "Consultorios" => 5,
                        _ => 0
                    });
                    cmd.Parameters.AddWithValue("@institucion", centro.Institucion);
                    cmd.Parameters.AddWithValue("@direccion", centro.Direccion);
                    cmd.Parameters.AddWithValue("@gps", centro.Link?.ToString() ?? "");
                    cmd.ExecuteNonQuery();
                }

                // 2) Insertar teléfono en la tabla Telefono_Centro
                if (!string.IsNullOrWhiteSpace(centro.Telefono))
                {
                    string insertTelefono = @"
                        INSERT INTO Telefono_Centro (id_centro, telefono, es_whatsapp)
                        VALUES (@idCentro, @telefono, 0)";

                    using (var cmdTel = new SqlCommand(insertTelefono, con))
                    {
                        cmdTel.Parameters.AddWithValue("@idCentro", centro.Id_centro);
                        cmdTel.Parameters.AddWithValue("@telefono", centro.Telefono);
                        cmdTel.ExecuteNonQuery();
                    }
                }
            }

            centros.Add(centro);
        }

        public ModeloCM ObtenerCentroPorNombre(string nombre)
        {
            foreach (var c in centros)
                if (c.Institucion.Equals(nombre, StringComparison.OrdinalIgnoreCase))
                    return c;
            return null;
        }
    }
}
