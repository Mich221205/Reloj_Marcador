using Microsoft.AspNetCore.Mvc.RazorPages;
using EjemploCoreWeb.Entities;
using EjemploCoreWeb.Services;
using System.Collections.Generic;
using EjemploCoreWeb.Services.Abstract;

namespace EjemploProyecto.Pages.Roles
{
    public class IndexModel : PageModel
    {
        private readonly IRolService _rolService;
        private readonly IBitacoraService _bitacoraService;


        public IndexModel(IRolService rolService, IBitacoraService bitacoraService)
        {
            _rolService = rolService;
            _bitacoraService = bitacoraService;
        }

        public IEnumerable<Rol> Roles { get; set; }

        public async Task OnGetAsync()
        {
            Roles = _rolService.ObtenerRoles();


            int idUsuario = 1; //  temporal, luego vendrá del usuario logueado
            await _bitacoraService.Registrar(
                idUsuario,
                idAccion: 4,
                detalle: null,
                nombreAccion: "El usuario consulta Roles"
            );
        }
    }
}
