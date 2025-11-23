using CiberCare_Api.Repository.Interfaces;
using Microsoft.Data.SqlClient;
using PrimerApi.Model;
using System.Data;

namespace CiberCare_Api.Repository.DAO
{
    public class HorarioDao : IHorarios
    {
        private readonly string cadena;
        public HorarioDao()
        {
            cadena = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetConnectionString("ConexionBD");
        }

        public Horarios getHorario(Int32 id_horario)
        {
            return getHorario().FirstOrDefault(c => c.id_horario == id_horario);
        }

        public IEnumerable<Horarios> getHorario()
        {
            List<Horarios> temporal = new List<Horarios>();
            using (SqlConnection cn = new SqlConnection(cadena))
            {
                cn.Open();
                SqlCommand cmd = new SqlCommand("exec usp_listar_horarios_disponibles", cn);
                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    temporal.Add(new Horarios()
                    {
                        id_horario = dr.GetInt32(0),
                        fecha = DateOnly.FromDateTime(dr.GetDateTime(1)),
                        hora = TimeOnly.FromTimeSpan(dr.GetTimeSpan(2)),
                        id_doctor = dr.GetInt32(3),
                        apellido = dr.GetString(4),
                        id_especialidad = dr.GetInt32(5),
                    });
                }
                dr.Close();
            }
            return temporal;
        }
        public string insertHorarios(Horarios reg)
        {

            string mensaje = "";
            using (SqlConnection cn = new SqlConnection(cadena))
            {
                try
                {
                    SqlCommand cmd = new SqlCommand("usp_insertar_horario", cn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@fecha", reg.fecha);
                    cmd.Parameters.AddWithValue("@hora", reg.hora);
                    cmd.Parameters.AddWithValue("@id_doctor", reg.id_doctor);


                    cn.Open();
                    int i = cmd.ExecuteNonQuery();
                    mensaje = $"Se ha agregado {i} Horario(s)";
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

        public string updateHorarios(Horarios reg)
        {
            string mensaje = "";
            using (SqlConnection cn = new SqlConnection(cadena))
            {
                try
                {
                    SqlCommand cmd = new SqlCommand("usp_actualizar_horario", cn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@id_horario", reg.id_horario);
                    cmd.Parameters.AddWithValue("@fecha", reg.fecha);
                    cmd.Parameters.AddWithValue("@hora", reg.hora);
                    cmd.Parameters.AddWithValue("@id_doctor", reg.id_doctor);
                    cmd.Parameters.AddWithValue("@disponible", reg.disponible);


                    cn.Open();
                    int i = cmd.ExecuteNonQuery();
                    mensaje = $"Se ha actualizado {i} Horario(s)";
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
        public string deleteHorarios(Horarios reg)
        {
            string mensaje = "";
            using (SqlConnection cn = new SqlConnection(cadena))
            {
                try
                {
                    SqlCommand cmd = new SqlCommand("usp_eliminar_horario", cn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@id_horario", reg.id_horario);
                    cn.Open();
                    int i = cmd.ExecuteNonQuery();
                    mensaje = $"Se ha eliminado {i} Horario(s)";
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
        public Horarios getHorario(TimeOnly hora)
        {
            return getHorario().FirstOrDefault(c => c.hora == hora);
        }
    }
}
