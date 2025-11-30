using ClosedXML.Excel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using System.IO;
using System.Text;
using VistasCiberCare.Models;

namespace VistasCiberCare.Controllers
{
    [Authorize]
    public class PacientesController : Controller
    {
        public async Task<IActionResult> Index(string dni = null, int page = 1, int pageSize = 10)
        {
            List<Pacientes> temporal = new List<Pacientes>();

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("https://localhost:7112/api/Paciente/");

                HttpResponseMessage response;

                if (!string.IsNullOrEmpty(dni))
                {
                    response = await client.GetAsync($"getPaciente/{dni}");
                    if (response.IsSuccessStatusCode)
                    {
                        string apiResponse = await response.Content.ReadAsStringAsync();
                        var paciente = JsonConvert.DeserializeObject<Pacientes>(apiResponse);
                        if (paciente != null)
                        {
                            temporal.Add(paciente);
                        }
                    }
                }
                else
                {
                    response = await client.GetAsync("getPacientes");
                    string apiResponse = await response.Content.ReadAsStringAsync();
                    temporal = JsonConvert.DeserializeObject<List<Pacientes>>(apiResponse);
                }
            }
            var totalItems = temporal.Count;
            var PacientesPaginados = temporal
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            ViewBag.CurrentPage = page;
            ViewBag.PageSize = pageSize;
            ViewBag.TotalItems = totalItems;
            ViewBag.DniBusqueda = dni;

            return View(PacientesPaginados);
        }

        public async Task<IActionResult> Insert()
        {
            return View(await Task.Run(() => new Pacientes()));
        }
        [HttpPost]
        public async Task<IActionResult> Insert(Pacientes reg)
        {
            string mensaje = "";

            if (ModelState.IsValid)
            {
                using (var pacient = new HttpClient())
                {
                    pacient.BaseAddress = new Uri("https://localhost:7112/api/Paciente/");

                    StringContent content = new StringContent(JsonConvert.SerializeObject(reg), Encoding.UTF8, "application/json");

                    HttpResponseMessage response = await pacient.PostAsync("insertPacientes", content);

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
            return View(reg);
        }

        public async Task<IActionResult> Edit(string id)
        {
            if (string.IsNullOrEmpty(id)) return RedirectToAction("Index");

            Pacientes reg = new Pacientes();
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("https://localhost:7112/api/Paciente/");
                HttpResponseMessage response = await client.GetAsync("getPaciente/" + id);

                if (response.IsSuccessStatusCode)
                {
                    string apiResponse = await response.Content.ReadAsStringAsync();
                    reg = JsonConvert.DeserializeObject<Pacientes>(apiResponse);
                }
                else
                {
                    ViewBag.mensaje = "No se pudo cargar el paciente. Código: " + response.StatusCode;
                }
            }
            ViewBag.mensaje = TempData["mensaje"]; 
            return View(reg); 
        }

        [HttpPost]
        public async Task<IActionResult> Edit(Pacientes reg)
        {
                string mensaje = "";
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri("https://localhost:7112/api/Paciente/");
                    StringContent content = new StringContent(JsonConvert.SerializeObject(reg), Encoding.UTF8, "application/json");
                    HttpResponseMessage response = await client.PutAsync("updatePacientes", content);

                    if (response.IsSuccessStatusCode)
                    {
                        mensaje = "Paciente actualizado con éxito.";
                    }
                    else
                    {
                        mensaje = "Error al actualizar el paciente: " + response.ReasonPhrase;
                    }
                }
                ViewBag.mensaje = mensaje;
            return View(await Task.Run(() => reg));
        }


        public async Task<IActionResult> Delete(string id)
        {
            string mensaje = "";

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("https://localhost:7112/api/Paciente/");
                HttpResponseMessage response = await client.DeleteAsync("deleteCliente/" + id);

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

            Pacientes reg = new Pacientes();
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("https://localhost:7112/api/Paciente/");
                HttpResponseMessage response = await client.GetAsync("getPaciente/" + id);

                if (response.IsSuccessStatusCode)
                {
                    string apiResponse = await response.Content.ReadAsStringAsync();
                    reg = JsonConvert.DeserializeObject<Pacientes>(apiResponse);
                }
                else
                {
                    ViewBag.mensaje = "No se encontró el paciente. Código: " + response.StatusCode;
                    return RedirectToAction("Index");
                }
            }

