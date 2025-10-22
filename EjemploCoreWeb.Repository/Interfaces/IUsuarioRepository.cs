using EjemploCoreWeb.Entities;

namespace EjemploCoreWeb.Repository.Interfaces
{
    public interface IUsuarioRepository
    {
        Task<IEnumerable<Usuario>> ListarAsync(string? filtro);

        Task<(IEnumerable<Usuario> Items, int Total)> ListarPaginadoAsync(string? filtro, int page, int size);

        Task<IEnumerable<Usuario>> ListarPaginaAsync(string? filtro, int offset, int size);
        Task<int> ContarAsync(string? filtro);

        Task<Usuario?> ObtenerAsync(int id);

        Task<int> CrearAsync(Usuario u, int tipoId, string plainPassword);
        Task<bool> ActualizarAsync(Usuario u, int tipoId, string? plainPassword);

        Task<bool> EliminarAsync(int id);

        Task<IEnumerable<Rol>> ListarRolesAsync();
        Task<IEnumerable<TipoIdentificacion>> ListarTiposAsync();
    }
}
