var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddHttpContextAccessor();


builder.Services.AddAuthentication("CookieAuth") // Define el esquema de autenticacion
    .AddCookie("CookieAuth", options =>
    {
        // Define la ruta a la que se redirigira si el usuario no esta autenticado
        options.LoginPath = "/Admin/Index";
        //options.AccessDeniedPath = "/Home/AccessDenied"; // Opcional: para permisos
    });

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30); // Tiempo de expiracion de la sesion
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();


app.UseAuthentication();

app.UseSession();

app.UseAuthorization();



// Ruta personalizada para acceder directamente a la accion Insert del controlador Pacientes
//app.MapControllerRoute(
//    name: "insertPaciente",
//    pattern: "Pacientes/Insert", // Ruta que ira a la accion Insert
//    defaults: new { controller = "Pacientes", action = "Insert" }
//);

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Admin}/{action=Index}/{id?}");

app.Run();

