using EjemploCoreWeb.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EjemploCoreWeb.Services.Abstract
{
    public interface IMarcaService
    {

        //Jocsan
        //Reportes de Inconsistencias ADM 15

        Task<IEnumerable<Marca>> Reporte_Marcas(
        int page,
        int pageSize,
        string? identificacion,
        DateTime? fecha);

        Task<int> Contar_Reporte_Marca(
        string? identificacion,
        DateTime? fecha);

    }
}
