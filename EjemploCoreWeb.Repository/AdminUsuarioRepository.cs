using Dapper;
using EjemploCoreWeb.Entities;
using EjemploCoreWeb.Repository.Interfaces;
using System.Data;

namespace EjemploCoreWeb.Repository.Repositories
{
    /// <summary>
    /// Repositorio PARA ADMIN (HU7–HU9).
    /// No tocar el UsuarioRepository existente (login/cambio de clave).
    /// </summary>
    public class AdminUsuarioRepository : IUsuarioRepository
    {
        private readonly IDbConnectionFactory _db;

        // Clave/IV iguales a los usados por el Login (AES-256-CBC + Base64)
        private const string AES_KEY = "12345678901234567890123456789012"; // 32 chars
        private const string AES_IV = "1234567890123456";                // 16 chars

        public AdminUsuarioRepository(IDbConnectionFactory db)
        {
            _db = db;
        }

        // ---------------------------
        // Helpers
        // ---------------------------
        private static string FilterClause(string? filtro, out object param)
        {
            if (string.IsNullOrWhiteSpace(filtro))
            {
                param = new { };
                return string.Empty;
            }

            param = new { f = $"%{filtro.Trim()}%" };
            return @"
                WHERE  Identificacion LIKE @f
                OR     Nombre         LIKE @f
                OR     Apellido_1     LIKE @f
                OR     Apellido_2     LIKE @f
                OR     Correo         LIKE @f
            ";
        }

        // ---------------------------
        // Listados
        // ---------------------------
        public async Task<IEnumerable<Usuario>> ListarAsync(string? filtro)
        {
            using var cn = _db.CreateConnection();

            var where = FilterClause(filtro, out var p);
            var sql = $@"
                SELECT  Id_Usuario, ID_Tipo_Identificacion, Identificacion,
                        Nombre, Apellido_1, Apellido_2,
                        Correo, Telefono, ID_Rol_Usuario,
                        Contrasena, Estado
                FROM Usuario
                {where}
                ORDER BY Nombre, Apellido_1, Apellido_2;
            ";

            return await cn.QueryAsync<Usuario>(sql, p);
        }

        public async Task<int> ContarAsync(string? filtro)
        {
            using var cn = _db.CreateConnection();

            var where = FilterClause(filtro, out var p);
            var sql = $@"SELECT COUNT(*) FROM Usuario {where};";
            return await cn.ExecuteScalarAsync<int>(sql, p);
        }

        public async Task<IEnumerable<Usuario>> ListarPaginaAsync(string? filtro, int offset, int size)
        {
            using var cn = _db.CreateConnection();

            var where = FilterClause(filtro, out var p);
            var sql = $@"
                SELECT  Id_Usuario, ID_Tipo_Identificacion, Identificacion,
                        Nombre, Apellido_1, Apellido_2,
                        Correo, Telefono, ID_Rol_Usuario,
                        Contrasena, Estado
                FROM Usuario
                {where}
                ORDER BY Nombre, Apellido_1, Apellido_2
                LIMIT @size OFFSET @offset;
            ";

            var args = new DynamicParameters(p);
            args.Add("size", size);
            args.Add("offset", offset);

            return await cn.QueryAsync<Usuario>(sql, args);
        }

        public async Task<(IEnumerable<Usuario> Items, int Total)> ListarPaginadoAsync(string? filtro, int page, int size)
        {
            var total = await ContarAsync(filtro);
            var offset = Math.Max(0, (page - 1) * size);
            var items = await ListarPaginaAsync(filtro, offset, size);
            return (items, total);
        }

        // ---------------------------
        // CRUD
        // ---------------------------
        public async Task<Usuario?> ObtenerAsync(int id)
        {
            using var cn = _db.CreateConnection();
            const string sql = @"
                SELECT  Id_Usuario, ID_Tipo_Identificacion, Identificacion,
                        Nombre, Apellido_1, Apellido_2,
                        Correo, Telefono, ID_Rol_Usuario,
                        Contrasena, Estado
                FROM Usuario
                WHERE Id_Usuario = @id;
            ";
            return await cn.QuerySingleOrDefaultAsync<Usuario>(sql, new { id });
        }

