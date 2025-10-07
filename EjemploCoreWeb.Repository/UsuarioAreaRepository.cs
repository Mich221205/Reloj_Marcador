using Dapper;
using EjemploCoreWeb.Entities;
using EjemploCoreWeb.Repository.Interfaces;
using MySql.Data.MySqlClient;
using System.Data;

namespace EjemploCoreWeb.Repository.Repositories;

public class UsuarioAreaRepository : IUsuarioAreaRepository
{
    private readonly IDbConnectionFactory _factory;
    public UsuarioAreaRepository(IDbConnectionFactory factory) => _factory = factory;

    public async Task<IEnumerable<UsuarioArea>> ListarPorUsuarioAsync(int idUsuario)
    {
        using var con = _factory.CreateConnection();
        var sql = @"
SELECT ua.ID_Usuario, ua.ID_Area, a.Nombre_Area
FROM Usuario_Area ua
JOIN Areas a ON a.ID_Area = ua.ID_Area
WHERE ua.ID_Usuario=@id
ORDER BY a.Nombre_Area;";
        return await con.QueryAsync<UsuarioArea>(sql, new { id = idUsuario });
    }

    public async Task<IEnumerable<Area>> ListarNoAsociadasAsync(int idUsuario)
    {
        using var con = _factory.CreateConnection();
        var sql = @"
SELECT a.ID_Area, a.Nombre_Area, a.Jefe_Area
FROM Areas a
WHERE a.ID_Area NOT IN (SELECT ID_Area FROM Usuario_Area WHERE ID_Usuario=@id)
ORDER BY a.Nombre_Area;";
        return await con.QueryAsync<Area>(sql, new { id = idUsuario });
    }

    public async Task<bool> AsociarAsync(int idUsuario, int idArea)
    {
        using var con = _factory.CreateConnection();
        var rows = await con.ExecuteAsync(
            "INSERT IGNORE INTO Usuario_Area (ID_Usuario, ID_Area) VALUES (@u,@a);",
            new { u = idUsuario, a = idArea });
        return rows >= 0;
    }

    public async Task<bool> DesasociarAsync(int idUsuario, int idArea)
    {
        using var con = _factory.CreateConnection();
        try
        {
            var rows = await con.ExecuteAsync(
                "DELETE FROM Usuario_Area WHERE ID_Usuario=@u AND ID_Area=@a;",
                new { u = idUsuario, a = idArea });
            return rows == 1;
        }
        catch (MySqlException ex) when (ex.Number == 1451)
        {
            return false;
        }
    }
}
