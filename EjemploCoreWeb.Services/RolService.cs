using System.Collections.Generic;
using EjemploCoreWeb.Entities;
using EjemploCoreWeb.Repository;
using EjemploCoreWeb.Services.Abstract;

namespace EjemploCoreWeb.Services
{
    public class RolService : IRolService
    {
        private readonly RolRepository _rolRepository;

        public RolService(RolRepository rolRepository)
        {
            _rolRepository = rolRepository;
        }

        public IEnumerable<Rol> ObtenerRoles() => _rolRepository.GetAll();

        public void CrearRol(Rol rol) => _rolRepository.Insert(rol);

        public void ActualizarRol(Rol rol) => _rolRepository.Update(rol);

        public void EliminarRol(int idRol) => _rolRepository.Delete(idRol);
    }
}
