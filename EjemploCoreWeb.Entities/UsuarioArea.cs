using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EjemploCoreWeb.Entities
{
    public class UsuarioArea
    {
        public int ID_Usuario { get; set; }
        public int ID_Area { get; set; }
        public DateTime Fecha_Asignacion { get; set; }

        // Para listados
        public string? Nombre_Area { get; set; }
    }
}
