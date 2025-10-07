using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EjemploCoreWeb.Entities
{
    public class Usuario
    {
        public int Id_Usuario { get; set; }
        public string Identificacion { get; set; } = string.Empty;
        public string Nombre { get; set; } = string.Empty;
        public string Apellido_1 { get; set; } = string.Empty;
        public string Apellido_2 { get; set; } = string.Empty;
        public string Correo { get; set; } = string.Empty;
        public string Telefono { get; set; }
        public int Id_Rol_Usuario { get; set; }
        public string Contrasena { get; set; } = string.Empty;
        public bool Estado { get; set; }

    }
    public class Motivos_Ausencia
    {

        public int ID_Motivo { get; set; }

        public string Nombre_Motivo { get; set; }

    }


}
