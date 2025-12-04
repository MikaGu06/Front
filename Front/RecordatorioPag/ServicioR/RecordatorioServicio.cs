using Front.RecordatorioPag.ModelosR;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Configuration;

namespace Front.RecordatorioPag.ServicioR
{
    public class RecordatorioServicio
    {
        private List<Recordatorio> recordatorios = new List<Recordatorio>();

        private SqlConnection GetConnection()
        {
            return new SqlConnection(ConfigurationManager.ConnectionStrings["cnHealthyU"].ConnectionString);
        }

       
        public List<Recordatorio> ListarRecordatoriosPorPaciente(int ciPaciente)
        {
            var lista = new List<Recordatorio>();

            using (var con = GetConnection())
            {
                con.Open();
                string query = @"
                    SELECT r.id_recordatorio, r.id_medicamento, r.fecha, r.hora_inicio, r.frecuencia, r.estado, m.nombre
                    FROM Recordatorio r
                    JOIN pac_rec pr ON r.id_recordatorio = pr.id_recordatorio
                    JOIN Medicamento m ON r.id_medicamento = m.id_medicamento
                    WHERE pr.ci_paciente = @ci";

                using (var cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@ci", ciPaciente);

                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            DateTime fecha = reader.GetDateTime(2);
                            TimeSpan hora = reader.GetTimeSpan(3);
                            DateTime fechaHoraInicio = fecha.Date + hora;

                            string nombreMed = reader.IsDBNull(6) ? "" : reader.GetString(6);

                            lista.Add(new Recordatorio(
                                reader.GetInt32(0),
                                fecha,
                                fechaHoraInicio,
                                reader.GetInt32(4),
                                reader.GetBoolean(5),
                                nombreMed,
                                ciPaciente
                            ));
                        }
                    }
                }
            }

            return lista;
        }

        
        public void AgregarRecordatorio(Recordatorio rec, int idMed)
        {
            using (var con = GetConnection())
            {
                con.Open();
                string query = @"
                    INSERT INTO Recordatorio 
                        (id_recordatorio, id_medicamento, fecha, hora_inicio, frecuencia, estado, ci_paciente)
                    VALUES (@id, @idMed, @fecha, @hora, @frecuencia, @estado, @ci)";

                using (var cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@id", rec.Id_recordatorio);
                    cmd.Parameters.AddWithValue("@idMed", idMed);
                    cmd.Parameters.AddWithValue("@fecha", rec.Fecha);
                    cmd.Parameters.AddWithValue("@hora", rec.Hora_inicio.TimeOfDay);
                    cmd.Parameters.AddWithValue("@frecuencia", rec.Frecuencia);
                    cmd.Parameters.AddWithValue("@estado", rec.Estado);
                    cmd.Parameters.AddWithValue("@ci", rec.CiPaciente); // 🔹 importante
                    cmd.ExecuteNonQuery();
                }
            }
        }

        
        public void AsignarRecordatorioAPaciente(int idRec, int ciPaciente, DateTime fecha)
        {
            using (var con = GetConnection())
            {
                con.Open();
                string query = @"
                    INSERT INTO pac_rec (id_recordatorio, ci_paciente, fecha)
                    VALUES (@id, @ci, @fecha)";
                using (var cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@id", idRec);
                    cmd.Parameters.AddWithValue("@ci", ciPaciente);
                    cmd.Parameters.AddWithValue("@fecha", fecha);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        
        public void ActualizarEstadoEnBD(Recordatorio rec)
        {
            using (var con = GetConnection())
            {
                con.Open();
                string query = "UPDATE Recordatorio SET estado = @estado WHERE id_recordatorio = @id";
                using (var cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@estado", rec.Estado);
                    cmd.Parameters.AddWithValue("@id", rec.Id_recordatorio);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        
        public int ObtenerNuevoId()
        {
            using (var con = GetConnection())
            {
                con.Open();
                string query = "SELECT ISNULL(MAX(id_recordatorio), 0) FROM Recordatorio";
                using (var cmd = new SqlCommand(query, con))
                {
                    return (int)cmd.ExecuteScalar() + 1;
                }
            }
        }
    }
}