        // Recibe tipoId y plainPassword (se cifra igual que en Login).
        public async Task<int> CrearAsync(Usuario u, int tipoId, string plainPassword)
        {
            using var cn = _db.CreateConnection();

            const string sql = @"
                INSERT INTO Usuario
                    (ID_Tipo_Identificacion, Identificacion, Nombre, Apellido_1, Apellido_2,
                     Correo, Telefono, ID_Rol_Usuario, Contrasena, Estado)
                VALUES
                    (@TipoId, @Identificacion, @Nombre, @Apellido_1, @Apellido_2,
                     @Correo, @Telefono, @Id_Rol_Usuario,
                     TO_BASE64(AES_ENCRYPT(@Pwd, @Key, @IV)),
                     @Estado);
                SELECT LAST_INSERT_ID();
            ";

            return await cn.ExecuteScalarAsync<int>(sql, new
            {
                TipoId = tipoId,
                u.Identificacion,
                u.Nombre,
                u.Apellido_1,
                u.Apellido_2,
                u.Correo,
                u.Telefono,
                Id_Rol_Usuario = u.Id_Rol_Usuario,
                Pwd = plainPassword,
                Key = AES_KEY,
                IV = AES_IV,
                u.Estado
            });
        }

        public async Task<bool> ActualizarAsync(Usuario u, int tipoId, string? plainPassword)
        {
            using var cn = _db.CreateConnection();

            var setPwd = string.IsNullOrWhiteSpace(plainPassword)
                ? ""
                : ", Contrasena = TO_BASE64(AES_ENCRYPT(@Pwd, @Key, @IV))";

            var sql = $@"
                UPDATE Usuario
                   SET ID_Tipo_Identificacion = @TipoId,
                       Identificacion        = @Identificacion,
                       Nombre                = @Nombre,
                       Apellido_1            = @Apellido_1,
                       Apellido_2            = @Apellido_2,
                       Correo                = @Correo,
                       Telefono              = @Telefono,
                       ID_Rol_Usuario        = @Id_Rol_Usuario,
                       Estado                = @Estado
                       {setPwd}
                 WHERE Id_Usuario = @Id_Usuario;
            ";

            var rows = await cn.ExecuteAsync(sql, new
            {
                TipoId = tipoId,
                u.Identificacion,
                u.Nombre,
                u.Apellido_1,
                u.Apellido_2,
                u.Correo,
                u.Telefono,
                Id_Rol_Usuario = u.Id_Rol_Usuario,
                u.Estado,
                Pwd = plainPassword,
                Key = AES_KEY,
                IV = AES_IV,
                u.Id_Usuario
            });

            return rows > 0;
        }

        public async Task<bool> EliminarAsync(int id)
        {
            using var cn = _db.CreateConnection();
            try
            {
                const string sql = @"DELETE FROM Usuario WHERE Id_Usuario = @id;";
                var rows = await cn.ExecuteAsync(sql, new { id });
                return rows > 0;
            }
            catch
            {
                // FK (1451) → el servicio traduce a:
                // "No se puede eliminar un registro con datos relacionados."
                return false;
            }
        }

        // ---------------------------
        // Catálogos
        // ---------------------------
        public async Task<IEnumerable<Rol>> ListarRolesAsync()
        {
            using var cn = _db.CreateConnection();
            const string sql = @"
                SELECT ID_Rol_Usuario, Nombre_Rol
                FROM roles
                ORDER BY Nombre_Rol;
            ";
            return await cn.QueryAsync<Rol>(sql);
        }

        public async Task<IEnumerable<TipoIdentificacion>> ListarTiposAsync()
        {
            using var cn = _db.CreateConnection();
            const string sql = @"
                SELECT ID_Tipo_Identificacion, Tipo_Identificacion
                FROM tipos_identificacion
                ORDER BY Tipo_Identificacion;
            ";
            return await cn.QueryAsync<TipoIdentificacion>(sql);
        }
    }
}
