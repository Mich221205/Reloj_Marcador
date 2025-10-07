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
        private readonly IBitacoraService _bitacoraService;


        public DeleteModel(IRolService rolService, IBitacoraService bitacoraService)
        {
            _rolService = rolService;
            _bitacoraService = bitacoraService;
        }

        [BindProperty]
        public Rol Rol { get; set; }

        public void OnGet(int id)
        {
            Rol = _rolService.ObtenerRoles().FirstOrDefault(r => r.ID_Rol_Usuario == id);
        }

        public async Task<IActionResult> OnPostAsync()
        {
            try
            {
                var eliminado = _rolService.ObtenerRoles()
                    .FirstOrDefault(r => r.ID_Rol_Usuario == Rol.ID_Rol_Usuario);

                _rolService.EliminarRol(Rol.ID_Rol_Usuario);

                // Registrar en bitácora (versión original)
                await _bitacoraService.Registrar(1, 3, eliminado, "DELETE");

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
