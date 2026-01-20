using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PeopleApp.Api.Models;

[Table("personas")]
public class Persona
{
    [Key]
    public int Id { get; set; }

    [Required]
    public string Nombre { get; set; } = string.Empty;

    [Required]
    public int Edad { get; set; }

    [Required]
    public double Estatura { get; set; }

    [Required]
    public double Peso { get; set; }

    [Required]
    public string Descripcion { get; set; } = string.Empty;

    // más adelante:
    // public string UserId { get; set; }
}
