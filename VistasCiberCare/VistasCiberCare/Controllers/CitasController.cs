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
using Microsoft.AspNetCore.Authorization;

namespace VistasCiberCare.Controllers
{
    [Authorize]
    public class CitasController : Controller
    {
        public async Task<IActionResult> Index(string dni = null, int page = 1, int pageSize = 10)
        {
            List<Citas> temporal = new List<Citas>();

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("https://localhost:7112/api/Citas/");

                HttpResponseMessage response;

                if (!string.IsNullOrEmpty(dni))
                {
                    response = await client.GetAsync($"getCitas/{dni}");
                    if (response.IsSuccessStatusCode)
                    {
                        string apiResponse = await response.Content.ReadAsStringAsync();
                        var cita = JsonConvert.DeserializeObject<Citas>(apiResponse);
                        if (cita != null)
                            temporal.Add(cita);
                    }
                }
                else
                {
                    response = await client.GetAsync("getCitas");
                    if (response.IsSuccessStatusCode)
                    {
                        string apiResponse = await response.Content.ReadAsStringAsync();
                        temporal = JsonConvert.DeserializeObject<List<Citas>>(apiResponse);
                    }
                }
            }

            var totalItems = temporal.Count;
            var citasPaginadas = temporal
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            ViewBag.CurrentPage = page;
            ViewBag.PageSize = pageSize;
            ViewBag.TotalItems = totalItems;
            ViewBag.DniBusqueda = dni;

            return View(citasPaginadas);
        }


        public async Task<IActionResult> Insert()
        {
            await CargarCombos();
            return View(new Citas());
        }

