using Humanizer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using System.Text;
using VistasCiberCare.Models;

namespace VistasCiberCare.Controllers
{
    [Authorize]
    public class DoctoresController : Controller
    {

        public async Task<IActionResult> Index(string nombre = null, int page = 1, int pageSize = 10)
        {
            List<DoctoresDto> temporal = new List<DoctoresDto>();

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("https://localhost:7112/api/Doctores/");

                HttpResponseMessage response;

                if (!string.IsNullOrEmpty(nombre))
                {
                    response = await client.GetAsync($"getPorNombre/{nombre}");
                    if (response.IsSuccessStatusCode)
                    {
                        string apiResponse = await response.Content.ReadAsStringAsync();
                        var doctor = JsonConvert.DeserializeObject<DoctoresDto>(apiResponse);
                        if (doctor != null)
                        {
                            temporal.Add(doctor);
                        }
                    }
                }
                else
                {
                    response = await client.GetAsync("getDoctor");
                    string apiResponse = await response.Content.ReadAsStringAsync();
                    temporal = JsonConvert.DeserializeObject<List<DoctoresDto>>(apiResponse);
                }
            }
            var totalItems = temporal.Count;
            var doctoresPaginados = temporal
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            ViewBag.CurrentPage = page;
            ViewBag.PageSize = pageSize;
            ViewBag.TotalItems = totalItems;
            ViewBag.NombreBusqueda = nombre;

            return View(doctoresPaginados);
        }


        public async Task<IActionResult> Insert()

        {
            Doctores reg = new Doctores();

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("https://localhost:7112/api/Especialidades/");
                HttpResponseMessage response = await client.GetAsync("getEspecialidade");

                if (response.IsSuccessStatusCode)
                {
                    string apiResponse = await response.Content.ReadAsStringAsync();
                    var especialidades = JsonConvert.DeserializeObject<List<Especialidades>>(apiResponse);
                    ViewBag.Especialidades = new SelectList(especialidades, "id_especialidad", "nombre", reg.id_especialidad);
                }
                else
                {
                    ViewBag.Especialidades = new List<SelectListItem>(); 
                }
            }
            return View(await Task.Run(() => new DoctoresDto()));
        }
        [HttpPost]
        public async Task<IActionResult> Insert(DoctoresDto reg)
        {
            string mensaje = "";

            if (ModelState.IsValid)
            {
                using (var pacient = new HttpClient())
                {
                    pacient.BaseAddress = new Uri("https://localhost:7112/api/Doctores/");

                    StringContent content = new StringContent(JsonConvert.SerializeObject(reg), Encoding.UTF8, "application/json");

                    HttpResponseMessage response = await pacient.PostAsync("insertDoctores", content);

                    if (response.IsSuccessStatusCode)
                    {
                        string apiResponse = await response.Content.ReadAsStringAsync();
                        mensaje = "Doctor insertado con éxito!";
                    }
                    else
                    {
                        mensaje = "Error al insertar el Doctor: " + response.ReasonPhrase;
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

            Doctores reg = new Doctores();

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("https://localhost:7112/api/Doctores/");
                HttpResponseMessage response = await client.GetAsync("getPaciente/" + id);

                if (response.IsSuccessStatusCode)
                {
                    string apiResponse = await response.Content.ReadAsStringAsync();
                    reg = JsonConvert.DeserializeObject<Doctores>(apiResponse);
                }
                else
                {
                    ViewBag.mensaje = "No se pudo cargar el Id_Doctores. Doctores: " + response.StatusCode;
                }
            }
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("https://localhost:7112/api/Especialidades/");
                HttpResponseMessage response = await client.GetAsync("getEspecialidade");

                if (response.IsSuccessStatusCode)
                {
                    string apiResponse = await response.Content.ReadAsStringAsync();
                    var especialidades = JsonConvert.DeserializeObject<List<Especialidades>>(apiResponse);
                    ViewBag.Especialidades = new SelectList(especialidades, "id_especialidad", "nombre", reg.id_especialidad);
                }
                else
                {
                    ViewBag.Especialidades = new List<SelectListItem>(); // o un mensaje de error
                }
            }

            return View(reg);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(Doctores reg)
        {
            string mensaje = "";
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("https://localhost:7112/api/Doctores/updateDoctores");
                StringContent content = new StringContent(JsonConvert.SerializeObject(reg), Encoding.UTF8, "application/json");
                HttpResponseMessage response = await client.PutAsync("updateDoctores", content);
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
                //SEAGREGO*************
                client.BaseAddress = new Uri("https://localhost:7112/api/Doctores/");
                HttpResponseMessage response = await client.DeleteAsync("deleteDoctores/" + id);
                string respuestaBackend = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    mensaje = respuestaBackend;//"Doctor eliminado correctamente.";
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

            Doctores reg = new Doctores();
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("https://localhost:7112/api/Doctores/");
                HttpResponseMessage response = await client.GetAsync("getPaciente/" + id);

                if (response.IsSuccessStatusCode)
                {
                    string apiResponse = await response.Content.ReadAsStringAsync();
                    reg = JsonConvert.DeserializeObject<Doctores>(apiResponse);
                }
                else
                {
                    ViewBag.mensaje = "No se encontró el Doctor. Código: " + response.StatusCode;
                    return RedirectToAction("Index");
                }
            }
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("https://localhost:7112/api/Especialidades/");
                HttpResponseMessage response = await client.GetAsync("getPaciente/" + reg.id_especialidad);

                if (response.IsSuccessStatusCode)
                {
                    string apiResponse = await response.Content.ReadAsStringAsync();
                    var especialidad = JsonConvert.DeserializeObject<Especialidades>(apiResponse);

                    ViewBag.NombreEspecialidad = especialidad.nombre;
                }
                else
                {
                    ViewBag.NombreEspecialidad = "Especialidad no encontrada";
                }
            }
            return View(reg);
        }
    }
}
