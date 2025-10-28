using Dapper;
using EjemploCoreWeb.Entities;
using EjemploCoreWeb.Repository.Interfaces;
using MySql.Data.MySqlClient;

namespace EjemploCoreWeb.Repository.Repositories
{
    public class AreaRepository : IAreaRepository
    {
        private readonly IDbConnectionFactory _factory;
        public AreaRepository(IDbConnectionFactory factory) => _factory = factory;

        public async Task<IEnumerable<Area>> ListarAsync(string? filtro)
        {
            using var con = _factory.CreateConnection();

            var where = @"
(@f IS NULL
 OR a.Nombre_Area LIKE CONCAT('%',@f,'%')
 OR a.Codigo_Area LIKE CONCAT('%',@f,'%')
 OR u.Identificacion LIKE CONCAT('%',@f,'%')
 OR CONCAT(u.Nombre,' ',u.Apellido_1,' ',u.Apellido_2) LIKE CONCAT('%',@f,'%')
)";

            var sql = $@"
SELECT 
    a.ID_Area,
    a.Nombre_Area,
    a.Jefe_Area,
    a.Codigo_Area,
    CONCAT(u.Nombre,' ',u.Apellido_1,' ',u.Apellido_2) AS Jefe_Nombre
FROM areas a
LEFT JOIN usuario u ON u.ID_Usuario = a.Jefe_Area
WHERE {where}
ORDER BY a.Nombre_Area;";

            return await con.QueryAsync<Area>(sql, new { f = string.IsNullOrWhiteSpace(filtro) ? null : filtro!.Trim() });
        }

        public async Task<Area?> ObtenerAsync(int id)
        {
            using var con = _factory.CreateConnection();
            const string sql = @"
SELECT 
    a.ID_Area,
    a.Nombre_Area,
    a.Jefe_Area,
    a.Codigo_Area,
    CONCAT(u.Nombre,' ',u.Apellido_1,' ',u.Apellido_2) AS Jefe_Nombre
FROM areas a
LEFT JOIN usuario u ON u.ID_Usuario = a.Jefe_Area
WHERE a.ID_Area = @id
LIMIT 1;";
            return await con.QueryFirstOrDefaultAsync<Area>(sql, new { id });
        }

        public async Task<int> CrearAsync(Area a)
        {
            using var con = _factory.CreateConnection();
            const string sql = @"
INSERT INTO areas (Nombre_Area, Jefe_Area, Codigo_Area)
VALUES (@Nombre_Area, @Jefe_Area, @Codigo_Area);
SELECT LAST_INSERT_ID();";
            return await con.ExecuteScalarAsync<int>(sql, a);
        }

        public async Task<bool> ActualizarAsync(Area a)
        {
            using var con = _factory.CreateConnection();
            const string sql = @"
UPDATE areas
SET Nombre_Area = @Nombre_Area,
    Jefe_Area   = @Jefe_Area,
    Codigo_Area = @Codigo_Area
WHERE ID_Area = @ID_Area;";
            return await con.ExecuteAsync(sql, a) == 1;
        }

        public async Task<bool> EliminarAsync(int id)
        {
            using var con = _factory.CreateConnection();
            try
            {
                var rows = await con.ExecuteAsync("DELETE FROM areas WHERE ID_Area = @id;", new { id });
                return rows == 1;
            }
            catch (MySqlException ex) when (ex.Number == 1451)
            {
                // Registro con datos relacionados (FK en horario, usuario_area, marca, etc.)
                return false;
            }
        }
    }
}
