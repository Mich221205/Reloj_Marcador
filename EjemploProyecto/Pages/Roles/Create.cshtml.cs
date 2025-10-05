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

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                var errores = string.Join("<br>",
                    ModelState.Values.SelectMany(v => v.Errors)
                                     .Select(e => e.ErrorMessage));

                ViewData["ModalType"] = "error";
                ViewData["ModalTitle"] = "Error de validación";
                ViewData["ModalMessage"] = errores;

                return Page();
            }

            try
            {
                _rolService.CrearRol(Rol);

                ViewData["ModalType"] = "success";
                ViewData["ModalTitle"] = "Éxito";
                ViewData["ModalMessage"] = "Rol creado correctamente.";
                ViewData["RedirectPage"] = "Index";
            }
            catch (InvalidOperationException ex)
            {
                ViewData["ModalType"] = "error";
                ViewData["ModalTitle"] = "Error";
                ViewData["ModalMessage"] = ex.Message;
            }

            return Page();
        }
    }
}
