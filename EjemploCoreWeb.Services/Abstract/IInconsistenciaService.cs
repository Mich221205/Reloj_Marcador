using EjemploCoreWeb.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EjemploCoreWeb.Services.Abstract
{
    public interface IInconsistenciaService
    {
        Task<IEnumerable<Inconsistencia>> Listar(int page, int pageSize);
        Task<Inconsistencia?> ObtenerPorId(int id);
        Task<int> Crear(Inconsistencia inconsistencia);
        Task<int> Actualizar(Inconsistencia inconsistencia);
        Task<int> Eliminar(int id);
        Task<int> Contar();


        //Reportes
        Task<IEnumerable<Reporte_Inconsistencia>> Reporte_Inconsistencias(int page, int pageSize);

        Task<int> ContarReporteInconsistencias();

        Task<IEnumerable<Reporte_Inconsistencia>> Reporte_Inconsistencias_Filtros(int page,
        int pageSize,
        string? identificacion = null,
        DateTime? fecha = null);




    }
}

