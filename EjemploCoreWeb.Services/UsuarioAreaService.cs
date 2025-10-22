using EjemploCoreWeb.Entities;
using EjemploCoreWeb.Repository.Interfaces;
using EjemploCoreWeb.Services.Interfaces;

namespace EjemploCoreWeb.Services.Services
{
    public class UsuarioAreaService : IUsuarioAreaService
    {
        private readonly IUsuarioAreaRepository _repo;
        private readonly IServiceProvider _sp;

        public UsuarioAreaService(IUsuarioAreaRepository repo, IServiceProvider sp)
        {
            _repo = repo;
            _sp = sp;
        }

        // Bitácora opcional: se resuelve dinámicamente para no acoplar fuerte.
        private async Task TryLogAsync(string accion, object data)
        {
            try
            {
                var t = Type.GetType("EjemploCoreWeb.Services.Abstract.IBitacoraService, EjemploCoreWeb.Services");
                if (t != null)
                {
                    dynamic? bit = _sp.GetService(t);
                    if (bit != null) await bit.RegistrarAsync(accion, data);
                }
            }
            catch
            {
                // Silenciar errores de bitácora para no romper el flujo funcional.
            }
        }

        public Task<IEnumerable<UsuarioArea>> ListarPorUsuarioAsync(int idUsuario)
        {
            if (idUsuario <= 0) throw new ArgumentOutOfRangeException(nameof(idUsuario));
            return _repo.ListarPorUsuarioAsync(idUsuario);
        }

        public Task<IEnumerable<Area>> ListarNoAsociadasAsync(int idUsuario)
        {
            if (idUsuario <= 0) throw new ArgumentOutOfRangeException(nameof(idUsuario));
            return _repo.ListarNoAsociadasAsync(idUsuario);
        }

        public async Task<bool> AsociarAsync(int idUsuario, int idArea)
        {
            if (idUsuario <= 0) throw new ArgumentOutOfRangeException(nameof(idUsuario));
            if (idArea <= 0) throw new ArgumentOutOfRangeException(nameof(idArea));

            var ok = await _repo.AsociarAsync(idUsuario, idArea);
            if (ok)
                await TryLogAsync("Asociar Área a Usuario", new { ID_Usuario = idUsuario, ID_Area = idArea });

            return ok;
        }

        public async Task<bool> DesasociarAsync(int idUsuario, int idArea)
        {
            if (idUsuario <= 0) throw new ArgumentOutOfRangeException(nameof(idUsuario));
            if (idArea <= 0) throw new ArgumentOutOfRangeException(nameof(idArea));

            var ok = await _repo.DesasociarAsync(idUsuario, idArea);
            await TryLogAsync(ok ? "Desasociar Área de Usuario" : "Desasociar Área (bloqueado por FK)",
                new { ID_Usuario = idUsuario, ID_Area = idArea, Ok = ok });

            return ok;
        }
    }
}
