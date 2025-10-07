using EjemploCoreWeb.Entities;

namespace EjemploCoreWeb.Repository.Interfaces;

public interface ITipoIdentificacionRepository
{
    Task<IEnumerable<TipoIdentificacion>> ListarAsync();
}
