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

        public IndexModel(IRolService rolService)
        {
            _rolService = rolService;
        }

        public IEnumerable<Rol> Roles { get; set; }

        public void OnGet()
        {
            Roles = _rolService.ObtenerRoles();
        }
    }
}
