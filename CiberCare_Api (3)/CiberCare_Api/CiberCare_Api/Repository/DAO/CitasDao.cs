using CiberCare_Api.Model;
using CiberCare_Api.Repository.Interfaces;
using Microsoft.Data.SqlClient;
using PrimerApi.Model;
using System.Data;

namespace CiberCare_Api.Repository.DAO
{
    public class CitasDao : ICitas
    {
        private readonly string cadena;
        public CitasDao()
        {
            cadena = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetConnectionString("ConexionBD");
        }


        public IEnumerable<Citas> getCitas()
        {
            List<Citas> temporal = new List<Citas>();
            using (SqlConnection cn = new SqlConnection(cadena))
            {
                cn.Open();
                SqlCommand cmd = new SqlCommand("exec usp_listar_citas", cn);
                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    temporal.Add(new Citas()
                    {
                        id_cita = dr.GetInt32(0),
                        dni_paciente = dr.GetString(1),
                        nombre_paciente = dr.GetString(2),
                        id_horario = dr.GetInt32(3),
                        fecha = DateOnly.FromDateTime(dr.GetDateTime(4)),
                        hora = dr.GetString(5),
                        nombre_doctor = dr.GetString(6),
                        especialidad = dr.GetString(7),



                    });
                }
                dr.Close();
            }
            return temporal;
        }

        public Citas getCitas(string dni_paciente)
        {
            return getCitas().FirstOrDefault(c => c.dni_paciente == dni_paciente);
        }
        public Citas getCitas(int id_cita)
        {
            return getCitas().FirstOrDefault(c => c.id_cita == id_cita);
        }


        public string insertCitas(Citas reg)
        {
            string mensaje = "";
            using (SqlConnection cn = new SqlConnection(cadena))
            {
                try
                {
                    SqlCommand cmd = new SqlCommand("usp_registrar_cita", cn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@dni_paciente", reg.dni_paciente);
                    cmd.Parameters.AddWithValue("@id_horario", reg.id_horario);

                    cn.Open();
                    int i = cmd.ExecuteNonQuery();
                    mensaje = $"Se ha agregado {i} Cita(s)";
                }
                catch (Exception ex)
                {
                    mensaje = ex.Message;
                }
                finally
                {
                    cn.Close();
                }
            }
            return mensaje;
        }

        public string updateCitas(CrearCitaDTO reg)
        {
            string mensaje = "";
            using (SqlConnection cn = new SqlConnection(cadena))
            {
                try
                {
                    SqlCommand cmd = new SqlCommand("usp_actualizar_cita", cn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@id_cita", reg.id_cita);
                    cmd.Parameters.AddWithValue("@nuevo_id_horario", reg.id_horario);
                    cn.Open();
                    int i = cmd.ExecuteNonQuery();
                    mensaje = $"Se ha actualizado {i} Citas(s)";
                }
                catch (Exception ex)
                {
                    mensaje = ex.Message;
                }
                finally
                {
                    cn.Close();
                }
            }
            return mensaje;
        }
        public string deleteCitas(Citas reg)
        {
            string mensaje = "";
            using (SqlConnection cn = new SqlConnection(cadena))
            {
                try
                {
                    SqlCommand cmd = new SqlCommand("usp_eliminar_cita", cn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@id_cita", reg.id_cita);
                    cn.Open();
                    int i = cmd.ExecuteNonQuery();
                    mensaje = $"Se ha eliminado {i} Cita(s)";
                }
                catch (Exception ex)
                {
                    mensaje = ex.Message;
                }
                finally
                {
                    cn.Close();
                }

                return mensaje;
            }
        }
    }
}
