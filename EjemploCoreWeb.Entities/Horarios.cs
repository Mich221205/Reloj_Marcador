using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EjemploCoreWeb.Entities
{
    public class Horarios
    {
        public int ID_Horario { get; set; }

        public string Identificacion { get; set; }

        public string Nombre_Area { get; set; }

        public int ID_Area { get; set; }

        public string Codigo_Area { get; set; } = string.Empty;


    }

    public class Detalle_Horarios
    {
        public int ID_Detalle { get; set; }
        public int ID_Horario { get; set; }

        public string Dia { get; set; } = string.Empty; // Ej: "Lunes", "Martes", etc.

        // Horas
        public byte Hora_Ingreso { get; set; }
        public byte Minuto_Ingreso { get; set; }
        public byte Hora_Salida { get; set; }
        public byte Minuto_Salida { get; set; }

    }

}