        [HttpPost]
        public async Task<IActionResult> Insert(Citas reg)
        {
            string mensaje = "";

            if (ModelState.IsValid)
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri("https://localhost:7112/api/Citas/");

                    StringContent content = new StringContent(JsonConvert.SerializeObject(reg), Encoding.UTF8, "application/json");
                    HttpResponseMessage response = await client.PostAsync("insertCitas", content);

                    if (response.IsSuccessStatusCode)
                    {
                        mensaje = "¡Cita registrada con éxito!";
                        ViewBag.mensaje = mensaje;
                        await CargarCombos();
                        return View(new Citas());
                    }
                    else
                    {
                        mensaje = "Error al registrar cita: " + response.ReasonPhrase;
                    }
                }
            }
            else
            {
                mensaje = "Datos inválidos.";
            }

            ViewBag.mensaje = mensaje;
            await CargarCombos();
            return View(reg);
        }

        private async Task CargarCombos()
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("https://localhost:7112/api/");
                var responsePacientes = await client.GetAsync("Paciente/getPacientes");
                List<Pacientes> pacientes = new();
                if (responsePacientes.IsSuccessStatusCode)
                {
                    string apiResponse = await responsePacientes.Content.ReadAsStringAsync();
                    pacientes = JsonConvert.DeserializeObject<List<Pacientes>>(apiResponse);
                }

                var pacientesDni = pacientes.Select(p => new
                {
                    Value = p.dni,
                    Text = $"DNI: {p.dni}",
                    Nombre = p.nombre
                }).ToList();
                ViewBag.Pacientes = pacientesDni;
                var responseHorarios = await client.GetAsync("Horarios/getHorario");
                var responseDoctores = await client.GetAsync("Doctores/getDoctor");
                var responseEspecialidades = await client.GetAsync("Especialidades/getEspecialidade");
                List<Horarios> horarios = new();
                List<Doctores> doctores = new();
                List<Especialidades> especialidades = new();
                if (responseHorarios.IsSuccessStatusCode)
                {
                    string apiResponse = await responseHorarios.Content.ReadAsStringAsync();
                    horarios = JsonConvert.DeserializeObject<List<Horarios>>(apiResponse);
                }
                if (responseDoctores.IsSuccessStatusCode)
                {
                    string apiResponse = await responseDoctores.Content.ReadAsStringAsync();
                    doctores = JsonConvert.DeserializeObject<List<Doctores>>(apiResponse);
                }
                if (responseEspecialidades.IsSuccessStatusCode)
                {
                    string apiResponse = await responseEspecialidades.Content.ReadAsStringAsync();
                    especialidades = JsonConvert.DeserializeObject<List<Especialidades>>(apiResponse);
                }
                var horariosInfo = horarios.Select(h =>
                {
                    var doctor = doctores.FirstOrDefault(d => d.id_doctor == h.id_doctor);
                    var especialidad = especialidades.FirstOrDefault(e => e.id_especialidad == doctor?.id_especialidad)?.nombre ?? "Sin especialidad";
                    return new
                    {
                        Value = h.id_horario,
                        Text = $"Horario {h.id_horario}: {h.fecha} {h.hora} - Dr. {doctor?.apellido} ({especialidad})",
                        Fecha = h.fecha,
                        Hora = h.hora,
                        Doctor = doctor?.apellido,
                        Especialidad = especialidad
                    };
                }).ToList();

                ViewBag.Horarios = horariosInfo;
            }
        }
        public async Task<IActionResult> Edit(string dni)
        {
            if (string.IsNullOrEmpty(dni)) return RedirectToAction("Index");

            Citas reg = new();

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("https://localhost:7112/api/");
                HttpResponseMessage response = await client.GetAsync("Citas/getCitas/" + dni);

                if (response.IsSuccessStatusCode)
                {
                    string apiResponse = await response.Content.ReadAsStringAsync();
                    reg = JsonConvert.DeserializeObject<Citas>(apiResponse);
                }
                else
                {
                    TempData["mensaje"] = "No se pudo cargar la cita";
                    return RedirectToAction("Index");
                }
            }

            await CargarCombos();
            return View(reg);
        }


        [HttpPost]
        public async Task<IActionResult> Edit(Citas reg)
        {
            string mensaje = "";

            if (ModelState.IsValid)
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri("https://localhost:7112/api/Citas/");

                    StringContent content = new StringContent(JsonConvert.SerializeObject(reg), Encoding.UTF8, "application/json");
                    HttpResponseMessage response = await client.PutAsync("updateCitas", content);

                    if (response.IsSuccessStatusCode)
                    {
                        mensaje = "¡Cita actualizada con éxito!";
                        ViewBag.mensaje = mensaje;
                        await CargarCombos();
                        return View(reg);
                    }
                    else
                    {
                        mensaje = "Error al actualizar la cita: " + response.ReasonPhrase;
                    }
                }
            }
            else
            {
                mensaje = "Datos inválidos.";
            }

            ViewBag.mensaje = mensaje;
            await CargarCombos();
            return View(reg);
        }

        public async Task<IActionResult> Details(string dni)
        {
            if (string.IsNullOrEmpty(dni)) return RedirectToAction("Index");

            Citas cita = new();

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("https://localhost:7112/api/Citas/");
                HttpResponseMessage response = await client.GetAsync("getCitas/" + dni);

                if (response.IsSuccessStatusCode)
                {
                    string apiResponse = await response.Content.ReadAsStringAsync();
                    cita = JsonConvert.DeserializeObject<Citas>(apiResponse);
                }
                else
                {
                    TempData["mensaje"] = "No se pudo cargar la cita";
                    return RedirectToAction("Index");
                }
            }

            return View(cita);
        }


        public async Task<IActionResult> Delete(int id_cita)
        {
            string mensaje = "";

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("https://localhost:7112/api/Citas/");
                HttpResponseMessage response = await client.DeleteAsync("deleteCitas/" + id_cita);

                if (response.IsSuccessStatusCode)
                {
                    TempData["mensaje"] = "Cita eliminada con éxito";
                }
                else
                {
                    string errorMessage = await response.Content.ReadAsStringAsync();
                    TempData["mensaje"] = "Error al eliminar la cita: " + errorMessage;

                }
            }
            TempData["mensaje"] = mensaje;
            return RedirectToAction("Index");
        }
        [HttpGet]
        public async Task<JsonResult> GetTopHorarios()
        {
            List<Citas> citas = new();

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("https://localhost:7112/api/Citas/");
                var response = await client.GetAsync("getCitas");

                if (response.IsSuccessStatusCode)
                {
                    string apiResponse = await response.Content.ReadAsStringAsync();
                    citas = JsonConvert.DeserializeObject<List<Citas>>(apiResponse);
                }
                else
                {
                    // Puedes manejar errores aquí si deseas
                    return Json(new List<object>()); // Devuelve lista vacía si falla
                }
            }

            var datos = citas
                .GroupBy(c => c.hora)
                .Select(g => new
                {
                    Hora = g.Key,
                    Cantidad = g.Count()
                })
                .OrderByDescending(g => g.Cantidad)
                .Take(5)
                .ToList();

            return Json(datos);
        }


        public async Task<IActionResult> ExportarExcel()
        {
            List<Citas> listaCitas = new List<Citas>();

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("https://localhost:7112/api/Citas/");
                HttpResponseMessage response = await client.GetAsync("getCitas");
                if (response.IsSuccessStatusCode)
                {
                    string apiResponse = await response.Content.ReadAsStringAsync();
                    listaCitas = JsonConvert.DeserializeObject<List<Citas>>(apiResponse);
                }
            }

            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("Citas");

                // Fila 1 
                worksheet.Cell(1, 1).Value = "LISTA DE CITAS";
                worksheet.Range(1, 1, 1, 7).Merge();
                worksheet.Row(1).Height = 30;
                worksheet.Cell(1, 1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                worksheet.Cell(1, 1).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                worksheet.Cell(1, 1).Style.Font.Bold = true;
                worksheet.Cell(1, 1).Style.Font.FontSize = 16;
                worksheet.Cell(1, 1).Style.Fill.BackgroundColor = XLColor.Purple;
                worksheet.Cell(1, 1).Style.Font.FontColor = XLColor.White;

                // Fila 2
                worksheet.Cell(2, 1).Value = "Id de cita";
                worksheet.Cell(2, 2).Value = "Dni del Paciente";
                worksheet.Cell(2, 3).Value = "Nombre del Paciente";
                worksheet.Cell(2, 4).Value = "Fecha";
                worksheet.Cell(2, 5).Value = "Hora";
                worksheet.Cell(2, 6).Value = "Nombre del Doctor";
                worksheet.Cell(2, 7).Value = "Especialidad";


                for (int col = 1; col <= 7; col++)
                {
                    worksheet.Cell(2, col).Style.Font.Bold = true;
                    worksheet.Cell(2, col).Style.Fill.BackgroundColor = XLColor.LightGray; // Opcional
                    worksheet.Cell(2, col).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                }
                worksheet.Cell(2, 7).Value = "Especialidad";



                // Fila 3
                var currentRow = 3;
                foreach (var cita in listaCitas)
                {
                    worksheet.Cell(currentRow, 1).Value = cita.id_cita;
                    worksheet.Cell(currentRow, 2).Value = cita.dni_paciente;
                    worksheet.Cell(currentRow, 3).Value = cita.nombre_paciente;
                    worksheet.Cell(currentRow, 4).Value = cita.fecha.ToString("dd/MM/yyyy");
                    worksheet.Cell(currentRow, 5).Value = cita.hora;
                    worksheet.Cell(currentRow, 6).Value = cita.nombre_doctor;
                    worksheet.Cell(currentRow, 7).Value = cita.especialidad;
                    currentRow++;
                }
                worksheet.Columns().AdjustToContents();

                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    var content = stream.ToArray();

                    return File(content,
                                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                                "ReporteCitas.xlsx");
                }
            }

        }



        public async Task<IActionResult> ExportarCitaPdf(string dni)
        {
            QuestPDF.Settings.License = QuestPDF.Infrastructure.LicenseType.Community;

            if (string.IsNullOrEmpty(dni))
                return RedirectToAction("Index");

            Citas cita = new();

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("https://localhost:7112/api/Citas/");
                HttpResponseMessage response = await client.GetAsync("getCitas/" + dni);

                if (response.IsSuccessStatusCode)
                {
                    string apiResponse = await response.Content.ReadAsStringAsync();
                    cita = JsonConvert.DeserializeObject<Citas>(apiResponse);
                }
                else
                {
                    TempData["mensaje"] = "No se pudo cargar la cita";
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
                    page.DefaultTextStyle(x => x.FontSize(12));

                    // ENCABEZADO
                    page.Header().Row(row =>
                    {
                        // IZQUIERDA: LOGO Y DATOS
                        row.RelativeColumn().Column(col =>
                        {
                            col.Item().Text("CiberCare").FontSize(16).SemiBold().FontColor(Colors.Blue.Medium);
                            col.Item().Text("Av. Salud N°123 - Lima");
                            col.Item().Text("Tel: (01) 123-4567");
                            col.Item().Text("citas@cibercare.com");
                        });

                        // DERECHA: RUC y código boleta
                        row.ConstantColumn(200).Column(col =>
                        {
                            col.Item().Border(1).Padding(5).AlignCenter().Text("RUC: 209933350023").SemiBold();
                            col.Item().Border(1).Padding(5).AlignCenter().Text("Boleta de Cita");
                            col.Item().Border(1).Padding(5).AlignCenter().Text("B0001 - " + cita.id_cita);
                        });
                    });

                    // CUERPO DEL REPORTE
                    page.Content().Column(col =>
                    {
                        col.Item().PaddingVertical(10).Text("Datos del Paciente").Bold().FontSize(14);

                        col.Item().Table(table =>
                        {
                            table.ColumnsDefinition(columns =>
                            {
                                columns.ConstantColumn(150);
                                columns.RelativeColumn();
                            });

                            table.Cell().Element(CellStyle).Text("Nombre:");
                            table.Cell().Element(CellStyle).Text(cita.nombre_paciente);

                            table.Cell().Element(CellStyle).Text("DNI:");
                            table.Cell().Element(CellStyle).Text(cita.dni_paciente);

                            table.Cell().Element(CellStyle).Text("Dirección:");
                            table.Cell().Element(CellStyle).Text("Av. Miraflores 123"); // Si tienes dirección real, úsala
                        });

                        col.Item().PaddingVertical(10).Text("Detalle de la Cita").Bold().FontSize(14);

                        col.Item().Table(table =>
                        {
                            table.ColumnsDefinition(columns =>
                            {
                                columns.ConstantColumn(150);
                                columns.RelativeColumn();
                            });

                            table.Cell().Element(CellStyle).Text("Fecha:");
                            table.Cell().Element(CellStyle).Text(cita.fecha.ToString("dd/MM/yyyy"));

                            table.Cell().Element(CellStyle).Text("Hora:");
                            table.Cell().Element(CellStyle).Text(cita.hora.ToString());

                            table.Cell().Element(CellStyle).Text("Doctor:");
                            table.Cell().Element(CellStyle).Text(cita.nombre_doctor);

                            table.Cell().Element(CellStyle).Text("Especialidad:");
                            table.Cell().Element(CellStyle).Text(cita.especialidad);
                        });
                    });

                    // Estilo de celdas
                    IContainer CellStyle(IContainer container) =>
                        container.Padding(2).BorderBottom(1).BorderColor(Colors.Grey.Lighten2);
                });
            });

            using var stream = new MemoryStream();
            pdf.GeneratePdf(stream);
            stream.Position = 0;

            return File(stream.ToArray(), "application/pdf", $"Cita_{dni}.pdf");
        }

        public async Task<IActionResult> ExportarPdf()

        {
            QuestPDF.Settings.License = QuestPDF.Infrastructure.LicenseType.Community;

            List<Citas> listaCitas = new List<Citas>();

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("https://localhost:7112/api/Citas/");
                HttpResponseMessage response = await client.GetAsync("getCitas");
                if (response.IsSuccessStatusCode)
                {
                    string apiResponse = await response.Content.ReadAsStringAsync();
                    listaCitas = JsonConvert.DeserializeObject<List<Citas>>(apiResponse);
                }
            }
            var logoPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "img", "CiberCareLogo.png");
            var pdf = Document.Create(container =>
            {

                container.Page(page =>
                {
                    page.Margin(30);
                    page.Size(PageSizes.A4);
                    page.PageColor(Colors.White);
                    page.DefaultTextStyle(x => x.FontSize(12));

                    page.Content().Column(col =>
                    {


                        foreach (var cita in listaCitas)
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
                            col.Item().Text("DETALLE DE CITA")
                                .FontSize(18)
                                .SemiBold()
                                .FontColor(Colors.Blue.Medium);

                            col.Item().LineHorizontal(1).LineColor(Colors.Grey.Lighten2);

                            
                            col.Item().Grid(grid =>
                            {
                                grid.Columns(2);

                                grid.Item().Text("ID Cita:").SemiBold();
                                grid.Item().Text(cita.id_cita.ToString());

                                grid.Item().Text("DNI Paciente:").SemiBold();
                                grid.Item().Text(cita.dni_paciente);

                                grid.Item().Text("Nombre Paciente:").SemiBold();
                                grid.Item().Text(cita.nombre_paciente);

                                grid.Item().Text("ID Horario:").SemiBold();
                                grid.Item().Text(cita.id_horario.ToString());

                                grid.Item().Text("Fecha:").SemiBold();
                                grid.Item().Text(cita.fecha.ToString("dd/MM/yyyy"));

                                grid.Item().Text("Hora:").SemiBold();
                                grid.Item().Text(cita.hora.ToString());

                                grid.Item().Text("Nombre Doctor:").SemiBold();
                                grid.Item().Text(cita.nombre_doctor);

                                grid.Item().Text("Especialidad:").SemiBold();
                                grid.Item().Text(cita.especialidad);
                            });

                            col.Item().PaddingVertical(10);
                        }
                    });
                });
            });


            using var stream = new MemoryStream();
            pdf.GeneratePdf(stream);
            stream.Position = 0;

            return File(stream.ToArray(), "application/pdf", "Citas.pdf");
        }
    }
}
