using EjemploCoreWeb.Entities;

namespace EjemploCoreWeb.Services.Interfaces;

public interface IUsuarioService
{
    Task<IEnumerable<Usuario>> ListarAsync(string? filtro);
    Task<(IEnumerable<Usuario> Items, int Total)> ListarPaginadoAsync(string? filtro, int page, int size);
    Task<int> ContarAsync(string? filtro);

    Task<Usuario?> ObtenerAsync(int id);
    Task<int> CrearAsync(Usuario u, string password);
    Task<bool> ActualizarAsync(Usuario u, string? nuevaPassword);
    Task<bool> EliminarAsync(int id);

    Task<IEnumerable<Rol>> ListarRolesAsync();
    Task<IEnumerable<TipoIdentificacion>> ListarTiposAsync();
}
