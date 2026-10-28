using Eventos.Web.Data;
using Eventos.Web.Models;
using Microsoft.EntityFrameworkCore;
namespace Eventos.Web.Repositorios;
public class EventoRepositorio(EventosContext db)
{
    public Task<List<Categoria>> CategoriasAsync() => db.Categorias.AsNoTracking().Where(c => c.Activo).OrderBy(c => c.Nombre).ToListAsync();
    public Task<bool> CategoriaActivaAsync(int id) => db.Categorias.AsNoTracking().AnyAsync(c => c.Id == id && c.Activo);
    public Task<List<Evento>> ListarAsync(int? categoriaId) => db.Eventos.AsNoTracking().Include(e => e.Categoria).Include(e => e.Inscripciones).Where(e => e.Activo && (!categoriaId.HasValue || e.CategoriaId == categoriaId)).OrderBy(e => e.Fecha).ToListAsync();
    public Task<Evento?> ObtenerAsync(int id) => db.Eventos.AsNoTracking().Include(e => e.Categoria).Include(e => e.Inscripciones).FirstOrDefaultAsync(e => e.Id == id && e.Activo);
    public async Task CrearAsync(Evento e) { e.Activo = true; db.Eventos.Add(e); await db.SaveChangesAsync(); }
    public async Task<bool> EditarAsync(Evento input)
    {
        var e = await db.Eventos.FirstOrDefaultAsync(e => e.Id == input.Id && e.Activo);
        if (e == null) return false;
        e.Titulo = input.Titulo; e.Fecha = input.Fecha; e.Lugar = input.Lugar; e.Aforo = input.Aforo; e.Precio = input.Precio; e.CategoriaId = input.CategoriaId; e.Modalidad = input.Modalidad;
        await db.SaveChangesAsync(); return true;
    }
    public async Task<bool> EliminarAsync(int id)
    {
        var e = await db.Eventos.FirstOrDefaultAsync(e => e.Id == id && e.Activo);
        if (e == null) return false;
        e.Activo = false; await db.SaveChangesAsync(); return true;
    }
}
