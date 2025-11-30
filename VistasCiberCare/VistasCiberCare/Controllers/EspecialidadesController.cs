using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Text;
using VistasCiberCare.Models;

namespace VistasCiberCare.Controllers
{
    [Authorize]
    public class EspecialidadesController : Controller
    {
        public async Task<IActionResult> Index(int page = 1, int pageSize = 10)
        {
            List<Especialidades> temporal = new List<Especialidades>();
            using (var pacient = new HttpClient())
            {
                pacient.BaseAddress = new Uri("https://localhost:7112/api/Especialidades/getEspecialidade");
                HttpResponseMessage response = await pacient.GetAsync("getEspecialidade");
                String apiResponse = await response.Content.ReadAsStringAsync();
                temporal = JsonConvert.DeserializeObject<List<Especialidades>>(apiResponse).ToList();

            }
            var totalItems = temporal.Count;
            var EspecialidadesPaginados = temporal
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            ViewBag.CurrentPage = page;
            ViewBag.PageSize = pageSize;
            ViewBag.TotalItems = totalItems;

            return View(EspecialidadesPaginados);
        }
        public async Task<IActionResult> Insert()
        {
            return View(await Task.Run(() => new Especialidades()));
        }
        [HttpPost]
        public async Task<IActionResult> Insert(Especialidades reg)
        {
            string mensaje = "";

            if (ModelState.IsValid)
            {
                using (var pacient = new HttpClient())
                {
                    pacient.BaseAddress = new Uri("https://localhost:7112/api/Especialidades/");

                    StringContent content = new StringContent(JsonConvert.SerializeObject(reg), Encoding.UTF8, "application/json");

                    HttpResponseMessage response = await pacient.PostAsync("insertEspecialidades", content);

                    if (response.IsSuccessStatusCode)
                    {
                        string apiResponse = await response.Content.ReadAsStringAsync();
                        mensaje = "Paciente insertado con éxito!";
                    }
                    else
                    {
                        mensaje = "Error al insertar el paciente: " + response.ReasonPhrase;
                    }
                }
            }
            else
            {
                mensaje = "Los datos proporcionados no son válidos.";
            }

            ViewBag.mensaje = mensaje;

            return View(await Task.Run(() => reg));
        }
        public async Task<IActionResult> Edit(string id)
        {
            if (string.IsNullOrEmpty(id)) return RedirectToAction("Index");

            Especialidades reg = new Especialidades();
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("https://localhost:7112/api/Especialidades/");
                HttpResponseMessage response = await client.GetAsync("getPaciente/" + id);

                if (response.IsSuccessStatusCode)
                {
                    string apiResponse = await response.Content.ReadAsStringAsync();
                    reg = JsonConvert.DeserializeObject<Especialidades>(apiResponse);
                }
                else
                {
                    ViewBag.mensaje = "No se pudo cargar el paciente. Código: " + response.StatusCode;
                }
            }

            return View(await Task.Run(() => reg));
        }
        [HttpPost]
        public async Task<IActionResult> Edit(Especialidades reg)
        {
            string mensaje = "";
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("https://localhost:7112/api/Especialidades/updateEspecialidades");
                StringContent content = new StringContent(JsonConvert.SerializeObject(reg), Encoding.UTF8, "application/json");
                HttpResponseMessage response = await client.PutAsync("updateEspecialidades", content);
                string apiResponse = await response.Content.ReadAsStringAsync();
                mensaje = apiResponse;
            }
            ViewBag.mensaje = mensaje;
            return View(await Task.Run(() => reg));
        }


        public async Task<IActionResult> Delete(string id)
        {
            string mensaje = "";

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("https://localhost:7112/api/Especialidades/");
                HttpResponseMessage response = await client.DeleteAsync("deleteEspecialidades/" + id);

                if (response.IsSuccessStatusCode)
                {
                    mensaje = "Paciente eliminado correctamente.";
                }
                else
                {
                    mensaje = "Error al eliminar: " + response.ReasonPhrase;
                }
            }

            TempData["mensaje"] = mensaje;
            return RedirectToAction("Index");
        }


        public async Task<IActionResult> Details(string id)
        {
            if (string.IsNullOrEmpty(id)) return RedirectToAction("Index");

            Especialidades reg = new Especialidades();
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("https://localhost:7112/api/Especialidades/");
                HttpResponseMessage response = await client.GetAsync("getPaciente/" + id);

                if (response.IsSuccessStatusCode)
                {
                    string apiResponse = await response.Content.ReadAsStringAsync();
                    reg = JsonConvert.DeserializeObject<Especialidades>(apiResponse);
                }
                else
                {
                    ViewBag.mensaje = "No se encontró el paciente. Código: " + response.StatusCode;
                    return RedirectToAction("Index");
                }
            }

            return View(reg);
        }
    }
}
