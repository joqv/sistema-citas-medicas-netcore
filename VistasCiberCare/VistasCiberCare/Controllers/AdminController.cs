using Microsoft.AspNetCore.Mvc;
using System.Text;
using System.Text.Json;
using VistasCiberCare.Models;

namespace VistasCiberCare.Controllers
{
    public class AdminController : Controller
    {
        private readonly HttpClient _httpClient;

        public AdminController()
        {
            _httpClient = new HttpClient();
            _httpClient.BaseAddress = new Uri( "https://localhost:7112/api/Admin/"); // ✅ CORRECTO
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Index(Admin admin)
        {
            if (!ModelState.IsValid)
            {
                return View(admin);
            }

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("https://localhost:7112/api/Admin/");
                StringContent content = new StringContent(JsonSerializer.Serialize(admin), Encoding.UTF8, "application/json");

                HttpResponseMessage response = await client.PostAsync("login", content);

                if (response.IsSuccessStatusCode)
                {
                    TempData["mensaje"] = "Inicio de sesión exitoso.";
                    return Redirect("https://localhost:7069/"); // Redirige a tu otro proyecto
                }
                else
                {
                    string error = await response.Content.ReadAsStringAsync();
                    ModelState.AddModelError(string.Empty, "Credenciales incorrectas.");
                    return View(admin);
                }
            }
        }


    }
}
