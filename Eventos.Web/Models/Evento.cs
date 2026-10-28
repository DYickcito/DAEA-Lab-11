using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace Eventos.Web.Models;
public class Evento
{
    public int Id { get; set; }
    [Required, StringLength(100)] public string Titulo { get; set; } = "";
    [Required, DataType(DataType.DateTime)] public DateTime Fecha { get; set; }
    [Required, StringLength(100)] public string Lugar { get; set; } = "";
    [Range(1, 5000)] public int Aforo { get; set; }
    [Range(typeof(decimal), "0", "99999999.99", ParseLimitsInInvariantCulture = true), Column(TypeName = "decimal(10,2)")] public decimal Precio { get; set; }
    [Range(1, int.MaxValue)] public int CategoriaId { get; set; }
    public Categoria? Categoria { get; set; }
    public bool Activo { get; set; } = true;
    [Required, StringLength(20), RegularExpression("Presencial|Virtual")] public string Modalidad { get; set; } = "Presencial";
    public ICollection<Inscripcion> Inscripciones { get; set; } = new List<Inscripcion>();
}
