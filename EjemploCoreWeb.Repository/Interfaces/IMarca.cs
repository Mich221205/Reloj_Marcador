using EjemploCoreWeb.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EjemploCoreWeb.Repository.Interfaces
{
    public interface IMarca
    {
        //Jocsan
        //Reporte de marcas ADM 16

        Task<IEnumerable<Marca>> Reporte_Marcas(
        int page,
        int pageSize,
        string? identificacion,
        DateTime? fecha);

        Task<int> Contar_Reporte_Marca(string? identificacion, DateTime? fecha);


    }
}
