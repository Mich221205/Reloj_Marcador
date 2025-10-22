using EjemploCoreWeb.Entities;

namespace EjemploCoreWeb.Services.Interfaces
{
    public interface IUsuarioAreaService
    {
        Task<IEnumerable<UsuarioArea>> ListarPorUsuarioAsync(int idUsuario);
        Task<IEnumerable<Area>> ListarNoAsociadasAsync(int idUsuario);
        Task<bool> AsociarAsync(int idUsuario, int idArea);
        Task<bool> DesasociarAsync(int idUsuario, int idArea);
    }
}
