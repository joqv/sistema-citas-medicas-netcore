using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
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
            _httpClient.BaseAddress = new Uri( "https://localhost:7112/api/Admin/");
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
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
                    string apiResponse = await response.Content.ReadAsStringAsync();

                    using (JsonDocument doc = JsonDocument.Parse(apiResponse))
                    {
                        if (doc.RootElement.TryGetProperty("usuario", out JsonElement usuarioElement))
                        {
                            if (usuarioElement.TryGetProperty("usuario", out JsonElement nombreUsuarioElement))
                            {
                                string nombreUsuario = nombreUsuarioElement.GetString();

                                // 1. ALMACENAR EN SESIÓN (Para mostrar en el header, ya lo tienes)
                                HttpContext.Session.SetString("UsuarioLogeado", nombreUsuario);

                                // 2. CREAR CLAIMS (Identidad)
                                var claims = new List<Claim>
                                {
                                    // Añadimos el nombre de usuario como ClaimType.Name
                                    new Claim(ClaimTypes.Name, nombreUsuario), 
                                    // Si tuvieras roles, podrías añadir: new Claim(ClaimTypes.Role, "Admin")
                                };

                                var claimsIdentity = new ClaimsIdentity(claims, "CookieAuth");
                                var authProperties = new AuthenticationProperties();

                                // 3. FIRMAR EL TICKET DE AUTENTICACIÓN (Autenticar al usuario)
                                await HttpContext.SignInAsync("CookieAuth", new ClaimsPrincipal(claimsIdentity), authProperties);
                            }
                        }
                    }


                    TempData["mensaje"] = "Inicio de sesión exitoso.";

                    //return Redirect("https://localhost:7069/"); // Redirige a tu otro proyecto
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    string error = await response.Content.ReadAsStringAsync();
                    ModelState.AddModelError(string.Empty, "Credenciales incorrectas.");
                    return View(admin);
                }
            }
        }

        [HttpGet]
        public async Task<IActionResult> LogoutAsync()
        {
            // 1. Limpiar toda la información de sesión
            HttpContext.Session.Clear();

            TempData["mensaje"] = null;

            await HttpContext.SignOutAsync("CookieAuth");

            // 2. Redirigir a la página de login (o Home, según prefieras)
            return RedirectToAction("Index", "Admin");
        }
    }
}
