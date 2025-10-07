using EjemploCoreWeb.Entities;
using System.Collections.Generic;

namespace EjemploCoreWeb.Services.Abstract
{
    public interface IRolService
    {
        IEnumerable<Rol> ObtenerRoles();
        void CrearRol(Rol rol);
        void ActualizarRol(Rol rol);
        void EliminarRol(int idRol);
    }
}
