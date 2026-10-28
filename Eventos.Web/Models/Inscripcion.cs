using System.ComponentModel.DataAnnotations;
namespace Eventos.Web.Models;
public class Inscripcion
{
    public int Id { get; set; }
    [Range(1, int.MaxValue)] public int EventoId { get; set; }
    public Evento? Evento { get; set; }
    [Range(1, int.MaxValue)] public int AsistenteId { get; set; }
    public Asistente? Asistente { get; set; }
    public DateTime FechaInscripcion { get; set; }
    [Required, StringLength(20), RegularExpression("Confirmada|Cancelada")] public string Estado { get; set; } = "Confirmada";
}
