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
            // Asegúrate de que tienes una referencia a System.Configuration si usas ConfigurationManager
            return new SqlConnection(ConfigurationManager.ConnectionStrings["cnHealthyU"].ConnectionString);
        }

        // 🔥 MODIFICADO: Acepta ciPaciente para filtrar
        public void CargarMedicamentos(int ciPaciente)
        {
            medicamentos.Clear();
            using (var con = GetConnection())
            {
                con.Open();
                // 🔥 MODIFICADA la consulta: incluye ci_paciente en SELECT y usa WHERE para filtrar
                string query = "SELECT id_medicamento, nombre, descripcion, dosis, unidad, ci_paciente FROM Medicamento WHERE ci_paciente = @ciPaciente";
                using (var cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@ciPaciente", ciPaciente); // Agrega el parámetro de filtro

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
                                reader.GetString(4),
                                reader.GetInt32(5) // Lee el ci_paciente
                            ));
                        }
                    }
                }
            }
        }

        public int ObtenerNuevoId()
        {
            // Mejorar para obtener el MAX(ID) de la BD para evitar colisiones
            using (var con = GetConnection())
            {
                con.Open();
                string query = "SELECT ISNULL(MAX(id_medicamento), 0) FROM Medicamento";
                using (var cmd = new SqlCommand(query, con))
                {
                    return (int)cmd.ExecuteScalar() + 1;
                }
            }
        }

        // 🔥 MODIFICADO: Incluye ci_paciente en la inserción
        public void AgregarMedicamento(Medicamento med)
        {
            using (var con = GetConnection())
            {
                con.Open();
                string insertQuery = @"
                    INSERT INTO Medicamento (id_medicamento, nombre, descripcion, dosis, unidad, ci_paciente)
                    VALUES (@id, @nombre, @descripcion, @dosis, @unidad, @ciPaciente)";
                using (var cmd = new SqlCommand(insertQuery, con))
                {
                    cmd.Parameters.AddWithValue("@id", med.Id_medicamento);
                    cmd.Parameters.AddWithValue("@nombre", med.Nombre);
                    cmd.Parameters.AddWithValue("@descripcion", med.Descripcion);
                    cmd.Parameters.AddWithValue("@dosis", med.Dosis);
                    cmd.Parameters.AddWithValue("@unidad", med.Unidad);
                    cmd.Parameters.AddWithValue("@ciPaciente", med.CiPaciente); // Agrega el parámetro
                    cmd.ExecuteNonQuery();
                }
            }
            medicamentos.Add(med);
        }

        public int ObtenerIdPorNombre(string nombre)
        {
            // Si solo se debe buscar entre los medicamentos del paciente actual, este método debería 
            // modificarse para aceptar también el ciPaciente. Por simplicidad, por ahora busca en la lista cargada.
            foreach (var med in medicamentos)
                if (med.Nombre.Equals(nombre, StringComparison.OrdinalIgnoreCase))
                    return med.Id_medicamento;
            return 0;
        }
    }
}