using CiberCare_Api.Repository.Interfaces;
using Microsoft.Data.SqlClient;
using PrimerApi.Model;
using System.Data;

namespace CiberCare_Api.Repository.DAO
{
    public class DoctoresDao : IDoctores
    {
        private readonly string cadena;
        public DoctoresDao()
        {
            cadena = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetConnectionString("ConexionBD");
        }
        public IEnumerable<Doctores> getDoctor()
        {
            List<Doctores> temporal = new List<Doctores>();
            using (SqlConnection cn = new SqlConnection(cadena))
            {
                cn.Open();
                SqlCommand cmd = new SqlCommand("exec usp_listar_doctores", cn);
                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    temporal.Add(new Doctores()
                    {
                        id_doctor = dr.GetInt32(0),
                        nombre = dr.GetString(1),
                        apellido = dr.GetString(2),
                        cmp = dr.GetString(3),
                        celular = dr.GetString(4),
                        id_especialidad = dr.GetInt32(5) 

                    });
                }
                dr.Close();
            }
            return temporal;

        }

        public Doctores getDoctores(int id_doctor)
        {
            return getDoctor().FirstOrDefault(c => c.id_doctor == id_doctor);
        }

        public string insertDoctores(Doctores reg)
        {
            string mensaje = "";
            using (SqlConnection cn = new SqlConnection(cadena))
            {
                try
                {
                    SqlCommand cmd = new SqlCommand("usp_insertar_doctor", cn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@nombre", reg.nombre);
                    cmd.Parameters.AddWithValue("@apellido", reg.apellido);
                    cmd.Parameters.AddWithValue("@cmp", reg.cmp);
                    cmd.Parameters.AddWithValue("@celular", reg.celular);
                    cmd.Parameters.AddWithValue("@id_especialidad", reg.id_especialidad);



                    cn.Open();
                    int i = cmd.ExecuteNonQuery();
                    mensaje = $"Se ha agregado {i} Doctor(s)";
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

        public string updateDoctores(Doctores reg)
        {
            string mensaje = "";
            using (SqlConnection cn = new SqlConnection(cadena))
            {
                try
                {
                    SqlCommand cmd = new SqlCommand("usp_actualizar_doctor", cn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@id_doctor", reg.id_doctor);
                    cmd.Parameters.AddWithValue("@nombre", reg.nombre);
                    cmd.Parameters.AddWithValue("@apellido", reg.apellido);
                    cmd.Parameters.AddWithValue("@cmp", reg.cmp);
                    cmd.Parameters.AddWithValue("@celular", reg.celular);
                    cmd.Parameters.AddWithValue("@id_especialidad", reg.id_especialidad);



                    cn.Open();
                    int i = cmd.ExecuteNonQuery();
                    mensaje = $"Se ha actualizado {i} Doctore(s)";
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
        public string deleteDoctores(Doctores reg)
        {
            string mensaje = "";
            using (SqlConnection cn = new SqlConnection(cadena))
            {
                try
                {
                    SqlCommand cmd = new SqlCommand("usp_eliminar_doctor", cn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@id_doctor", reg.id_doctor);
                    //SE AGREGOOO 
                    SqlParameter pMensaje = new SqlParameter("@mensaje", SqlDbType.VarChar, 200);
                    pMensaje.Direction = ParameterDirection.Output;
                    cmd.Parameters.Add(pMensaje);

                    cn.Open();
                    int i = cmd.ExecuteNonQuery();
                    mensaje = pMensaje.Value.ToString();// $"Se ha eliminado {i} Doctores(s)";
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

        public Doctores getDoctor(string nombre)
        {
            return getDoctor().FirstOrDefault(c => c.nombre == nombre);
        }
    }
}
