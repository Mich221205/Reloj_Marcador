using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EjemploCoreWeb.Entities
{
    public class Area
    {
        public int ID_Area { get; set; }

        [Required(ErrorMessage = "El nombre del área es obligatorio.")]
        [StringLength(40, ErrorMessage = "El nombre del área no debe exceder los 40 caracteres.")]
        [RegularExpression(@"^[A-Za-zÁÉÍÓÚáéíóúñÑ ]+$",
            ErrorMessage = "El nombre del área solo puede tener letras y espacios.")]
        public string Nombre_Area { get; set; } = string.Empty;

        // FK a Usuario.Id_Usuario (funcionario que es jefe del área)
        [Required(ErrorMessage = "Debe seleccionar un jefe de área.")]
        public int Jefe_Area { get; set; }

        // Campo opcional: no es requerido por la HU, pero lo dejamos disponible
        [StringLength(15, ErrorMessage = "El código no debe exceder 15 caracteres.")]
        public string? Codigo_Area { get; set; }

        // Solo para mostrar en UI (no se persiste)
        [NotMapped]
        public string? Jefe_Nombre { get; set; }
    }
}
