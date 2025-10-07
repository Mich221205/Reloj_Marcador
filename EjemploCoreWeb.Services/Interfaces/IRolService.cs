using EjemploCoreWeb.Entities;

namespace EjemploCoreWeb.Services.Interfaces
{
    public interface IRolService
    {
        Task<IEnumerable<Rol>> ListarAsync();
        Task<bool> ExisteAsync(int idRol);
    }
}
