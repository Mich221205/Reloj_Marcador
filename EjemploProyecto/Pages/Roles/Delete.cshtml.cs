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

                int idUsuario = 1;
                await _bitacoraService.Registrar(
                    idUsuario,
                    idAccion: 3,
                    detalle: eliminado,
                    nombreAccion: "Eliminación de Rol"
                );

                ViewData["ModalType"] = "success";
                ViewData["ModalTitle"] = "Eliminación exitosa";
                ViewData["ModalMessage"] = "El rol fue eliminado correctamente.";
                ViewData["RedirectPage"] = "Index";
            }
            catch (MySql.Data.MySqlClient.MySqlException ex)
            {
                // ⚠️ Si el error es por restricción FK (rol asignado a usuarios)
                if (ex.Number == 1451)
                {
                    ViewData["ModalType"] = "error";
                    ViewData["ModalTitle"] = "Error";
                    ViewData["ModalMessage"] = "No se puede eliminar un registro con datos relacionados.";
                }
                
                // Registrar el error técnico en bitácora
                int idUsuario = 1;
                await _bitacoraService.Registrar(
                    idUsuario,
                    idAccion: 99,
                    detalle: new { Error = ex.Message },
                    nombreAccion: "Error al eliminar Rol"
                );
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
