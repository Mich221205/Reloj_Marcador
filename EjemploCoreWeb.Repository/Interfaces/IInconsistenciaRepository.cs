using EjemploCoreWeb.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EjemploCoreWeb.Repository.Interfaces
{
    public interface IInconsistenciaRepository
    {
        Task<IEnumerable<Inconsistencia>> GetAllAsync(int page, int pageSize);
        Task<Inconsistencia?> GetByIdAsync(int id);
        Task<int> InsertAsync(Inconsistencia inconsistencia);
        Task<int> UpdateAsync(Inconsistencia inconsistencia);
        Task<int> DeleteAsync(int id);
        Task<int> CountAsync();
        Task<bool> EstaAsignadoAsync(int id);

        //Jocsan
        //Reporte de Inconsistencias ADM 15
        Task<IEnumerable<Reporte_Inconsistencia>> Reporte_Inconsistencias(int page, int pageSize);
        Task<int> Contar_Reportes();

       Task<IEnumerable<Reporte_Inconsistencia>> Reporte_Inconsistencias_Filtros(
        int page,
        int pageSize,
        string? identificacion = null,
        DateTime? fecha = null);



    }
}

