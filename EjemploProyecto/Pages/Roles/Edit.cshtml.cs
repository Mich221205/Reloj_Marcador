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
        private readonly IBitacoraService _bitacoraService;

        [BindProperty]
        public Rol Rol { get; set; }


        public EditModel(IRolService rolService, IBitacoraService bitacoraService)
        {
            _rolService = rolService;
            _bitacoraService = bitacoraService;
        }

        public void OnGet(int id)
        {
            Rol = _rolService.ObtenerRoles().FirstOrDefault(r => r.ID_Rol_Usuario == id);
        }

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
                
                var anterior = _rolService.ObtenerRoles()
                    .FirstOrDefault(r => r.ID_Rol_Usuario == Rol.ID_Rol_Usuario);

             
                _rolService.ActualizarRol(Rol);

                
                int idUsuario = 1; // temporal — luego se obtiene del usuario logueado
                await _bitacoraService.Registrar(
                    idUsuario,
                    idAccion: 2,
                    detalle: new { Antes = anterior, Despues = Rol },
                    nombreAccion: "EL rol ha sido actualizado"
                );

           
                ViewData["ModalType"] = "success";
                ViewData["ModalTitle"] = "Actualización exitosa";
                ViewData["ModalMessage"] = "El rol fue actualizado correctamente.";
                ViewData["RedirectPage"] = "Index";
            }
            catch (InvalidOperationException ex)
            {
             
                int idUsuario = 1;
                await _bitacoraService.Registrar(
                    idUsuario,
                    idAccion: 99,
                    detalle: new { Error = ex.Message },
                    nombreAccion: "Error al actualizar Rol"
                );

                ViewData["ModalType"] = "error";
                ViewData["ModalTitle"] = "Error";
                ViewData["ModalMessage"] = ex.Message;
            }

            return Page();
        }
    }
}
