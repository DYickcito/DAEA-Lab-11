using Eventos.Web.Data;
using Eventos.Web.Models;
using Eventos.Web.Repositorios;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<EventosContext>(o => o.UseSqlServer(builder.Configuration.GetConnectionString("EventosDB")));
builder.Services.AddScoped<EventoRepositorio>();
builder.Services.AddScoped<AsistenteRepositorio>();
builder.Services.AddScoped<InscripcionRepositorio>();
var app = builder.Build();

// La base debe estar migrada antes de iniciar la aplicación.
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<EventosContext>();
    if (!await db.Categorias.AnyAsync())
    {
        db.Categorias.AddRange(new Categoria { Nombre = "Tecnología" }, new Categoria { Nombre = "Cultura" }, new Categoria { Nombre = "Negocios" });
        await db.SaveChangesAsync();
    }
    if (!await db.Eventos.AnyAsync())
    {
        var categorias = await db.Categorias.AsNoTracking().OrderBy(c => c.Id).ToListAsync();
        if (categorias.Count > 0)
        {
            db.Eventos.AddRange(
                new Evento { Titulo = "Encuentro de innovación", Fecha = DateTime.Today.AddDays(20).AddHours(9), Lugar = "Centro de convenciones", Aforo = 150, Precio = 45, CategoriaId = categorias[0].Id },
                new Evento { Titulo = "Ideas que conectan", Fecha = DateTime.Today.AddDays(35).AddHours(18), Lugar = "Auditorio central", Aforo = 120, Precio = 30, CategoriaId = categorias[Math.Min(1, categorias.Count - 1)].Id },
                new Evento { Titulo = "Foro de emprendimiento", Fecha = DateTime.Today.AddDays(50).AddHours(10), Lugar = "Sala principal", Aforo = 80, Precio = 60, CategoriaId = categorias[Math.Min(2, categorias.Count - 1)].Id },
                new Evento { Titulo = "Taller de tendencias digitales", Fecha = DateTime.Today.AddDays(65).AddHours(16), Lugar = "En línea", Aforo = 200, Precio = 0, CategoriaId = categorias[0].Id, Modalidad = "Virtual" });
            await db.SaveChangesAsync();
        }
    }
    if (!await db.Asistentes.AnyAsync())
    {
        db.Asistentes.AddRange(new Asistente { Nombres = "Ana Torres", Dni = "12345671", Correo = "ana@example.com" }, new Asistente { Nombres = "Bruno Díaz", Dni = "12345672", Correo = "bruno@example.com" }, new Asistente { Nombres = "Carla Soto", Dni = "12345673", Correo = "carla@example.com" }, new Asistente { Nombres = "Diego Pérez", Dni = "12345674", Correo = "diego@example.com" }, new Asistente { Nombres = "Elena Ruiz", Dni = "12345675", Correo = "elena@example.com" });
        await db.SaveChangesAsync();
    }
}
if (!app.Environment.IsDevelopment()) { app.UseExceptionHandler("/Home/Error"); app.UseHsts(); }
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();
app.MapControllerRoute(name: "default", pattern: "{controller=Eventos}/{action=Index}/{id?}");
app.Run();
