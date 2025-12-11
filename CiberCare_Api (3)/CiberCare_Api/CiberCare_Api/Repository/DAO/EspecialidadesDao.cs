using Microsoft.Data.SqlClient;
using System.Data;
using Microsoft.Extensions.Configuration;
using PrimerApi.Model;
using PrimerApi.Repository.Interfaces;
using System.Net;

namespace PrimerApi.Repository.DAO
{
    public class EspecialidadesDao : IEspecialidades
    {
        private readonly string cadena;
        public EspecialidadesDao()
        {
            cadena = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetConnectionString("ConexionBD");
        }

        public string deleteEspecialidades(Especialidades reg)
        {
            string mensaje = "";
            using (SqlConnection cn = new SqlConnection(cadena))
            {
                try
                {
                    SqlCommand cmd = new SqlCommand("usp_eliminar_especialidad", cn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@id_especialidad", reg.id_especialidad);
                    cn.Open();
                    int i = cmd.ExecuteNonQuery();
                    mensaje = $"Se ha eliminado {i} Especialidade(s)";
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

        public IEnumerable<Especialidades> getEspecialidade()
        
            {
                List<Especialidades> temporal = new List<Especialidades>();
                using (SqlConnection cn = new SqlConnection(cadena))
                {
                    cn.Open();
                    SqlCommand cmd = new SqlCommand("exec usp_listar_especialidad", cn);
                    SqlDataReader dr = cmd.ExecuteReader();
                    while (dr.Read())
                    {
                        temporal.Add(new Especialidades()
                        {
                            id_especialidad = dr.GetInt32(0),
                            nombre = dr.GetString(1),
                        });
                    }
                    dr.Close();
                }
                return temporal;
            }

        public Especialidades getEspecialidades(Int32 id_especialidad)
        {
            return getEspecialidade().FirstOrDefault(c => c.id_especialidad == id_especialidad);
        }

        public string insertEspecialidades(Especialidades reg)
        {

            string mensaje = "";
            using (SqlConnection cn = new SqlConnection(cadena))
            {
                try
                {
                    SqlCommand cmd = new SqlCommand("usp_insertar_especialidad", cn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@nombre", reg.nombre);
                   

                    cn.Open();
                    int i = cmd.ExecuteNonQuery();
                    mensaje = $"Se ha agregado {i} Especialidade(s)";
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

        public string updateEspecialidades(Especialidades reg)
        {
            string mensaje = "";
            using (SqlConnection cn = new SqlConnection(cadena))
            {
                try
                {
                    SqlCommand cmd = new SqlCommand("usp_actualizar_especialidad", cn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@id_especialidad", reg.id_especialidad);
                    cmd.Parameters.AddWithValue("@nombre", reg.nombre);
              
                    cn.Open();
                    int i = cmd.ExecuteNonQuery();
                    mensaje = $"Se ha actualizado {i} Especialidade(s)";
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

        public Especialidades getEspecialidades(string nombre)
        {
            return getEspecialidade().FirstOrDefault(c => c.nombre == nombre);
        }

    }

}
