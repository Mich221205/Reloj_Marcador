using System.Linq;
using EjemploCoreWeb.Entities;
using EjemploCoreWeb.Repository.Interfaces;   // IAreaRepository
using EjemploCoreWeb.Services.Interfaces;     // IAreaService

namespace EjemploCoreWeb.Services.Services
{
    public class AreaService : IAreaService
    {
        private readonly IAreaRepository _repo;

        // Sólo el repo de áreas (ya está registrado en Program.cs)
        public AreaService(IAreaRepository repo)
        {
            _repo = repo;
        }

        public Task<IEnumerable<Area>> ListarAsync(string? filtro) => _repo.ListarAsync(filtro);
        public Task<Area?> ObtenerAsync(int id) => _repo.ObtenerAsync(id);
        public Task<int> CrearAsync(Area a) => _repo.CrearAsync(a);
        public Task<bool> ActualizarAsync(Area a) => _repo.ActualizarAsync(a);
        public Task<bool> EliminarAsync(int id) => _repo.EliminarAsync(id);

        // Stub temporal para compilar (ajústalo luego si necesitas datos reales)
        public Task<IEnumerable<(int Id, string Nombre)>> ListarFuncionariosAsync()
        {
            IEnumerable<(int, string)> vacio = Enumerable.Empty<(int, string)>();
            return Task.FromResult(vacio);
        }
    }
}
