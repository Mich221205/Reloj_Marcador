using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using EjemploCoreWeb.Entities;
using EjemploCoreWeb.Services;
using System.Linq;
using EjemploCoreWeb.Services.Abstract;

namespace EjemploProyecto.Pages.Roles
{
    public class EditModel : PageModel
    {
        private readonly IRolService _rolService;

        public EditModel(IRolService rolService)
        {
            _rolService = rolService;
        }

        [BindProperty]
        public Rol Rol { get; set; }

        public void OnGet(int id)
        {
            Rol = _rolService.ObtenerRoles().FirstOrDefault(r => r.ID_Rol_Usuario == id);
        }

        public IActionResult OnPost()
        {
            if (!ModelState.IsValid)
                return Page();

            _rolService.ActualizarRol(Rol);
            return RedirectToPage("Index");
        }
    }
}
