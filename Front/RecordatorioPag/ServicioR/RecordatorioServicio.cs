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

        public List<Recordatorio> ListarRecordatorios() => recordatorios;

        private SqlConnection GetConnection()
        {
            return new SqlConnection(ConfigurationManager.ConnectionStrings["cnHealthyU"].ConnectionString);
        }

        public void CargarRecordatorios()
        {
            recordatorios.Clear();
            using (var con = GetConnection())
            {
                con.Open();
                string query = @"
                    SELECT r.id_recordatorio, r.id_medicamento, r.fecha, r.hora_inicio, r.frecuencia, r.estado, m.nombre
                    FROM Recordatorio r
                    JOIN Medicamento m ON r.id_medicamento = m.id_medicamento";
                using (var cmd = new SqlCommand(query, con))
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        recordatorios.Add(new Recordatorio(
                            reader.GetInt32(0),
                            reader.GetDateTime(2),
                            reader.GetDateTime(3),
                            reader.GetInt32(4),
                            reader.GetBoolean(5),
                            reader.GetString(6)
                        ));
                    }
                }
            }
        }

        public int ObtenerNuevoId()
        {
            if (recordatorios.Count == 0) return 1;
            return recordatorios[recordatorios.Count - 1].Id_recordatorio + 1;
        }

        public void AgregarRecordatorio(Recordatorio rec)
        {
            using (var con = GetConnection())
            {
                con.Open();
                string insertQuery = @"
                    INSERT INTO Recordatorio (id_recordatorio, id_medicamento, fecha, hora_inicio, frecuencia, estado)
                    VALUES (@id, @idMed, @fecha, @hora, @frecuencia, 1)";
                using (var cmd = new SqlCommand(insertQuery, con))
                {
                    cmd.Parameters.AddWithValue("@id", rec.Id_recordatorio);
                    cmd.Parameters.AddWithValue("@idMed", rec.MedicamentoNombre);
                    cmd.Parameters.AddWithValue("@fecha", rec.Fecha);
                    cmd.Parameters.AddWithValue("@hora", rec.Hora_inicio.TimeOfDay);
                    cmd.Parameters.AddWithValue("@frecuencia", rec.Frecuencia);
                    cmd.ExecuteNonQuery();
                }
            }
            recordatorios.Add(rec);
        }
    }
}
