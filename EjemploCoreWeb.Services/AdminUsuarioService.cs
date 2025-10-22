using System.Text.RegularExpressions;
using EjemploCoreWeb.Entities;
using EjemploCoreWeb.Repository.Interfaces;
using EjemploCoreWeb.Services.Interfaces;

namespace EjemploCoreWeb.Services.Services
{
    public class AdminUsuarioService : IUsuarioService
    {
        private readonly IUsuarioRepository _repo;

        public AdminUsuarioService(IUsuarioRepository repo)
        {
            _repo = repo;
        }

        private static readonly Regex SoloLetrasEspacios50 =
            new(@"^[A-Za-zÁÉÍÓÚáéíóúñÑ ]{1,50}$", RegexOptions.Compiled);

        private static void ValidarUsuarioHU7(Usuario u)
        {
            if (u is null) throw new ArgumentNullException(nameof(u));
            if (string.IsNullOrWhiteSpace(u.Nombre) || !SoloLetrasEspacios50.IsMatch(u.Nombre))
                throw new ArgumentException("El nombre debe tener máximo 50 caracteres y solo letras y espacios.");
            if (string.IsNullOrWhiteSpace(u.Apellido_1) || !SoloLetrasEspacios50.IsMatch(u.Apellido_1))
                throw new ArgumentException("El primer apellido debe tener máximo 50 caracteres y solo letras y espacios.");
            if (string.IsNullOrWhiteSpace(u.Apellido_2) || !SoloLetrasEspacios50.IsMatch(u.Apellido_2))
                throw new ArgumentException("El segundo apellido debe tener máximo 50 caracteres y solo letras y espacios.");
            if (string.IsNullOrWhiteSpace(u.Correo) || !u.Correo.Contains('@') || u.Correo.Length > 100)
                throw new ArgumentException("El correo no es válido o excede 100 caracteres.");
            if (string.IsNullOrWhiteSpace(u.Identificacion) || u.Identificacion.Length > 20)
                throw new ArgumentException("La identificación es obligatoria y no debe exceder 20 caracteres.");
            if (u.Id_Rol_Usuario <= 0)
                throw new ArgumentException("Debe seleccionar un rol.");
        }

        public Task<(IEnumerable<Usuario> Items, int Total)> ListarPaginadoAsync(string? filtro, int page, int size)
            => _repo.ListarPaginadoAsync(filtro, page, size);

        public Task<IEnumerable<Usuario>> ListarAsync(string? filtro)
            => _repo.ListarAsync(filtro);

        public Task<int> ContarAsync(string? filtro)
            => _repo.ContarAsync(filtro);

        public Task<Usuario?> ObtenerAsync(int id)
            => _repo.ObtenerAsync(id);

        public Task<IEnumerable<Rol>> ListarRolesAsync() => _repo.ListarRolesAsync();
        public Task<IEnumerable<TipoIdentificacion>> ListarTiposAsync() => _repo.ListarTiposAsync();

        public async Task<int> CrearAsync(Usuario u, int tipoId, string plainPassword)
        {
            ValidarUsuarioHU7(u);
            if (tipoId <= 0) throw new ArgumentException("Debe seleccionar un tipo de identificación.");
            if (string.IsNullOrWhiteSpace(plainPassword)) throw new ArgumentException("La contraseña es obligatoria.");

            await LogAsync("INSERT", "Usuario", new { u.Identificacion, u.Nombre, u.Apellido_1, u.Apellido_2, u.Correo, u.Id_Rol_Usuario, tipoId, u.Estado });
            var id = await _repo.CrearAsync(u, tipoId, plainPassword);
            await LogAsync("INSERT-OK", "Usuario", new { IdNuevo = id });
            return id;
        }

        public async Task<bool> ActualizarAsync(Usuario u, int tipoId, string? plainPassword)
        {
            if (u.Id_Usuario <= 0) throw new ArgumentException("El Id_Usuario es obligatorio.");
            ValidarUsuarioHU7(u);
            if (tipoId <= 0) throw new ArgumentException("Debe seleccionar un tipo de identificación.");

            await LogAsync("UPDATE", "Usuario", new
            {
                u.Id_Usuario,
                u.Identificacion,
                u.Nombre,
                u.Apellido_1,
                u.Apellido_2,
                u.Correo,
                u.Id_Rol_Usuario,
                tipoId,
                u.Estado,
                CambiaPassword = !string.IsNullOrWhiteSpace(plainPassword)
            });

            var ok = await _repo.ActualizarAsync(u, tipoId, plainPassword);
            await LogAsync(ok ? "UPDATE-OK" : "UPDATE-FAIL", "Usuario", new { u.Id_Usuario });
            return ok;
        }

        public async Task<bool> EliminarAsync(int id)
        {
            if (id <= 0)
                throw new ArgumentException("El id es obligatorio.");

            await LogAsync("DELETE", "Usuario", new { Id = id });

            var ok = await _repo.EliminarAsync(id);
            if (!ok) return false;

            await LogAsync("DELETE-OK", "Usuario", new { Id = id });
            return true;
        }


        private static Task LogAsync(string accion, string entidad, object? data) => Task.CompletedTask;
    }
}
