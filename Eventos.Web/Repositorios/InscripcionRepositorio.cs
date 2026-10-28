using System.Data;
using Eventos.Web.Data;
using Eventos.Web.Models;
using Microsoft.EntityFrameworkCore;
namespace Eventos.Web.Repositorios;
public class InscripcionRepositorio(EventosContext db)
{
    public Task<List<Evento>> EventosAsync() => db.Eventos.AsNoTracking().Where(e => e.Activo).OrderBy(e => e.Fecha).ToListAsync();
    public Task<List<Asistente>> AsistentesAsync() => db.Asistentes.AsNoTracking().Where(a => a.Activo).OrderBy(a => a.Nombres).ToListAsync();
    public Task<List<Inscripcion>> ListarAsync() => db.Inscripciones.AsNoTracking().Include(i => i.Evento).Include(i => i.Asistente).OrderByDescending(i => i.FechaInscripcion).ToListAsync();
    // La transacción serializable evita sobrepasar el aforo o duplicar inscripciones concurrentes.
    public async Task<(string campo, string error)> CrearAsync(int eventoId, int asistenteId)
    {
        await using var tx = await db.Database.BeginTransactionAsync(IsolationLevel.Serializable);
        var evento = await db.Eventos.AsNoTracking().FirstOrDefaultAsync(e => e.Id == eventoId && e.Activo);
        if (evento == null) return ("EventoId", "Seleccione un evento activo.");
        if (!await db.Asistentes.AsNoTracking().AnyAsync(a => a.Id == asistenteId && a.Activo)) return ("AsistenteId", "Seleccione un asistente activo.");
        if (evento.Fecha < DateTime.Now) return ("EventoId", "El evento ya ha pasado.");
        if (await db.Inscripciones.AsNoTracking().AnyAsync(i => i.EventoId == eventoId && i.AsistenteId == asistenteId && i.Estado == "Confirmada")) return ("AsistenteId", "El asistente ya está inscrito en este evento.");
        if (await db.Inscripciones.AsNoTracking().CountAsync(i => i.EventoId == eventoId && i.Estado == "Confirmada") >= evento.Aforo) return ("EventoId", "El aforo está completo.");
        db.Inscripciones.Add(new Inscripcion { EventoId = eventoId, AsistenteId = asistenteId, FechaInscripcion = DateTime.Now, Estado = "Confirmada" });
        await db.SaveChangesAsync(); await tx.CommitAsync(); return ("", "");
    }
    public async Task<bool> CancelarAsync(int id)
    {
        var i = await db.Inscripciones.FirstOrDefaultAsync(i => i.Id == id && i.Estado == "Confirmada");
        if (i == null) return false;
        i.Estado = "Cancelada"; await db.SaveChangesAsync(); return true;
    }
}
