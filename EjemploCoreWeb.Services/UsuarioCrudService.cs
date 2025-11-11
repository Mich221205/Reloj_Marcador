using System.Data;
using Dapper;
using EjemploCoreWeb.Entities;
using EjemploCoreWeb.Repository;

namespace EjemploCoreWeb.Services
{
    public class UsuarioCrudService : Interfaces.IUsuarioService
    {
        private readonly IDbConnectionFactory _connectionFactory;

        public UsuarioCrudService(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<IEnumerable<Rol>> ListarRolesAsync()
        {
            const string sql = @"SELECT ID_Rol_Usuario, Nombre_Rol
                                 FROM roles
                                 ORDER BY Nombre_Rol;";
            using var cn = _connectionFactory.CreateConnection();
            return await cn.QueryAsync<Rol>(sql);
        }

        public async Task<IEnumerable<TipoIdentificacion>> ListarTiposAsync()
        {
            const string sql = @"SELECT ID_Tipo_Identificacion, Tipo_Identificacion
                                 FROM tipos_identificacion
                                 ORDER BY Tipo_Identificacion;";
            using var cn = _connectionFactory.CreateConnection();
            return await cn.QueryAsync<TipoIdentificacion>(sql);
        }

        public async Task<int> CrearAsync(Usuario u, int tipoId, string plainPassword)
        {
            if (u is null) throw new ArgumentNullException(nameof(u));
            if (string.IsNullOrWhiteSpace(u.Identificacion))
                throw new ArgumentException("La identificación es obligatoria.");
            if (string.IsNullOrWhiteSpace(u.Nombre))
                throw new ArgumentException("El nombre es obligatorio.");
            if (string.IsNullOrWhiteSpace(u.Apellido_1))
                throw new ArgumentException("El primer apellido es obligatorio.");
            if (string.IsNullOrWhiteSpace(u.Correo))
                throw new ArgumentException("El correo es obligatorio.");
            if (string.IsNullOrWhiteSpace(plainPassword))
                throw new ArgumentException("La contraseña es obligatoria.");

            object? apellido2Db = string.IsNullOrWhiteSpace(u.Apellido_2)
                ? DBNull.Value
                : u.Apellido_2!.Trim();

            var hash = plainPassword;

            const string sql = @"
INSERT INTO usuario
    (ID_Tipo_Identificacion, Identificacion, Nombre, Apellido_1, Apellido_2, Correo, ID_Rol_Usuario, Contrasena, Estado)
VALUES
    (@Tipo, @Identificacion, @Nombre, @Apellido1, @Apellido2, @Correo, @Rol, @Contrasena, @Estado);
SELECT LAST_INSERT_ID();";

            using var cn = _connectionFactory.CreateConnection();
            var id = await cn.ExecuteScalarAsync<int>(sql, new
            {
                Tipo = tipoId,
                u.Identificacion,
                u.Nombre,
                Apellido1 = u.Apellido_1,
                Apellido2 = apellido2Db,
                u.Correo,
                Rol = u.Id_Rol_Usuario,
                Contrasena = hash,
                u.Estado
            });

            return id;
        }

        public async Task<bool> ActualizarAsync(Usuario u, int tipoId, string? plainPassword)
        {
            if (u is null) throw new ArgumentNullException(nameof(u));
            if (u.ID_Usuario <= 0) throw new ArgumentException("ID inválido.");

            object? apellido2Db = string.IsNullOrWhiteSpace(u.Apellido_2)
                ? DBNull.Value
                : u.Apellido_2!.Trim();

            const string sql = @"
UPDATE usuario
SET ID_Tipo_Identificacion = @Tipo,
    Identificacion         = @Identificacion,
    Nombre                 = @Nombre,
    Apellido_1             = @Apellido1,
    Apellido_2             = @Apellido2,
    Correo                 = @Correo,
    ID_Rol_Usuario         = @Rol,
    Estado                 = @Estado
WHERE ID_Usuario = @Id;";

            using var cn = _connectionFactory.CreateConnection();
            var rows = await cn.ExecuteAsync(sql, new
            {
                Id = u.ID_Usuario,
                Tipo = tipoId,
                u.Identificacion,
                u.Nombre,
                Apellido1 = u.Apellido_1,
                Apellido2 = apellido2Db,
                u.Correo,
                Rol = u.Id_Rol_Usuario,
                u.Estado
            });

            if (!string.IsNullOrWhiteSpace(plainPassword))
            {
                const string sqlPwd = "UPDATE usuario SET Contrasena=@Pwd WHERE ID_Usuario=@Id;";
                await cn.ExecuteAsync(sqlPwd, new { Id = u.ID_Usuario, Pwd = plainPassword });
            }

            return rows > 0;
        }

        public async Task<bool> EliminarAsync(int id)
        {
            const string sql = @"DELETE FROM usuario WHERE ID_Usuario = @Id;";
            using var cn = _connectionFactory.CreateConnection();
            var rows = await cn.ExecuteAsync(sql, new { Id = id });
            return rows > 0;
        }

        public async Task<Usuario?> ObtenerAsync(int id)
        {
            const string sql = @"SELECT ID_Usuario AS Id_Usuario,
                                        ID_Tipo_Identificacion,
                                        Identificacion,
                                        Nombre,
                                        Apellido_1,
                                        Apellido_2,
                                        Correo,
                                        Telefono,
                                        ID_Rol_Usuario AS Id_Rol_Usuario,
                                        Contrasena,
                                        Estado
                                 FROM usuario
                                 WHERE ID_Usuario = @Id;";
            using var cn = _connectionFactory.CreateConnection();
            return await cn.QuerySingleOrDefaultAsync<Usuario>(sql, new { Id = id });
        }

        public async Task<IEnumerable<Usuario>> ListarAsync(string? filtro)
        {
            using var cn = _connectionFactory.CreateConnection();
            if (string.IsNullOrWhiteSpace(filtro))
            {
                return await cn.QueryAsync<Usuario>(@"
SELECT ID_Usuario AS Id_Usuario, ID_Tipo_Identificacion, Identificacion, Nombre, Apellido_1, Apellido_2,
       Correo, Telefono, ID_Rol_Usuario AS Id_Rol_Usuario, Contrasena, Estado
FROM usuario
ORDER BY Nombre;");
            }
            else
            {
                return await cn.QueryAsync<Usuario>(@"
SELECT ID_Usuario AS Id_Usuario, ID_Tipo_Identificacion, Identificacion, Nombre, Apellido_1, Apellido_2,
       Correo, Telefono, ID_Rol_Usuario AS Id_Rol_Usuario, Contrasena, Estado
FROM usuario
WHERE Identificacion LIKE @q OR Nombre LIKE @q OR Apellido_1 LIKE @q OR Apellido_2 LIKE @q OR Correo LIKE @q
ORDER BY Nombre;", new { q = $"%{filtro.Trim()}%" });
            }
        }

        public async Task<(IEnumerable<Usuario> Items, int Total)> ListarPaginadoAsync(string? filtro, int page, int size)
        {
            var offset = Math.Max(page - 1, 0) * Math.Max(size, 1);
            var limit = Math.Max(size, 1);

            string where = string.IsNullOrWhiteSpace(filtro)
                ? ""
                : "WHERE Identificacion LIKE @q OR Nombre LIKE @q OR Apellido_1 LIKE @q OR Apellido_2 LIKE @q OR Correo LIKE @q";

            using var cn = _connectionFactory.CreateConnection();

            var count = await cn.ExecuteScalarAsync<int>($"SELECT COUNT(*) FROM usuario {where};",
                string.IsNullOrWhiteSpace(filtro) ? null : new { q = $"%{filtro!.Trim()}%" });

            var items = await cn.QueryAsync<Usuario>($@"
SELECT ID_Usuario AS Id_Usuario, ID_Tipo_Identificacion, Identificacion, Nombre, Apellido_1, Apellido_2,
       Correo, Telefono, ID_Rol_Usuario AS Id_Rol_Usuario, Contrasena, Estado
FROM usuario
{where}
ORDER BY Nombre
LIMIT @limit OFFSET @offset;",
                string.IsNullOrWhiteSpace(filtro)
                    ? new { limit, offset }
                    : new { q = $"%{filtro!.Trim()}%", limit, offset });

            return (items, count);
        }

        public async Task<int> ContarAsync(string? filtro)
        {
            using var cn = _connectionFactory.CreateConnection();
            if (string.IsNullOrWhiteSpace(filtro))
                return await cn.ExecuteScalarAsync<int>("SELECT COUNT(*) FROM usuario;");
            return await cn.ExecuteScalarAsync<int>(@"
SELECT COUNT(*)
FROM usuario
WHERE Identificacion LIKE @q OR Nombre LIKE @q OR Apellido_1 LIKE @q OR Apellido_2 LIKE @q OR Correo LIKE @q;",
                new { q = $"%{filtro!.Trim()}%" });
        }

        public async Task<int?> ObtenerIdPorIdentificacionAsync(string identificacion)
        {
            const string sql = @"SELECT ID_Usuario 
                                 FROM usuario 
                                 WHERE Identificacion = @ident 
                                 LIMIT 1;";
            using var cn = _connectionFactory.CreateConnection();
            var id = await cn.ExecuteScalarAsync<int?>(sql, new { ident = identificacion });
            return id;
        }

        public async Task<Usuario?> ObtenerPorIdentificacionAsync(string identificacion)
        {
            const string sql = @"SELECT ID_Usuario      AS Id_Usuario,
                                        ID_Tipo_Identificacion,
                                        Identificacion,
                                        Nombre,
                                        Apellido_1,
                                        Apellido_2,
                                        Correo,
                                        Telefono,
                                        ID_Rol_Usuario  AS Id_Rol_Usuario,
                                        Contrasena,
                                        Estado
                                 FROM usuario
                                 WHERE Identificacion = @ident
                                 LIMIT 1;";
            using var cn = _connectionFactory.CreateConnection();
            return await cn.QuerySingleOrDefaultAsync<Usuario>(sql, new { ident = identificacion });
        }
    }
}
