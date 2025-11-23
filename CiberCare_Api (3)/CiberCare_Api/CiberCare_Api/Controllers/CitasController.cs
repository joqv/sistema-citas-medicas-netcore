using CiberCare_Api.Model;
using CiberCare_Api.Repository.DAO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CiberCare_Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CitasController : ControllerBase
    {

        [HttpGet("getCitas")]
        public async Task<ActionResult<List<Citas>>> getCitas()
        {
            var lista = await Task.Run(() => new CitasDao().getCitas());
            return Ok(lista);
        }

        [HttpPost("insertCitas")]
        public async Task<ActionResult<string>> insertCitas([FromBody] CrearCitaDTO reg)
        {
            try
            {
                var cita = new Citas
                {
                    dni_paciente = reg.dni_paciente,
                    id_horario = reg.id_horario
                };

                var mensaje = await Task.Run(() => new CitasDao().insertCitas(cita));
                return Ok(mensaje);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error al insertar la cita: {ex.Message}");
            }
        }


        [HttpPut("updateCitas")]
        public async Task<ActionResult<string>> updateCitas(CrearCitaDTO reg)
        {
            var mensaje = await Task.Run(() => new CitasDao().updateCitas(reg));
            return Ok(mensaje);
        }

        [HttpGet("getCitas/{dni_paciente}")]
        public async Task<ActionResult<Citas>> getCitas(string dni_paciente)
        {
            var cliente = await Task.Run(() => new CitasDao().getCitas(dni_paciente));
            return Ok(cliente);
        }

        [HttpDelete("deleteCitas/{id_cita}")]

        public async Task<ActionResult<string>> deleteCitas(Int32 id_cita)
        {
            var cliente = new CitasDao().getCitas(id_cita);
            if (cliente == null)
            {
                return NotFound("Paciente no encontrado.");
            }
            var mensaje = await Task.Run(() => new CitasDao().deleteCitas(cliente));
            return Ok(mensaje);
        }

    }
}
