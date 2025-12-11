using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PrimerApi.Model;
using PrimerApi.Repository.DAO;

namespace CiberCare_Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EspecialidadesController : ControllerBase
    {
        [HttpGet("getEspecialidade")]
        public async Task<ActionResult<List<Especialidades>>> getEspecialidade()
        {
            var lista = await Task.Run(() => new EspecialidadesDao().getEspecialidade());
            return Ok(lista);
        }

        [HttpPost("insertEspecialidades")]
        public async Task<ActionResult<string>> insertEspecialidades(Especialidades reg)
        {
            var mensaje = await Task.Run(() => new EspecialidadesDao().insertEspecialidades(reg));
            return Ok(mensaje);
        }

        [HttpPut("updateEspecialidades")]
        public async Task<ActionResult<string>> updateEspecialidades(Especialidades reg)
        {
            var mensaje = await Task.Run(() => new EspecialidadesDao().updateEspecialidades(reg));
            return Ok(mensaje);
        }

        [HttpGet("getPaciente/{id_especialidad}")]
        public async Task<ActionResult<Especialidades>> getEspecialidades(Int32 id_especialidad)
        {
            var cliente = await Task.Run(() => new EspecialidadesDao().getEspecialidades(id_especialidad));
            return Ok(cliente);
        }

        [HttpGet("getPorNombre/{nombre}")]
        public async Task<ActionResult<Especialidades>> getEspecialidades(string nombre)
        {
            var especialidad = await Task.Run(() => new EspecialidadesDao().getEspecialidades(nombre));
            return Ok(especialidad);
        }

        [HttpDelete("deleteEspecialidades/{id_especialidad}")]

        public async Task<ActionResult<string>> deleteEspecialidades(Int32 id_especialidad)
        {
            var cliente = new EspecialidadesDao().getEspecialidades(id_especialidad);
            if (cliente == null)
            {
                return NotFound("Paciente no encontrado.");
            }
            var mensaje = await Task.Run(() => new EspecialidadesDao().deleteEspecialidades(cliente));
            return Ok(mensaje); // Mensaje de éxito o error al eliminar
        }

    }
}
