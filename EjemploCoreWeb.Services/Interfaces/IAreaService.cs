using EjemploCoreWeb.Entities;

namespace EjemploCoreWeb.Services.Interfaces
{
    public interface IAreaService
    {
        Task<IEnumerable<Area>> ListarAsync(string? filtro);
        Task<Area?> ObtenerAsync(int id);
        Task<int> CrearAsync(Area a);
        Task<bool> ActualizarAsync(Area a);
        Task<bool> EliminarAsync(int id);

        Task<IEnumerable<(int Id, string Nombre)>> ListarFuncionariosAsync();
    }
}
