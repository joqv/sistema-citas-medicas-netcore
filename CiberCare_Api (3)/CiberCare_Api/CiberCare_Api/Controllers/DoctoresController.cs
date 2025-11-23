using CiberCare_Api.Repository.DAO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PrimerApi.Model;
using PrimerApi.Repository.DAO;

namespace CiberCare_Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DoctoresController : ControllerBase
    {
        [HttpGet("getDoctor")]
        public async Task<ActionResult<List<Doctores>>> getDoctor()
        {
            var lista = await Task.Run(() => new DoctoresDao().getDoctor());
            return Ok(lista);
        }
    

     [HttpPost("insertDoctores")]
        public async Task<ActionResult<string>> insertDoctores(Doctores reg)
        {
            var mensaje = await Task.Run(() => new DoctoresDao().insertDoctores(reg));
            return Ok(mensaje);
        }

        [HttpPut("updateDoctores")]
        public async Task<ActionResult<string>> updateDoctores(Doctores reg)
        {
            var mensaje = await Task.Run(() => new DoctoresDao().updateDoctores(reg));
            return Ok(mensaje);
        }

        [HttpGet("getPaciente/{id_doctor}")]
        public async Task<ActionResult<Especialidades>> getDoctores(Int32 id_doctor)
        {
            var cliente = await Task.Run(() => new DoctoresDao().getDoctores(id_doctor));
            return Ok(cliente);
        }

        [HttpGet("getPorNombre/{nombre}")]
        public async Task<ActionResult<Doctores>> getDoctor(string nombre)
        {
            var doctor = await Task.Run(() => new DoctoresDao().getDoctor(nombre));
            return Ok(doctor);
        }

        [HttpDelete("deleteDoctores/{id_doctor}")]

        public async Task<ActionResult<string>> deleteDoctores(Int32 id_doctor)
        {
            var cliente = new DoctoresDao().getDoctores(id_doctor);
            if (cliente == null)
            {
                return NotFound("Paciente no encontrado.");
            }
            var mensaje = await Task.Run(() => new DoctoresDao().deleteDoctores(cliente));
            return Ok(mensaje); 
        }

    }
}
