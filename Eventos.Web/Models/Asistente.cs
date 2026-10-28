using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
namespace Eventos.Web.Models;
[Index(nameof(Dni), IsUnique = true)]
[Index(nameof(Correo), IsUnique = true)]
public class Asistente
{
    public int Id { get; set; }
    [Required, StringLength(100)] public string Nombres { get; set; } = "";
    [Required, RegularExpression(@"^[0-9]{8}$", ErrorMessage = "El DNI debe tener 8 dígitos.")] public string Dni { get; set; } = "";
    [Required, EmailAddress, StringLength(254)] public string Correo { get; set; } = "";
    public bool Activo { get; set; } = true;
    [StringLength(9)] public string? Telefono { get; set; }
    public ICollection<Inscripcion> Inscripciones { get; set; } = new List<Inscripcion>();
}
