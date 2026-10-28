using Eventos.Web.Models;
using Eventos.Web.Repositorios;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
namespace Eventos.Web.Controllers;
public class InscripcionesController(InscripcionRepositorio repo) : Controller
{
    private async Task Listas(Inscripcion i) { ViewBag.Eventos = new SelectList(await repo.EventosAsync(), "Id", "Titulo", i.EventoId); ViewBag.Asistentes = new SelectList(await repo.AsistentesAsync(), "Id", "Nombres", i.AsistenteId); }
    public async Task<IActionResult> Index() => View(await repo.ListarAsync());
    public async Task<IActionResult> Create() { var i = new Inscripcion(); await Listas(i); return View(i); }
    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("EventoId,AsistenteId")] Inscripcion i)
    {
        if (ModelState.IsValid)
        {
            var (campo, error) = await repo.CrearAsync(i.EventoId, i.AsistenteId);
            if (error == "") { TempData["Mensaje"] = "Inscripción confirmada."; return RedirectToAction(nameof(Index)); }
            ModelState.AddModelError(campo, error);
        }
        await Listas(i); return View(i);
    }
    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Cancelar(int id) { if (!await repo.CancelarAsync(id)) return NotFound(); TempData["Mensaje"] = "Inscripción cancelada."; return RedirectToAction(nameof(Index)); }
}
