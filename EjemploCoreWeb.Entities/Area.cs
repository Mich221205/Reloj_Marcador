using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EjemploCoreWeb.Entities;

public class Area
{
    public int ID_Area { get; set; }

    [Required, StringLength(40)]
    [RegularExpression(@"^[A-Za-zÁÉÍÓÚáéíóúñÑ ]+$",
        ErrorMessage = "El nombre solo puede tener letras y espacios.")]
    public string Nombre_Area { get; set; } = string.Empty;

    [Required]
    public int Jefe_Area { get; set; }

    [Required, StringLength(15)]
    public string Codigo_Area { get; set; } = string.Empty;

    [NotMapped]
    public string? Jefe_Nombre { get; set; }
}
