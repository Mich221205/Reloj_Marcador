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

    //Jocsan
    public class Reporte_Inconsistencia : Usuario
    {
        public int ID_Inconsistencia_Usuario { get; set; }
        public string Identificacion { get; set; }
        public string Nombre_Inconsistencia { get; set; }
        public DateTime Fecha_Inconsistencia { get; set; }
        public string Detalle { get; set; }
        public string Referencia { get; set; }

    }

    public class ParametrosInconsistencia
    {
        public int Tolerancia_Atraso { get; set; }
        public int Tolerancia_Salida_Temprana { get; set; }
    }

    public class Exclusion_Inconsistencia
    {
        public bool Exclusion { get; set; }
        public string Detalle { get; set; }
    }

    public class InconsistenciaDetectada
    {
        public string Tipo { get; set; }
        public DateTime Fecha { get; set; }
        public int ID_Usuario { get; set; }
        public string? Detalle { get; set; }
        public string? Referencia { get; set; }

        public InconsistenciaDetectada(string tipo, DateTime fecha, int idUsuario, string? detalle = null, string? referencia = null)
        {
            Tipo = tipo;
            Fecha = fecha;
            ID_Usuario = idUsuario;
            Detalle = detalle;
            Referencia = referencia;
        }
    }

}
