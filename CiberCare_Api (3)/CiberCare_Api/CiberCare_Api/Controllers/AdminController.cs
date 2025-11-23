using CiberCare_Api.Model;
using CiberCare_Api.Repository.DAO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;

namespace CiberCare_Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminController : ControllerBase
    {
        // GET: api/admin
        [HttpGet]
        public ActionResult<IEnumerable<Admin>> GetAdmins()
        {
            var admins = new AdminDao().GetAdmins();
            return Ok(admins);
        }

        // POST: api/admin/login
        [HttpPost("login")]
        public ActionResult Login([FromBody] Admin loginRequest)
        {
            if (loginRequest == null || string.IsNullOrWhiteSpace(loginRequest.usuario) || string.IsNullOrWhiteSpace(loginRequest.contrasena))
            {
                return BadRequest(new { mensaje = "Credenciales inválidas." });
            }

            var admin = new AdminDao().GetAdminmLogin(loginRequest.usuario, loginRequest.contrasena);

            if (admin == null)
            {
                return Unauthorized(new { mensaje = "Usuario o contraseña incorrectos." });
            }

            return Ok(new
            {
                mensaje = "Inicio de sesión exitoso.",
                usuario = new
                {
                    usuario = admin.usuario
                }
            });
        }
    }
}
