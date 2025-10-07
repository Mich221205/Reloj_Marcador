using EjemploCoreWeb.Entities;

namespace EjemploCoreWeb.Repository.Interfaces
{
    public interface IRolRepository
    {
        Task<IEnumerable<Rol>> ListarAsync();
        Task<bool> ExisteAsync(int idRol);
    }
}

