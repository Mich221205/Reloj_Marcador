using EjemploCoreWeb.Entities;
using System.Collections.Generic;

namespace EjemploCoreWeb.Services.Abstract
{
    public interface ITipoIdentificacionService
    {
        IEnumerable<TipoIdentificacion> ObtenerTodos();
        void Crear(TipoIdentificacion tipo);
        void Actualizar(TipoIdentificacion tipo);
        void Eliminar(int idTipo);
    }
}
