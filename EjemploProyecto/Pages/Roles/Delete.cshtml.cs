using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using EjemploCoreWeb.Entities;
using EjemploCoreWeb.Services;
using System.Linq;
using EjemploCoreWeb.Services.Abstract;

namespace EjemploProyecto.Pages.Roles
{
    public class DeleteModel : PageModel
    {
        private readonly IRolService _rolService;

        public DeleteModel(IRolService rolService)
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
            try
            {
                _rolService.EliminarRol(Rol.ID_Rol_Usuario);

                ViewData["ModalType"] = "success";
                ViewData["ModalTitle"] = "Eliminación exitosa";
                ViewData["ModalMessage"] = "El rol fue eliminado correctamente.";
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
