using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;

namespace Front.SigVitalesPag
{
    internal class SignosVitalesServicio
    {
        // Cambia esto por tu cadena de conexión real
        private readonly string connectionString = "Data Source=159.203.102.189,1433;Initial Catalog = HealthyU; User ID = sa; Password=Passw0rd!; Encrypt=False; TrustServerCertificate=True; Connect Timeout = 15;";

        // Método para agregar un signo vital
        public void AgregarSigno(ModeloSignosVitales signo)
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();
                string query = @"INSERT INTO Signos_vitales
                                (id_signo, ci_paciente, fecha, hora, ritmo_cardiaco, presion_arterial, temperatura, oxigenacion)
                                VALUES
                                (@IdSigno, @CiPaciente, @Fecha, @Hora, @RitmoCardiaco, @PresionArterial, @Temperatura, @Oxigenacion)";

                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@IdSigno", signo.IdSigno);
                    cmd.Parameters.AddWithValue("@CiPaciente", signo.CiPaciente); // Temporalmente 0 si no hay paciente
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

        // Método para insertar en la tabla intermedia pac_signos
        public void AgregarPacSigno(int ciPaciente, int idSigno)
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();
                string query = @"INSERT INTO pac_signos (ci_paciente, id_signo)
                                 VALUES (@CiPaciente, @IdSigno)";

                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@CiPaciente", ciPaciente); // Temporalmente 0 si no hay paciente
                    cmd.Parameters.AddWithValue("@IdSigno", idSigno);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        // Obtener un nuevo id_signo incremental
        public int ObtenerNuevoId()
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();
                string query = "SELECT ISNULL(MAX(id_signo), 0) + 1 FROM Signos_vitales";
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    return (int)cmd.ExecuteScalar();
                }
            }
        }

        // Listar todos los signos vitales
        public List<ModeloSignosVitales> ListarSignos()
        {
            List<ModeloSignosVitales> lista = new List<ModeloSignosVitales>();

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();
                string query = "SELECT id_signo, ci_paciente, fecha, hora, ritmo_cardiaco, presion_arterial, temperatura, oxigenacion FROM Signos_vitales";

                using (SqlCommand cmd = new SqlCommand(query, con))
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        ModeloSignosVitales signo = new ModeloSignosVitales
                        {
                            IdSigno = reader.GetInt32(0),
                            CiPaciente = reader.GetInt32(1),
                            Fecha = reader.GetDateTime(2),
                            Hora = reader.GetTimeSpan(3),
                            RitmoCardiaco = reader.GetInt32(4),
                            PresionArterial = reader.GetInt32(5),
                            Temperatura = reader.GetDecimal(6),
                            Oxigenacion = reader.GetInt32(7)
                        };

                        lista.Add(signo);
                    }
                }
            }

            return lista;
        }
    }
}
