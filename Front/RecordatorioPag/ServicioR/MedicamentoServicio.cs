using Front.RecordatorioPag.ModelosR;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Configuration;

namespace Front.RecordatorioPag.ServicioR
{
    public class MedicamentoServicio
    {
        private List<Medicamento> medicamentos = new List<Medicamento>();

        public List<Medicamento> ListarMedicamentos() => medicamentos;

        private SqlConnection GetConnection()
        {
            return new SqlConnection(ConfigurationManager.ConnectionStrings["cnHealthyU"].ConnectionString);
        }

        public void CargarMedicamentos()
        {
            medicamentos.Clear();
            using (var con = GetConnection())
            {
                con.Open();
                string query = "SELECT id_medicamento, nombre, descripcion, dosis, unidad FROM Medicamento";
                using (var cmd = new SqlCommand(query, con))
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        // Convertimos la dosis a decimal de forma segura
                        decimal dosis = Convert.ToDecimal(reader.GetValue(3));

                        medicamentos.Add(new Medicamento(
                            reader.GetInt32(0),
                            reader.GetString(1),
                            reader.IsDBNull(2) ? "" : reader.GetString(2),
                            dosis,
                            reader.GetString(4)
                        ));
                    }
                }
            }
        }


        public int ObtenerNuevoId()
        {
            if (medicamentos.Count == 0) return 1;
            return medicamentos[medicamentos.Count - 1].Id_medicamento + 1;
        }

        public void AgregarMedicamento(Medicamento med)
        {
            using (var con = GetConnection())
            {
                con.Open();
                string insertQuery = @"
                    INSERT INTO Medicamento (id_medicamento, nombre, descripcion, dosis, unidad)
                    VALUES (@id, @nombre, @descripcion, @dosis, @unidad)";
                using (var cmd = new SqlCommand(insertQuery, con))
                {
                    cmd.Parameters.AddWithValue("@id", med.Id_medicamento);
                    cmd.Parameters.AddWithValue("@nombre", med.Nombre);
                    cmd.Parameters.AddWithValue("@descripcion", med.Descripcion);
                    cmd.Parameters.AddWithValue("@dosis", med.Dosis);
                    cmd.Parameters.AddWithValue("@unidad", med.Unidad);
                    cmd.ExecuteNonQuery();
                }
            }
            medicamentos.Add(med);
        }

        public int ObtenerIdPorNombre(string nombre)
        {
            foreach (var med in medicamentos)
                if (med.Nombre.Equals(nombre, StringComparison.OrdinalIgnoreCase))
                    return med.Id_medicamento;
            return 0;
        }
    }
}
