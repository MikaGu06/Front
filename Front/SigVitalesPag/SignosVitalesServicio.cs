using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;

namespace Front.SigVitalesPag
{
    internal class SignosVitalesServicio
    {
        // Cadena de conexión
        private readonly string _connectionString =
            "Data Source=159.203.102.189,1433;" +
            "Initial Catalog=HealthyU;" +
            "User ID=sa;" +
            "Password=Passw0rd!;" +
            "Encrypt=False;" +
            "TrustServerCertificate=True;" +
            "Connect Timeout=15;";

        // INSERTAR SIGNO 
        public void AgregarSigno(ModeloSignosVitales signo)
        {
            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                con.Open();

                string query = @"
                    INSERT INTO Signos_vitales
                        (id_signo, ci_paciente, fecha, hora,
                         ritmo_cardiaco, presion_arterial, temperatura, oxigenacion)
                    VALUES
                        (@IdSigno, @CiPaciente, @Fecha, @Hora,
                         @RitmoCardiaco, @PresionArterial, @Temperatura, @Oxigenacion);";

                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@IdSigno", signo.IdSigno);
                    cmd.Parameters.AddWithValue("@CiPaciente", signo.CiPaciente);
                    cmd.Parameters.AddWithValue("@Fecha", signo.Fecha);
                    cmd.Parameters.AddWithValue("@Hora", signo.Hora);
                    cmd.Parameters.AddWithValue("@RitmoCardiaco", signo.RitmoCardiaco);
                    cmd.Parameters.AddWithValue("@PresionArterial", signo.PresionArterial);
                    cmd.Parameters.AddWithValue("@Temperatura", signo.Temperatura);
                    cmd.Parameters.AddWithValue("@Oxigenacion", signo.Oxigenacion);

                    cmd.ExecuteNonQuery();
                }
            }
        }

        // agregar signos pasados a la tabla pac_signos
        public void AgregarPacSigno(int ciPaciente, int idSigno)
        {
            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                con.Open();

                string query = @"
                    INSERT INTO pac_signos (ci_paciente, id_signo)
                    VALUES (@CiPaciente, @IdSigno);";

                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@CiPaciente", ciPaciente);
                    cmd.Parameters.AddWithValue("@IdSigno", idSigno);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        // NUEVO ID
        public int ObtenerNuevoId()
        {
            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                con.Open();

                string query = "SELECT ISNULL(MAX(id_signo), 0) + 1 FROM Signos_vitales";

                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    return Convert.ToInt32(cmd.ExecuteScalar());
                }
            }
        }

        // LISTAR TODO
        public List<ModeloSignosVitales> ListarSignos()
        {
            List<ModeloSignosVitales> lista = new List<ModeloSignosVitales>();

            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                con.Open();

                string query = @"
                    SELECT id_signo,
                           ci_paciente,
                           fecha,
                           hora,
                           ritmo_cardiaco,
                           presion_arterial,
                           temperatura,
                           oxigenacion
                    FROM Signos_vitales
                    ORDER BY fecha DESC, hora DESC;";

                using (SqlCommand cmd = new SqlCommand(query, con))
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var signo = new ModeloSignosVitales
                        {
                            IdSigno = Convert.ToInt32(reader["id_signo"]),

                            CiPaciente = reader["ci_paciente"] == DBNull.Value
                                ? 0
                                : Convert.ToInt32(reader["ci_paciente"]),

                            Fecha = Convert.ToDateTime(reader["fecha"]),

                            Hora = reader["hora"] is TimeSpan ts
                                ? ts
                                : TimeSpan.Parse(reader["hora"].ToString()),

                            RitmoCardiaco = Convert.ToInt32(reader["ritmo_cardiaco"]),
                            PresionArterial = Convert.ToInt32(reader["presion_arterial"]),
                            Temperatura = Convert.ToDecimal(reader["temperatura"]),
                            Oxigenacion = Convert.ToInt32(reader["oxigenacion"])
                        };

                        lista.Add(signo);
                    }
                }
            }

            return lista;
        }

        // LISTAR SOLO DEL PACIENTE LOGUEADO 
        public List<ModeloSignosVitales> ListarSignosDePaciente(int ciPaciente)
        {
            List<ModeloSignosVitales> lista = new List<ModeloSignosVitales>();

            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                con.Open();

                string query = @"
                    SELECT id_signo,
                           ci_paciente,
                           fecha,
                           hora,
                           ritmo_cardiaco,
                           presion_arterial,
                           temperatura,
                           oxigenacion
                    FROM Signos_vitales
                    WHERE ci_paciente = @ci
                    ORDER BY fecha DESC, hora DESC;";

                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@ci", ciPaciente);

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var signo = new ModeloSignosVitales
                            {
                                IdSigno = Convert.ToInt32(reader["id_signo"]),
                                CiPaciente = Convert.ToInt32(reader["ci_paciente"]),
                                Fecha = Convert.ToDateTime(reader["fecha"]),
                                Hora = reader["hora"] is TimeSpan ts
                                    ? ts
                                    : TimeSpan.Parse(reader["hora"].ToString()),
                                RitmoCardiaco = Convert.ToInt32(reader["ritmo_cardiaco"]),
                                PresionArterial = Convert.ToInt32(reader["presion_arterial"]),
                                Temperatura = Convert.ToDecimal(reader["temperatura"]),
                                Oxigenacion = Convert.ToInt32(reader["oxigenacion"])
                            };

                            lista.Add(signo);
                        }
                    }
                }
            }

            return lista;
        }
    }
}
