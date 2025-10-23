using EjemploCoreWeb.Entities;
using EjemploCoreWeb.Services.Abstract;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace EjemploProyecto.Pages.ADM_Usuarios
{
    public class Autogenerar_ClaveModel : PageModel
    {
        private readonly IUsuarioService _usuarioService;
        private readonly IBitacoraService _bitacoraService;

        public Autogenerar_ClaveModel(IUsuarioService usuarioService, IBitacoraService bitacoraService)
        {
            _usuarioService = usuarioService;
            Usuario = new Usuario();
            _bitacoraService = bitacoraService;
        }

        [BindProperty]
        public Usuario Usuario { get; set; }

        public async Task<IActionResult> OnGetAsync(string id)
        {
            Usuario = await _usuarioService.Obtener_Usuario_X_Identificacion(id);

            await _bitacoraService.Registrar(1, 4, "El usuario consultó autogenerar", "CONSULTA");

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
                var anterior = await _usuarioService.Obtener_Usuario_X_Identificacion(Usuario.Identificacion);

                var rows = await _usuarioService.Cambiar_Clave(Usuario);

                await _bitacoraService.Registrar(1, 2, new { Antes = new { Contrasena = anterior.Contrasena }, Despues = new { Contrasena = Usuario.Contrasena } }, "UPDATE");

                if (rows > 0)
                {
                    ViewData["ModalType"] = "success";
                    ViewData["ModalTitle"] = "Exito";
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
