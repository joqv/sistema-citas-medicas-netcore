using CiberCare_Api.Repository.DAO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PrimerApi.Model;

namespace CiberCare_Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HorariosController : ControllerBase
    {
       
            [HttpGet("getHorario")]
            public async Task<ActionResult<List<Horarios>>> getHorario()
            {
                var lista = await Task.Run(() => new HorarioDao().getHorario());
                return Ok(lista);
            }

        [HttpPost("insertHorarios")]
        public async Task<ActionResult<string>> insertHorarios(Horarios reg)
        {
            var mensaje = await Task.Run(() => new HorarioDao().insertHorarios(reg));
            return Ok(mensaje);
        }

        [HttpPut("updateHorarios")]
        public async Task<ActionResult<string>> updateHorarios(Horarios reg)
        {
            var mensaje = await Task.Run(() => new HorarioDao().updateHorarios(reg));
            return Ok(mensaje);
        }

        [HttpGet("getHorario/{id_horario}")]
        public async Task<ActionResult<Horarios>> getHorario(Int32 id_horario)
        {
            var cliente = await Task.Run(() => new HorarioDao().getHorario(id_horario));
            return Ok(cliente);
        }
        [HttpGet("getPorHora/{hora}")]
        public async Task<ActionResult<List<Horarios>>> getHorario(TimeOnly hora)
        {
            var cliente = await Task.Run(() => new HorarioDao().getHorario(hora));
            return Ok(cliente);
        }
        [HttpDelete("deleteHorarios/{id_horario}")]

        public async Task<ActionResult<string>> deleteHorarios(Int32 id_horario)
        {
            var cliente = new HorarioDao().getHorario(id_horario);
            if (cliente == null)
            {
                return NotFound("Paciente no encontrado.");
            }
            var mensaje = await Task.Run(() => new HorarioDao().deleteHorarios(cliente));
            return Ok(mensaje);
        }
    }
    }
