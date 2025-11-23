using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using VistasCiberCare.Models;

namespace VistasCiberCare.Controllers
{
    public class LoginController : Controller
    {
        private readonly HttpClient _httpClient;

        public LoginController()
        {
            _httpClient = new HttpClient();
            _httpClient.BaseAddress = new Uri("https://localhost:7112/api/Paciente/login"); // Cambia el puerto si tu API usa otro
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(Pacientes paciente)
        {
            if (!ModelState.IsValid)
                return View(paciente);

            var content = new StringContent(JsonSerializer.Serialize(paciente), Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync("login", content);

            if (response.IsSuccessStatusCode)
            {
                var responseData = await response.Content.ReadAsStringAsync();
                TempData["mensaje"] = "Bienvenido, sesión iniciada correctamente.";
                return RedirectToAction("Bienvenida");
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Correo o contraseña incorrectos.");
                return View(paciente);
            }
        }

        public IActionResult Bienvenida()
        {
            return View();
        }
    }
}
