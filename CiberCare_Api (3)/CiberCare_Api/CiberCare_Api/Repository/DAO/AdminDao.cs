using CiberCare_Api.Model;
using CiberCare_Api.Repository.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Data.SqlClient;
using PrimerApi.Model;
using System.Data;

namespace CiberCare_Api.Repository.DAO
{
    public class AdminDao : IAdmin
    {

        private readonly string cadena;
        public AdminDao()
        {
            cadena = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetConnectionString("ConexionBD");
        }


        public Admin? GetAdminmLogin(string usuario, string contrasena)
        {
            Admin? result = null;

            using (SqlConnection cn = new SqlConnection(cadena))
            {
                cn.Open();
                SqlCommand cmd = new SqlCommand("sp_Login", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Usuario", usuario);
                cmd.Parameters.AddWithValue("@Contrasena", contrasena);

                using SqlDataReader dr = cmd.ExecuteReader();
                if (dr.Read() && dr["Mensaje"].ToString() == "Login exitoso")
                {
                    result = new Admin
                    {
                        usuario = usuario,
                        contrasena = contrasena
                    };
                }
            }

            return result;
        }


        public IEnumerable<Admin> GetAdmins()
        {
            List<Admin> temporal = new List<Admin>();
            using (SqlConnection cn = new SqlConnection(cadena))
            {
                cn.Open();
                SqlCommand cmd = new SqlCommand("exec usp_listar_admin", cn);
                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    temporal.Add(new Admin()
                    {
                        usuario = dr.GetString(0),
                        contrasena = dr.GetString(1),

                    });
                }
                dr.Close();
            }
            return temporal;
        }
    }
}
