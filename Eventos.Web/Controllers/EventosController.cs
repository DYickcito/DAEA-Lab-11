using Eventos.Web.Models;
using Eventos.Web.Repositorios;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
namespace Eventos.Web.Controllers;
public class EventosController(EventoRepositorio repo) : Controller
{
    private async Task Categorias(int? selected = null) => ViewBag.Categorias = new SelectList(await repo.CategoriasAsync(), "Id", "Nombre", selected);
    public async Task<IActionResult> Index(int? categoriaId) { await Categorias(categoriaId); return View(await repo.ListarAsync(categoriaId)); }
    public async Task<IActionResult> Details(int id) { var e = await repo.ObtenerAsync(id); return e == null ? NotFound() : View(e); }
    public async Task<IActionResult> Create() { await Categorias(); return View(new Evento { Fecha = DateTime.Today.AddDays(1).AddHours(9), Aforo = 1 }); }
    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("Titulo,Fecha,Lugar,Aforo,Precio,CategoriaId,Modalidad")] Evento e)
    {
        if (!await repo.CategoriaActivaAsync(e.CategoriaId)) ModelState.AddModelError("CategoriaId", "Seleccione una categoría activa.");
        if (ModelState.IsValid) { await repo.CrearAsync(e); TempData["Mensaje"] = "Evento creado correctamente."; return RedirectToAction(nameof(Index)); }
        await Categorias(e.CategoriaId); return View(e);
    }
    public async Task<IActionResult> Edit(int id) { var e = await repo.ObtenerAsync(id); if (e == null) return NotFound(); await Categorias(e.CategoriaId); return View(e); }
    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, [Bind("Id,Titulo,Fecha,Lugar,Aforo,Precio,CategoriaId,Modalidad")] Evento e)
    {
        if (id != e.Id) return NotFound();
        if (!await repo.CategoriaActivaAsync(e.CategoriaId)) ModelState.AddModelError("CategoriaId", "Seleccione una categoría activa.");
        var actual = await repo.ObtenerAsync(id); if (actual == null) return NotFound();
        if (e.Aforo < actual.Inscripciones.Count(i => i.Estado == "Confirmada")) ModelState.AddModelError("Aforo", "El aforo no puede ser menor que los inscritos confirmados.");
        if (ModelState.IsValid) { if (!await repo.EditarAsync(e)) return NotFound(); TempData["Mensaje"] = "Evento actualizado correctamente."; return RedirectToAction(nameof(Index)); }
        await Categorias(e.CategoriaId); return View(e);
    }
    public async Task<IActionResult> Delete(int id) { var e = await repo.ObtenerAsync(id); return e == null ? NotFound() : View(e); }
    [HttpPost, ActionName("Delete"), ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id) { if (!ModelState.IsValid) return BadRequest(); if (!await repo.EliminarAsync(id)) return NotFound(); TempData["Mensaje"] = "Evento desactivado correctamente."; return RedirectToAction(nameof(Index)); }
}
