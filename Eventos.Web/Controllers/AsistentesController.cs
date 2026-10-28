using Eventos.Web.Models;
using Eventos.Web.Repositorios;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
namespace Eventos.Web.Controllers;
public class AsistentesController(AsistenteRepositorio repo) : Controller
{
    public async Task<IActionResult> Index() => View(await repo.ListarAsync());
    public async Task<IActionResult> Create() { await Task.CompletedTask; return View(new Asistente()); }
    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("Nombres,Dni,Correo,Telefono")] Asistente a)
    {
        if (!string.IsNullOrWhiteSpace(a.Dni) && await repo.DniExisteAsync(a.Dni)) ModelState.AddModelError("Dni", "El DNI ya está registrado.");
        if (!string.IsNullOrWhiteSpace(a.Correo) && await repo.CorreoExisteAsync(a.Correo)) ModelState.AddModelError("Correo", "El correo ya está registrado.");
        if (ModelState.IsValid)
        {
            try { await repo.CrearAsync(a); TempData["Mensaje"] = "Asistente registrado correctamente."; return RedirectToAction(nameof(Index)); }
            catch (DbUpdateException) { ModelState.AddModelError("", "No se pudo guardar. Compruebe que el DNI y el correo sean únicos."); }
        }
        return View(a);
    }
}
