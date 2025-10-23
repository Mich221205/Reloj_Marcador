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
        private readonly IBitacoraService _bitacoraService;


        public CreateModel(IRolService rolService, IBitacoraService bitacoraService)
        {
            _rolService = rolService;
            _bitacoraService = bitacoraService;
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

                int idUsuario = 1; // Temporal, luego vendrá del login
                await _bitacoraService.Registrar(
                    idUsuario,
                    idAccion: 1, // INSERT
                    detalle: Rol,
                    nombreAccion: "Creación de Rol"
                );


                ViewData["ModalType"] = "success";
                ViewData["ModalTitle"] = "Exito";
                ViewData["ModalMessage"] = "Rol creado correctamente.";
                ViewData["RedirectPage"] = "Index";
            }
            catch (InvalidOperationException ex)
            {
                // Registrar error técnico
                int idUsuario = 1;
                await _bitacoraService.Registrar(
                    idUsuario,
                    idAccion: 99, // 99 = error técnico (puedes usar otro id)
                    detalle: new { Error = ex.Message },
                    nombreAccion: "Error al crear Rol"
                );


                ViewData["ModalType"] = "error";
                ViewData["ModalTitle"] = "Error";
                ViewData["ModalMessage"] = ex.Message;
            }

            return Page();
        }
    }
}
