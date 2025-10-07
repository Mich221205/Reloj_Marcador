using EjemploCoreWeb.Entities;

namespace EjemploCoreWeb.Repository.Interfaces;

public interface IUsuarioAreaRepository
{
    Task<IEnumerable<UsuarioArea>> ListarPorUsuarioAsync(int idUsuario);
    Task<IEnumerable<Area>> ListarNoAsociadasAsync(int idUsuario);
    Task<bool> AsociarAsync(int idUsuario, int idArea);
    Task<bool> DesasociarAsync(int idUsuario, int idArea);
}
