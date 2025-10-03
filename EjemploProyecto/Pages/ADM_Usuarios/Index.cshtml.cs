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

        public IndexModel(IUsuarioService usuarioService)
        {
            _usuarioService = usuarioService;
            Usuarios = new List<Usuario>();
        }

        public IEnumerable<Usuario> Usuarios { get; set; }

        public async Task OnGetAsync()
        {
            Usuarios = await _usuarioService.GetAllAsync();
        }
    }
}
