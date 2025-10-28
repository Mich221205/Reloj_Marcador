using System.ComponentModel.DataAnnotations;

namespace EjemploCoreWeb.Entities
{
    public class Usuario
    {
        public int Id_Usuario { get; set; }

        [Required]
        public string Identificacion { get; set; } = string.Empty;

        [Required]
        public string Nombre { get; set; } = string.Empty;

        [Required]
        public string Apellido_1 { get; set; } = string.Empty;
        public string? Apellido_2 { get; set; }

        [Required, EmailAddress]
        public string Correo { get; set; } = string.Empty;

        public string? Telefono { get; set; }

        [Required]
        public int Id_Rol_Usuario { get; set; }

        public string Contrasena { get; set; } = string.Empty;

        public bool Estado { get; set; }
    }

    public class Motivos_Ausencia
    {
        public int ID_Motivo { get; set; }
        public string Nombre_Motivo { get; set; } = string.Empty;
    }

    public class Rol
    {
        public int ID_Rol_Usuario { get; set; }

        [Required(ErrorMessage = "El nombre del rol es obligatorio.")]
        [StringLength(40, ErrorMessage = "El nombre del rol no puede exceder los 40 caracteres.")]
        [RegularExpression("^[A-Za-zÁÉÍÓÚáéíóúñÑ ]+$", ErrorMessage = "El nombre del rol solo puede contener letras y espacios.")]
        public string Nombre_Rol { get; set; } = string.Empty;
    }

    public class TipoIdentificacion
    {
        public int ID_Tipo_Identificacion { get; set; }

        [Required(ErrorMessage = "El tipo de identificación es obligatorio.")]
        [StringLength(40, ErrorMessage = "El nombre del tipo no puede exceder los 40 caracteres.")]
        [RegularExpression("^[A-Za-zÁÉÍÓÚáéíóúñÑ ]+$", ErrorMessage = "El nombre solo puede contener letras y espacios.")]
        public string Tipo_Identificacion { get; set; } = string.Empty;
    }
}
