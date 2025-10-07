using System.Collections.Generic;
using EjemploCoreWeb.Entities;
using EjemploCoreWeb.Repository;
using EjemploCoreWeb.Services.Abstract;

namespace EjemploCoreWeb.Services
{
    public class TipoIdentificacionService : ITipoIdentificacionService
    {
        private readonly IdentificacionRepository _repo;

        public TipoIdentificacionService(IdentificacionRepository repo)
        {
            _repo = repo;
        }

        public IEnumerable<TipoIdentificacion> ObtenerTodos() => _repo.GetAll();
        public void Crear(TipoIdentificacion tipo) => _repo.Insert(tipo);
        public void Actualizar(TipoIdentificacion tipo) => _repo.Update(tipo);
        public void Eliminar(int idTipo) => _repo.Delete(idTipo);
    }
}

