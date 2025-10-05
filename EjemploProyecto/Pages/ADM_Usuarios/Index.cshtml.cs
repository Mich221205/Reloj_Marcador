using EjemploCoreWeb.Entities;
using EjemploCoreWeb.Services;
using EjemploCoreWeb.Services.Abstract;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Reloj_Marcador.Pages.ADM_Usuarios
{
    public class IndexModel : PageModel
    {

        private readonly IUsuarioService _usuarioService;
        private readonly IBitacoraService _bitacoraService;

        public IndexModel(IUsuarioService usuarioService, IBitacoraService bitacoraService)
        {
            _usuarioService = usuarioService;
            Usuarios = new List<Usuario>();
            _bitacoraService = bitacoraService;
        }

        public IEnumerable<Usuario> Usuarios { get; set; }

        public async Task OnGetAsync()
        {
            Usuarios = await _usuarioService.GetAllAsync();

            await _bitacoraService.Registrar(1, 4, "El usuario consultó administración de funcionarios", "CONSULTA");

        }
    }
}
