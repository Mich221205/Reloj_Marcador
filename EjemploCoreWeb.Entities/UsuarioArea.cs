using System.ComponentModel.DataAnnotations.Schema;

namespace EjemploCoreWeb.Entities
{
    public class UsuarioArea
    {
        public int ID_Usuario { get; set; }
        public int ID_Area { get; set; }

        [NotMapped]
        public string Nombre_Area { get; set; } = string.Empty;

        [NotMapped]
        public string? Codigo_Area { get; set; }
    }
}
