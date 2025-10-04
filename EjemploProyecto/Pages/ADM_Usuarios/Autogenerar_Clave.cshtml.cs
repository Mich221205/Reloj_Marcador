using EjemploCoreWeb.Entities;
using EjemploCoreWeb.Services.Abstract;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace EjemploProyecto.Pages.ADM_Usuarios
{
    public class Autogenerar_ClaveModel : PageModel
    {
        private readonly IUsuarioService _usuarioService;

        public Autogenerar_ClaveModel(IUsuarioService usuarioService)
        {
            _usuarioService = usuarioService;
            Usuario = new Usuario();
        }

        [BindProperty]
        public Usuario Usuario { get; set; }

        public async Task<IActionResult> OnGetAsync(string id)
        {
            Usuario = await _usuarioService.Obtener_Usuario_X_Identificacion(id);

            if (Usuario == null)
                return NotFound();

            // Autogenerar la clave apenas se carga la página
            Usuario.Contrasena = _usuarioService.Autogenerar_Clave();

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            try
            {
                var rows = await _usuarioService.Cambiar_Clave(Usuario);

                if (rows > 0)
                {
                    ViewData["ModalType"] = "success";
                    ViewData["ModalTitle"] = "Éxito";
                    ViewData["ModalMessage"] = "Contraseña cambiada correctamente.";
                }
                else
                {
                    ViewData["ModalType"] = "error";
                    ViewData["ModalTitle"] = "Error";
                    ViewData["ModalMessage"] = "No se encontró el usuario a actualizar.";
                }
            }
            catch (Exception ex)
            {
                ViewData["ModalType"] = "error";
                ViewData["ModalTitle"] = "Error";
                ViewData["ModalMessage"] = $"No se pudo actualizar la contraseña: {ex.Message}";
            }

            return Page();
        }
    }
}
