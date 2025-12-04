using Microsoft.Data.SqlClient;
using PrimerApi.Model;
using Microsoft.Extensions.Configuration;
using PrimerApi.Repository.Interfaces;
using System.Data;

namespace PrimerApi.Repository.DAO
{
    public class PacienteDao : IPacientes
    {
        private readonly string cadena;

        public PacienteDao()
        {
            cadena = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build()
                .GetConnectionString("ConexionBD");
        }

        // LISTAR
        public IEnumerable<Pacientes> getPaciente()
        {
            List<Pacientes> lista = new List<Pacientes>();
            using (SqlConnection cn = new SqlConnection(cadena))
            {
                cn.Open();
                SqlCommand cmd = new SqlCommand("usp_listar_pacientes", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    lista.Add(new Pacientes()
                    {
                        dni = dr.GetString(0),
                        nombre = dr.GetString(1),
                        apellido = dr.GetString(2),
                        telefono = dr.GetString(3),
                        email = dr.GetString(4),
                        contrasena = dr.IsDBNull(5) ? "" : dr.GetString(5)
                    });
                }
                dr.Close();
            }
            return lista;
        }

        // OBTENER POR DNI
        public Pacientes getPacientes(string dni)
        {
            return getPaciente().FirstOrDefault(p => p.dni == dni);
        }

        // INSERTAR
        public string insertPacientes(Pacientes reg)
        {
            string mensaje = "";
            using (SqlConnection cn = new SqlConnection(cadena))
            {
                try
                {
                    SqlCommand cmd = new SqlCommand("usp_insertar_paciente", cn);
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@dni", reg.dni);
                    cmd.Parameters.AddWithValue("@nombre", reg.nombre);
                    cmd.Parameters.AddWithValue("@apellido", reg.apellido);
                    cmd.Parameters.AddWithValue("@telefono", reg.telefono);
                    cmd.Parameters.AddWithValue("@email", reg.email);
                    cmd.Parameters.AddWithValue("@contrasena", reg.contrasena);

                    cn.Open();
                    int i = cmd.ExecuteNonQuery();
                    mensaje = $"Se agregó {i} Paciente(s)";
                }
                catch (Exception ex)
                {
                    mensaje = ex.Message;
                }
            }
            return mensaje;
        }

        // ACTUALIZAR
        public string updatePacientes(Pacientes reg)
        {
            string mensaje = "";
            using (SqlConnection cn = new SqlConnection(cadena))
            {
                try
                {
                    SqlCommand cmd = new SqlCommand("usp_actualizar_paciente", cn);
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@dni", reg.dni);
                    cmd.Parameters.AddWithValue("@nombre", reg.nombre);
                    cmd.Parameters.AddWithValue("@apellido", reg.apellido);
                    cmd.Parameters.AddWithValue("@telefono", reg.telefono);
                    cmd.Parameters.AddWithValue("@email", reg.email);
                    cmd.Parameters.AddWithValue("@contrasena", reg.contrasena);

                    cn.Open();
                    int i = cmd.ExecuteNonQuery();
                    mensaje = $"Se actualizó {i} Paciente(s)";
                }
                catch (Exception ex)
                {
                    mensaje = ex.Message;
                }
            }
            return mensaje;
        }

        // ELIMINAR
        public string deletePacientes(Pacientes reg)
        {
            string mensaje = "";
            using (SqlConnection cn = new SqlConnection(cadena))
            {
                try
                {
                    SqlCommand cmd = new SqlCommand("usp_eliminar_paciente", cn);
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@dni", reg.dni);

                    cn.Open();
                    int i = cmd.ExecuteNonQuery();
                    mensaje = $"Se eliminó {i} Paciente(s)";
                }
                catch (Exception ex)
                {
                    mensaje = ex.Message;
                }
            }
            return mensaje;
        }

        // LOGIN
        public Pacientes? GetPacienteLogin(string email, string contrasena)
        {
            Pacientes? paciente = null;
            using (SqlConnection cn = new SqlConnection(cadena))
            {
                cn.Open();
                SqlCommand cmd = new SqlCommand("usp_login_paciente", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@correo", email);
                cmd.Parameters.AddWithValue("@contrasena", contrasena);

                SqlDataReader dr = cmd.ExecuteReader();
                if (dr.Read())
                {
                    paciente = new Pacientes()
                    {
                        dni = dr.GetString(0),
                        nombre = dr.GetString(1),
                        apellido = dr.GetString(2),
                        telefono = dr.GetString(3),
                        email = dr.GetString(4),
                        contrasena = dr.GetString(5)
                    };
                }
                dr.Close();
            }
            return paciente;
        }
    }
}




