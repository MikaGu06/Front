using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Configuration;

namespace Front.SigVitalesPag
{
    internal class SignosVitalesServicio
    {
        // Usa la misma cadena que el resto del proyecto (app.config)
        private readonly string connectionString =
            ConfigurationManager.ConnectionStrings["cnHealthyU"].ConnectionString;

        // Verificar si existe un Paciente con ese CI
        private bool ExistePaciente(int ciPaciente)
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();
                string query = "SELECT COUNT(*) FROM Paciente WHERE ci_paciente = @ci";

                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@ci", ciPaciente);
                    int count = Convert.ToInt32(cmd.ExecuteScalar());
                    return count > 0;
                }
            }
        }

        // Agregar signo vital (Signos_vitales) respetando FK FK_Signos_Paciente
        public void AgregarSigno(ModeloSignosVitales signo)
        {
            // Si el CI no existe en Paciente, lanzamos error controlado
            if (!ExistePaciente(signo.CiPaciente))
                throw new InvalidOperationException(
                    "El CI del paciente no existe en la tabla Paciente. " +
                    "Primero completa tus datos en 'Mi Cuenta' para crear el registro de Paciente.");

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();
                string query = @"INSERT INTO Signos_vitales
                                (id_signo, ci_paciente, fecha, hora,
                                 ritmo_cardiaco, presion_arterial, temperatura, oxigenacion)
                                VALUES
                                (@IdSigno, @CiPaciente, @Fecha, @Hora,
                                 @RitmoCardiaco, @PresionArterial, @Temperatura, @Oxigenacion)";

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

        // SOLO si realmente tienes la tabla pac_signos en tu BD
        public void AgregarPacSigno(int ciPaciente, int idSigno)
        {
            if (!ExistePaciente(ciPaciente))
                throw new InvalidOperationException(
                    "El CI del paciente no existe en la tabla Paciente. " +
                    "Primero completa tus datos en 'Mi Cuenta' para crear el registro de Paciente.");

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();
                string query = @"INSERT INTO pac_signos (ci_paciente, id_signo)
                                 VALUES (@CiPaciente, @IdSigno)";

                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@CiPaciente", ciPaciente);
                    cmd.Parameters.AddWithValue("@IdSigno", idSigno);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        // Nuevo id_signo incremental
        public int ObtenerNuevoId()
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();
                string query = "SELECT ISNULL(MAX(id_signo), 0) + 1 FROM Signos_vitales";

                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    return Convert.ToInt32(cmd.ExecuteScalar());
                }
            }
        }

        // Listar signos de un paciente concreto
        public List<ModeloSignosVitales> ListarSignosDePaciente(int ciPaciente)
        {
            List<ModeloSignosVitales> lista = new List<ModeloSignosVitales>();

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();
                string query = @"SELECT id_signo, ci_paciente, fecha, hora,
                                        ritmo_cardiaco, presion_arterial, temperatura, oxigenacion
                                 FROM Signos_vitales
                                 WHERE ci_paciente = @ci
                                 ORDER BY fecha DESC, hora DESC";

                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@ci", ciPaciente);

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var signo = new ModeloSignosVitales
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
            }

            return lista;
        }

        // Versión antigua: todos los signos (si la necesitas en otro lado)
        public List<ModeloSignosVitales> ListarSignos()
        {
            List<ModeloSignosVitales> lista = new List<ModeloSignosVitales>();

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();
                string query = @"SELECT id_signo, ci_paciente, fecha, hora,
                                        ritmo_cardiaco, presion_arterial, temperatura, oxigenacion
                                 FROM Signos_vitales";

                using (SqlCommand cmd = new SqlCommand(query, con))
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var signo = new ModeloSignosVitales
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
