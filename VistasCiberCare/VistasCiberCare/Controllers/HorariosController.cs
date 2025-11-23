using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System.Text;
using ClosedXML.Excel;
using System.IO;
using VistasCiberCare.Models;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace VistasCiberCare.Controllers
{
    public class HorariosController : Controller
    {
        public async Task<IActionResult> Index(string hora = null, int page = 1, int pageSize = 10)
        {
            List<Horarios> temporal = new List<Horarios>();
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("https://localhost:7112/api/Horarios/");

                HttpResponseMessage response;

                if (!string.IsNullOrEmpty(hora))
                {
                    response = await client.GetAsync($"getPorHora/{hora}");
                    if (response.IsSuccessStatusCode)
                    {
                        string apiResponse = await response.Content.ReadAsStringAsync();
                        var horario = JsonConvert.DeserializeObject<Horarios>(apiResponse);
                        if (horario != null)
                            temporal.Add(horario);
                    }
                }
                else
                {
                    response = await client.GetAsync("getHorario");
                    string apiResponse = await response.Content.ReadAsStringAsync();
                    temporal = JsonConvert.DeserializeObject<List<Horarios>>(apiResponse);
                }
            }

            var totalItems = temporal.Count;
            var horariosPaginados = temporal
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            ViewBag.CurrentPage = page;
            ViewBag.PageSize = pageSize;
            ViewBag.TotalItems = totalItems;
            ViewBag.HoraBusqueda = hora;

            return View(horariosPaginados);
        }

        public async Task<IActionResult> Insert()
        {
            Horarios reg = new Horarios();

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("https://localhost:7112/api/");
                HttpResponseMessage responseDoctores = await client.GetAsync("https://localhost:7112/api/Doctores/getDoctor");
                List<Doctores> doctores = new List<Doctores>();
                if (responseDoctores.IsSuccessStatusCode)
                {
                    string apiResponseDoctores = await responseDoctores.Content.ReadAsStringAsync();
                    doctores = JsonConvert.DeserializeObject<List<Doctores>>(apiResponseDoctores);
                }
                HttpResponseMessage responseEspecialidades = await client.GetAsync("https://localhost:7112/api/Especialidades/getEspecialidade");
                List<Especialidades> especialidades = new List<Especialidades>();
                if (responseEspecialidades.IsSuccessStatusCode)
                {
                    string apiResponse = await responseEspecialidades.Content.ReadAsStringAsync();
                    especialidades = JsonConvert.DeserializeObject<List<Especialidades>>(apiResponse);
                }
                var doctoresConFormato = doctores.Select(d =>
                {
                    var especialidadNombre = especialidades.FirstOrDefault(e => e.id_especialidad == d.id_especialidad)?.nombre ?? "Sin especialidad";
                    return new
                    {
                        Value = d.id_doctor,
                        Text = $"(ID: {d.id_doctor}) Dr. {d.apellido} - {especialidadNombre}"
                    };
                }).ToList();

                ViewBag.Doctores = new SelectList(doctoresConFormato, "Value", "Text", reg.id_doctor);
                var doctorSeleccionado = doctores.FirstOrDefault(d => d.id_doctor == reg.id_doctor);
                reg.apellido = doctorSeleccionado?.apellido;
                reg.id_especialidad = doctorSeleccionado?.id_especialidad ?? 0;
                ViewBag.NombreEspecialidad = especialidades.FirstOrDefault(e => e.id_especialidad == reg.id_especialidad)?.nombre ?? "No definida";
                return View(await Task.Run(() => new Horarios()));
            }
        }
        [HttpPost]
        public async Task<IActionResult> Insert(Horarios reg)
        {
            string mensaje = "";

            if (ModelState.IsValid)
            {
                using (var pacient = new HttpClient())
                {
                    pacient.BaseAddress = new Uri("https://localhost:7112/api/Horarios/");

                    StringContent content = new StringContent(JsonConvert.SerializeObject(reg), Encoding.UTF8, "application/json");

                    HttpResponseMessage response = await pacient.PostAsync("insertHorarios", content);

                    if (response.IsSuccessStatusCode)
                    {
                        string apiResponse = await response.Content.ReadAsStringAsync();
                        mensaje = "Horario insertado con éxito!";
                    }
                    else
                    {
                        mensaje = "Error al insertar el horario: " + response.ReasonPhrase;
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

            Horarios reg = new Horarios();

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("https://localhost:7112/api/Horarios/");
                HttpResponseMessage response = await client.GetAsync("getHorario/" + id);

                if (response.IsSuccessStatusCode)
                {
                    string apiResponse = await response.Content.ReadAsStringAsync();
                    reg = JsonConvert.DeserializeObject<Horarios>(apiResponse);
                }
                else
                {
                    ViewBag.mensaje = "No se pudo cargar el horario. Código: " + response.StatusCode;
                    ViewBag.tipoMensaje = "danger";
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
                    ViewBag.Especialidades = new List<SelectListItem>();
                }
            }
            using (var client = new HttpClient())
            {
                HttpResponseMessage responseDoctores = await client.GetAsync("https://localhost:7112/api/Doctores/getDoctor");

                if (responseDoctores.IsSuccessStatusCode)
                {
                    string apiResponseDoctores = await responseDoctores.Content.ReadAsStringAsync();
                    var doctores = JsonConvert.DeserializeObject<List<Doctores>>(apiResponseDoctores);

                    // Modificación: mostramos el ID y el apellido en el formato (ID: X) Dr. Apellido
                    var doctoresConFormato = doctores.Select(d => new
                    {
                        Value = d.id_doctor,
                        Text = $"(ID: {d.id_doctor}) Dr. {d.apellido}"
                    }).ToList();

                    ViewBag.Doctores = new SelectList(doctoresConFormato, "Value", "Text", reg.id_doctor);
                    reg.apellido = doctores.FirstOrDefault(d => d.id_doctor == reg.id_doctor)?.apellido;
                }
                else
                {
                    ViewBag.Doctores = new List<SelectListItem>();
                }
            }
            return View(reg);

        }
        [HttpPost]
        public async Task<IActionResult> Edit(Horarios reg)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("https://localhost:7112/api/Horarios/");
                StringContent content = new StringContent(JsonConvert.SerializeObject(reg), Encoding.UTF8, "application/json");

                HttpResponseMessage response = await client.PutAsync("updateHorarios", content);

                if (response.IsSuccessStatusCode)
                {
                    ViewBag.mensaje = "Horario actualizado exitosamente.";
                    ViewBag.tipoMensaje = "success"; 
                }
                else
                {
                    ViewBag.mensaje = "Error al actualizar el horario: " + response.ReasonPhrase;
                    ViewBag.tipoMensaje = "danger"; 
                }
            }

            return View(await Task.Run(() => reg));
        }
        public async Task<IActionResult> Delete(string id)
        {
            string mensaje = "";

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("https://localhost:7112/api/Horarios/");
                HttpResponseMessage response = await client.DeleteAsync("deleteHorarios/" + id);

                if (response.IsSuccessStatusCode)
                {
                    mensaje = "Horario eliminado correctamente.";
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

            Horarios reg = new Horarios();

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("https://localhost:7112/api/Horarios/");
                HttpResponseMessage response = await client.GetAsync("getHorario/" + id);

                if (response.IsSuccessStatusCode)
                {
                    string apiResponse = await response.Content.ReadAsStringAsync();
                    reg = JsonConvert.DeserializeObject<Horarios>(apiResponse);
                }
                else
                {
                    ViewBag.mensaje = "No se encontró el Horario. Código: " + response.StatusCode;
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
        public async Task<IActionResult> ExportarHorariosPdf()
        {
            QuestPDF.Settings.License = LicenseType.Community;

            List<Horarios> listaHorarios = new();

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("https://localhost:7112/api/Horarios/");
                HttpResponseMessage response = await client.GetAsync("getHorario");

                if (response.IsSuccessStatusCode)
                {
                    string apiResponse = await response.Content.ReadAsStringAsync();
                    listaHorarios = JsonConvert.DeserializeObject<List<Horarios>>(apiResponse);
                }
                else
                {
                    TempData["mensaje"] = "No se pudieron obtener los horarios.";
                    return RedirectToAction("Index");
                }
            }

            var logoPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "img", "CiberCareLogo.png");

            var pdf = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(30);
                    page.PageColor(Colors.White);
                    page.DefaultTextStyle(x => x.FontSize(11));

                    page.Content().Column(col =>
                    {
                        foreach (var horario in listaHorarios)
                        {
                            col.Item().PageBreak();

                            // Header
                            col.Item().Row(row =>
                            {
                                row.RelativeColumn().Column(col1 =>
                                {
                                    col1.Item().Text("CiberCare")
                                        .FontSize(20)
                                        .Bold()
                                        .FontColor(Colors.Blue.Medium);
                                    col1.Item().Text("Av. Salud N°123 - Lima");
                                    col1.Item().Text("Tel: (01) 123-4567");
                                    col1.Item().Text("citas@cibercare.com");
                                });

                                if (System.IO.File.Exists(logoPath))
                                {
                                    row.ConstantColumn(100).Height(50).Image(logoPath);
                                }
                            });

                            col.Item().PaddingVertical(10);

                            // Título
                            col.Item().Text("DETALLE DE HORARIO")
                                .FontSize(16)
                                .SemiBold()
                                .FontColor(Colors.Blue.Medium);

                            col.Item().LineHorizontal(1).LineColor(Colors.Grey.Lighten2);

                            col.Item().PaddingTop(5);

                            // Detalle de horario
                            col.Item().Grid(grid =>
                            {
                                grid.Columns(2);

                                grid.Item().Text("ID Horario:").SemiBold();
                                grid.Item().Text(horario.id_horario.ToString());

                                grid.Item().Text("Fecha:").SemiBold();
                                grid.Item().Text(horario.fecha?.ToString("dd/MM/yyyy") ?? "-");

                                grid.Item().Text("Hora:").SemiBold();
                                grid.Item().Text(horario.hora?.ToString("hh\\:mm") ?? "-");

                                grid.Item().Text("ID Doctor:").SemiBold();
                                grid.Item().Text(horario.id_doctor.ToString());

                                grid.Item().Text("Apellido Doctor:").SemiBold();
                                grid.Item().Text(horario.apellido ?? "-");

                                grid.Item().Text("Especialidad:").SemiBold();
                                grid.Item().Text(horario.id_especialidad.ToString());

                                grid.Item().Text("¿Disponible?:").SemiBold();
                                grid.Item().Text(horario.disponible == true ? "Sí" : "No");
                            });

                            col.Item().PaddingVertical(10);
                        }
                    });
                });
            });

            using var stream = new MemoryStream();
            pdf.GeneratePdf(stream);
            stream.Position = 0;

            return File(stream.ToArray(), "application/pdf", "Horarios.pdf");
        }

    }
}
