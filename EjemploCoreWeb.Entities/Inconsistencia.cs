using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EjemploCoreWeb.Entities
{
    public class Inconsistencia
    {
        public int ID_Inconsistencia { get; set; }
        public string Nombre_Inconsistencia { get; set; }
    }

    public class Reporte_Inconsistencia : Usuario
    {
        public int ID_Inconsistencia_Usuario { get; set; }
        public string Identificacion { get; set; }
        public string Nombre_Inconsistencia { get; set; }
        public DateTime Fecha_Inconsistencia { get; set; }
    
    }
}
