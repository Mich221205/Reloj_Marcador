using Dapper;
using EjemploCoreWeb.Entities;
using EjemploCoreWeb.Repository;
using EjemploCoreWeb.Repository.Interfaces;   // IAreaRepository
using EjemploCoreWeb.Services.Interfaces;     // IAreaService
using System.Linq;

namespace EjemploCoreWeb.Services.Services
{
    public class AreaService : IAreaService
    {
        private readonly IAreaRepository _repo;
        private readonly IDbConnectionFactory _factory;

        // Inyectamos también la fábrica de conexiones para listar funcionarios
        public AreaService(IAreaRepository repo, IDbConnectionFactory factory)
        {
            _repo = repo;
            _factory = factory;
        }

        public Task<IEnumerable<Area>> ListarAsync(string? filtro) => _repo.ListarAsync(filtro);
        public Task<Area?> ObtenerAsync(int id) => _repo.ObtenerAsync(id);
        public Task<int> CrearAsync(Area a) => _repo.CrearAsync(a);
        public Task<bool> ActualizarAsync(Area a) => _repo.ActualizarAsync(a);
        public Task<bool> EliminarAsync(int id) => _repo.EliminarAsync(id);

        /// <summary>
        /// Lista de funcionarios activos para seleccionar como Jefe de Área.
        /// Muestra: "Nombre Apellido1 Apellido2 (Identificación)".
        /// </summary>
        public async Task<IEnumerable<(int Id, string Nombre)>> ListarFuncionariosAsync()
        {
            using var con = _factory.CreateConnection();

            const string sql = @"
SELECT 
    u.ID_Usuario      AS Id,
    CONCAT(u.Nombre,' ',u.Apellido_1,' ',u.Apellido_2,' (',u.Identificacion,')') AS Nombre
FROM usuario u
WHERE u.Estado = 1
ORDER BY u.Nombre, u.Apellido_1, u.Apellido_2;";

            var rows = await con.QueryAsync<(int Id, string Nombre)>(sql);
            return rows;
        }
    }
}
