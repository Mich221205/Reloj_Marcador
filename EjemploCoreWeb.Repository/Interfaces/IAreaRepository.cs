using EjemploCoreWeb.Entities;

namespace EjemploCoreWeb.Repository.Interfaces
{
    public interface IAreaRepository
    {
        Task<IEnumerable<Area>> ListarAsync(string? filtro);
        Task<Area?> ObtenerAsync(int id);
        Task<int> CrearAsync(Area a);
        Task<bool> ActualizarAsync(Area a);
        Task<bool> EliminarAsync(int id);
    }
}
