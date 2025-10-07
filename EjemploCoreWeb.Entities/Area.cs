using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EjemploCoreWeb.Entities;

public class Area
{
    public int ID_Area { get; set; }
    public string Nombre_Area { get; set; } = "";
    public int Jefe_Area { get; set; }
    public string Codigo_Area { get; set; } = "";

    public string? Jefe_Nombre { get; set; }
}
