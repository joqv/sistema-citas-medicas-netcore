using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PrimerApi.Model;
using PrimerApi.Repository.DAO;

namespace CiberCare_Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PacienteController : ControllerBase
    {
        
        [HttpGet("getPacientes")]
        public async Task<ActionResult<List<Pacientes>>> getPacientes()
        {
            var lista = await Task.Run(() => new PacienteDao().getPaciente());
            return Ok(lista);
        }

        [HttpPost("insertPacientes")]
        public async Task<ActionResult<string>> insertPacientes(Pacientes reg)
        {
            var mensaje = await Task.Run(() => new PacienteDao().insertPacientes(reg));
            return Ok(mensaje);
        }

        [HttpPut("updatePacientes")]
        public async Task<ActionResult<string>> updatePacientes(Pacientes reg)
        {
            var mensaje = await Task.Run(() => new PacienteDao().updatePacientes(reg));
            return Ok(mensaje);
        }

        [HttpGet("getPaciente/{dni}")]
        public async Task<ActionResult<Pacientes>> getPaciente(string dni)
        {
            var cliente = await Task.Run(() => new PacienteDao().getPacientes(dni));
            return Ok(cliente);
        }

        [HttpDelete("deleteCliente/{dni}")]

        public async Task<ActionResult<string>> deletePacientes(string dni)
        {
            var cliente = new PacienteDao().getPacientes(dni);
            if (cliente == null)
            {
                return NotFound("Paciente no encontrado.");
            }
            var mensaje = await Task.Run(() => new PacienteDao().deletePacientes(cliente));
            return Ok(mensaje); 
        }
        [HttpPost("login")]
        public async Task<ActionResult> Login([FromBody] Pacientes pacienteLogin)
        {
            if (pacienteLogin == null || string.IsNullOrEmpty(pacienteLogin.email) || string.IsNullOrEmpty(pacienteLogin.contrasena))
            {
                return BadRequest(new { mensaje = "Email y contraseña son obligatorios." });
            }

            var dao = new PacienteDao();
            var paciente = await Task.Run(() => dao.GetPacienteLogin(pacienteLogin.email!, pacienteLogin.contrasena!));

            if (paciente == null)
            {
                return Unauthorized(new { mensaje = "Email o contraseña incorrectos." });
            }

            return Ok(new
            {
                mensaje = "Inicio de sesión exitoso.",
                paciente = new
                {
                    dni = paciente.dni,
                    nombre = paciente.nombre,
                    apellido = paciente.apellido,
                    telefono = paciente.telefono,
                    email = paciente.email
                }
            });
        }

    }



}

