using System.Security.Claims;
using System.Data;
using Dapper;
using Microsoft.AspNetCore.Http;
using EjemploCoreWeb.Repository;

namespace EjemploProyecto.Services
{
    public class KarinaBitacoraService : EjemploCoreWeb.Services.Abstract.IBitacoraService
    {
        private readonly IDbConnectionFactory _db;
        private readonly IHttpContextAccessor _http;

        public KarinaBitacoraService(IDbConnectionFactory db, IHttpContextAccessor http)
        {
            _db = db;
            _http = http;
        }

        public async Task Registrar(int idUsuario, int idAccion, object detalle, string descripcion)
        {
            if (idUsuario <= 0) idUsuario = await ResolveActorAsync();
            if (idUsuario <= 0) idUsuario = await GetFirstUserIdAsync();

            var payload = new { Accion = descripcion, Detalle = detalle };

            const string SQL = @"
INSERT INTO bitacora (Fecha_Registro, ID_Usuario, ID_Accion, Descripcion_Accion)
VALUES (CURDATE(), @ID_Usuario, @ID_Accion, @Descripcion);";

            using var cn = _db.CreateConnection();
            await cn.ExecuteAsync(SQL, new
            {
                ID_Usuario = idUsuario,
                ID_Accion = idAccion,
                Descripcion = System.Text.Json.JsonSerializer.Serialize(payload)
            });
        }

        public Task RegistrarCrearAsync(object? detalle, int? actorId = null)
            => Registrar(actorId ?? 0, 1, detalle ?? new { }, "CREAR");

        public Task RegistrarActualizarAsync(object? detalle, int? actorId = null)
            => Registrar(actorId ?? 0, 3, detalle ?? new { }, "UPDATE");

        public Task RegistrarEliminarAsync(object? detalle, int? actorId = null)
            => Registrar(actorId ?? 0, 2, detalle ?? new { }, "ELIMINAR");

        public Task RegistrarConsultaAsync(object? detalle, int? actorId = null)
            => Registrar(actorId ?? 0, 4, detalle ?? new { }, "CONSULTA");

        public Task RegistrarErrorAsync(string mensaje, object? detalle = null, int? actorId = null)
            => Registrar(actorId ?? 0, 5, new { Mensaje = mensaje, Detalle = detalle }, "ERROR");

        private async Task<int> ResolveActorAsync()
        {
            var ctx = _http.HttpContext;
            if (ctx == null) return 0;

            var c = ctx.User.FindFirstValue(ClaimTypes.NameIdentifier)
                 ?? ctx.User.FindFirstValue("ID_Usuario")
                 ?? ctx.User.FindFirstValue("id_usuario");

            if (int.TryParse(c, out var idC) && idC > 0) return idC;

            var sid = ctx.Session?.GetInt32("ID_Usuario");
            if (sid.HasValue && sid.Value > 0) return sid.Value;

            var identSession = ctx.Session?.GetString("Identificacion");
            if (!string.IsNullOrWhiteSpace(identSession))
            {
                using var cn = _db.CreateConnection();
                var fromSession = await cn.ExecuteScalarAsync<int?>(
                    "SELECT ID_Usuario FROM usuario WHERE Identificacion = @i LIMIT 1;",
                    new { i = identSession });
                if (fromSession.HasValue && fromSession.Value > 0) return fromSession.Value;
            }

            var username = ctx.User?.Identity?.Name;
            if (!string.IsNullOrWhiteSpace(username))
            {
                using var cn = _db.CreateConnection();
                var fromName = await cn.ExecuteScalarAsync<int?>(
                    "SELECT ID_Usuario FROM usuario WHERE Identificacion = @i LIMIT 1;",
                    new { i = username });
                if (fromName.HasValue && fromName.Value > 0) return fromName.Value;
            }

            return 0;
        }

        private async Task<int> GetFirstUserIdAsync()
        {
            using var cn = _db.CreateConnection();
            return await cn.ExecuteScalarAsync<int?>(
                "SELECT ID_Usuario FROM usuario ORDER BY ID_Usuario ASC LIMIT 1;"
            ) ?? 0;
        }
    }
}
