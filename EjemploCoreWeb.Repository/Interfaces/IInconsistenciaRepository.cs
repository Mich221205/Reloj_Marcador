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
    }
}