            return View(reg);
        }

        public async Task<IActionResult> ExportarPacientesPdf()
        {
            QuestPDF.Settings.License = QuestPDF.Infrastructure.LicenseType.Community;

            List<Pacientes> listaPacientes = new();

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("https://localhost:7112/api/Paciente/");
                HttpResponseMessage response = await client.GetAsync("getPacientes");

                if (response.IsSuccessStatusCode)
                {
                    string apiResponse = await response.Content.ReadAsStringAsync();
                    listaPacientes = JsonConvert.DeserializeObject<List<Pacientes>>(apiResponse);
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
                    page.DefaultTextStyle(x => x.FontSize(12).FontFamily("Arial"));

                    page.Content().Column(col =>
                    {
                        foreach (var paciente in listaPacientes)
                        {
                            col.Item().PageBreak(); // Página nueva para cada paciente

                            // Encabezado
                            col.Item().Row(row =>
                            {
                                row.RelativeColumn().Column(column =>
                                {
                                    column.Item().Text("CiberCare")
                                        .FontSize(20)
                                        .Bold()
                                        .FontColor(Colors.Blue.Medium);

                                    column.Item().Text("Av. Salud N°123 - Lima");
                                    column.Item().Text("Tel: (01) 123-4567");
                                    column.Item().Text("citas@cibercare.com");
                                });

                                if (System.IO.File.Exists(logoPath))
                                {
                                    row.ConstantColumn(100).Height(60).Image(logoPath, ImageScaling.FitHeight);
                                }
                            });

                            col.Item().PaddingVertical(10).LineHorizontal(1).LineColor(Colors.Grey.Lighten2);

                            // Título de sección
                            col.Item().Text("Ficha de Paciente")
                                .FontSize(16)
                                .Bold()
                                .FontColor(Colors.Indigo.Medium);

                            col.Item().PaddingBottom(10);

                            // Datos del paciente
                            col.Item().Grid(grid =>
                            {
                                grid.Columns(2);
                                grid.Spacing(5);

                                grid.Item().Text("DNI:").SemiBold();
                                grid.Item().Text(paciente.dni);

                                grid.Item().Text("Nombre:").SemiBold();
                                grid.Item().Text(paciente.nombre);

                                grid.Item().Text("Apellido:").SemiBold();
                                grid.Item().Text(paciente.apellido);

                                grid.Item().Text("Teléfono:").SemiBold();
                                grid.Item().Text(paciente.telefono);

                                grid.Item().Text("Email:").SemiBold();
                                grid.Item().Text(paciente.email);
                            });

                            col.Item().PaddingBottom(20);
                        }
                    });
                });
            });

            using var stream = new MemoryStream();
            pdf.GeneratePdf(stream);
            stream.Position = 0;

            return File(stream.ToArray(), "application/pdf", "Pacientes.pdf");
        }

        public async Task<IActionResult> ExportarPacientesExcel()
        {
            List<Pacientes> listaPacientes = new();

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("https://localhost:7112/api/Paciente/");
                HttpResponseMessage response = await client.GetAsync("getPacientes");

                if (response.IsSuccessStatusCode)
                {
                    string apiResponse = await response.Content.ReadAsStringAsync();
                    listaPacientes = JsonConvert.DeserializeObject<List<Pacientes>>(apiResponse);
                }
            }

            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("Pacientes");

                // Fila 1: Título
                worksheet.Cell(1, 1).Value = "REPORTE DE PACIENTES";
                worksheet.Range(1, 1, 1, 5).Merge();
                worksheet.Row(1).Height = 30;
                worksheet.Cell(1, 1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                worksheet.Cell(1, 1).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                worksheet.Cell(1, 1).Style.Font.Bold = true;
                worksheet.Cell(1, 1).Style.Font.FontSize = 16;
                worksheet.Cell(1, 1).Style.Fill.BackgroundColor = XLColor.BlueGray;
                worksheet.Cell(1, 1).Style.Font.FontColor = XLColor.White;

                // Fila 2: Encabezados
                worksheet.Cell(2, 1).Value = "DNI";
                worksheet.Cell(2, 2).Value = "Nombre";
                worksheet.Cell(2, 3).Value = "Apellido";
                worksheet.Cell(2, 4).Value = "Teléfono";
                worksheet.Cell(2, 5).Value = "Email";

                for (int col = 1; col <= 5; col++)
                {
                    worksheet.Cell(2, col).Style.Font.Bold = true;
                    worksheet.Cell(2, col).Style.Fill.BackgroundColor = XLColor.LightGray;
                    worksheet.Cell(2, col).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                }

                // Filas de datos
                var currentRow = 3;
                foreach (var paciente in listaPacientes)
                {
                    worksheet.Cell(currentRow, 1).Value = paciente.dni;
                    worksheet.Cell(currentRow, 2).Value = paciente.nombre;
                    worksheet.Cell(currentRow, 3).Value = paciente.apellido;
                    worksheet.Cell(currentRow, 4).Value = paciente.telefono;
                    worksheet.Cell(currentRow, 5).Value = paciente.email;
                    currentRow++;
                }

                worksheet.Columns().AdjustToContents();

                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    var content = stream.ToArray();

                    return File(content,
                                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                                "ReportePacientes.xlsx");
                }
            }
        }

    }
}
