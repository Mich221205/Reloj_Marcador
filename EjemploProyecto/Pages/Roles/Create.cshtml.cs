using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using EjemploCoreWeb.Entities;
using EjemploCoreWeb.Services;
using EjemploCoreWeb.Services.Abstract;

namespace EjemploProyecto.Pages.Roles
{
    public class CreateModel : PageModel
    {
        private readonly IRolService _rolService;

        public CreateModel(IRolService rolService)
        {
            _rolService = rolService;
        }

        [BindProperty]
        public Rol Rol { get; set; }

        public void OnGet() { }

        public IActionResult OnPost()
        {
            if (!ModelState.IsValid)
                return Page();

            _rolService.CrearRol(Rol);
            return RedirectToPage("Index");
        }
    }
}
