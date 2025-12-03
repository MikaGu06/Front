using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;

namespace Front.SigVitalesPag
{
    internal class SignosVitalesServicio
    {
        private readonly string _connectionString =
            "Data Source=159.203.102.189,1433;" +
            "Initial Catalog=HealthyU;" +
            "User ID=sa;" +
            "Password=Passw0rd!;" +
            "Encrypt=False;" +
            "TrustServerCertificate=True;" +
            "Connect Timeout=15;";

        // Obtener nuevo Id
        public int ObtenerNuevoId()
        {
            using var con = new SqlConnection(_connectionString);
            con.Open();

            string query = "SELECT ISNULL(MAX(id_signo),0) + 1 FROM Signos_vitales";
            using var cmd = new SqlCommand(query, con);
            return Convert.ToInt32(cmd.ExecuteScalar());
        }

        // Guardar signo y registrar en tabla intermedia
        public void AgregarSignoConPaciente(ModeloSignosVitales signo)
        {
            using var con = new SqlConnection(_connectionString);
            con.Open();

            using var transaction = con.BeginTransaction();

            try
            {
                // Insertar en Signos_vitales
                string querySigno = @"
                    INSERT INTO Signos_vitales
                    (id_signo, ci_paciente, fecha, hora, ritmo_cardiaco, presion_arterial, temperatura, oxigenacion)
                    VALUES (@IdSigno, @CiPaciente, @Fecha, @Hora, @RitmoCardiaco, @PresionArterial, @Temperatura, @Oxigenacion);";

                using (var cmd = new SqlCommand(querySigno, con, transaction))
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

                // Insertar en pac_signos con fecha
                string queryPacSigno = @"
                    INSERT INTO pac_signos (ci_paciente, id_signo, fecha)
                    VALUES (@CiPaciente, @IdSigno, @Fecha);";

                using (var cmd = new SqlCommand(queryPacSigno, con, transaction))
                {
                    cmd.Parameters.AddWithValue("@CiPaciente", signo.CiPaciente);
                    cmd.Parameters.AddWithValue("@IdSigno", signo.IdSigno);
                    cmd.Parameters.AddWithValue("@Fecha", signo.Fecha);
                    cmd.ExecuteNonQuery();
                }

                transaction.Commit();
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }

        // Listar signos de un paciente
        public List<ModeloSignosVitales> ListarSignosDePaciente(int ciPaciente)
        {
            List<ModeloSignosVitales> lista = new List<ModeloSignosVitales>();

            using var con = new SqlConnection(_connectionString);
            con.Open();

            string query = @"
                SELECT id_signo, ci_paciente, fecha, hora, ritmo_cardiaco, presion_arterial, temperatura, oxigenacion
                FROM Signos_vitales
                WHERE ci_paciente = @ci
                ORDER BY fecha DESC, hora DESC;";

            using var cmd = new SqlCommand(query, con);
            cmd.Parameters.AddWithValue("@ci", ciPaciente);

            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                lista.Add(new ModeloSignosVitales
                {
                    IdSigno = Convert.ToInt32(reader["id_signo"]),
                    CiPaciente = Convert.ToInt32(reader["ci_paciente"]),
                    Fecha = Convert.ToDateTime(reader["fecha"]),
                    Hora = TimeSpan.Parse(reader["hora"].ToString()),
                    RitmoCardiaco = Convert.ToInt32(reader["ritmo_cardiaco"]),
                    PresionArterial = Convert.ToInt32(reader["presion_arterial"]),
                    Temperatura = Convert.ToDecimal(reader["temperatura"]),
                    Oxigenacion = Convert.ToInt32(reader["oxigenacion"])
                });
            }

            return lista;
        }
    }
}
