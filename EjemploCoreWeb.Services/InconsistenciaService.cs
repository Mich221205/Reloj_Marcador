using EjemploCoreWeb.Entities;
using EjemploCoreWeb.Repository.Interfaces;
using EjemploCoreWeb.Services.Abstract;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EjemploCoreWeb.Services
{
    public class InconsistenciaService : IInconsistenciaService
    {
        private readonly IInconsistenciaRepository _repository;

        public InconsistenciaService(IInconsistenciaRepository repository)
        {
            _repository = repository;
        }

        public Task<IEnumerable<Inconsistencia>> Listar(int page, int pageSize)
            => _repository.GetAllAsync(page, pageSize);

        public Task<Inconsistencia?> ObtenerPorId(int id)
            => _repository.GetByIdAsync(id);

        public Task<int> Crear(Inconsistencia inconsistencia)
            => _repository.InsertAsync(inconsistencia);

        public Task<int> Actualizar(Inconsistencia inconsistencia)
            => _repository.UpdateAsync(inconsistencia);

        public async Task<int> Eliminar(int id)
        {
            bool asignado = await _repository.EstaAsignadoAsync(id);
            if (asignado)
                throw new InvalidOperationException("No se puede eliminar un registro con datos relacionados.");

            return await _repository.DeleteAsync(id);
        }


        public Task<int> Contar()
            => _repository.CountAsync();

        //Jocsan
        //Reportes de Inconsistencias ADM 15
        public Task<IEnumerable<Reporte_Inconsistencia>> Reporte_Inconsistencias(
    int page,
    int pageSize,
    string? identificacion,
    DateTime? fecha)
    => _repository.Reporte_Inconsistencias(page, pageSize, identificacion, fecha);


        public Task<int> ContarReporteInconsistencias(
    string? identificacion,
    DateTime? fecha)
    => _repository.Contar_Reporte(identificacion, fecha);



    }
}

