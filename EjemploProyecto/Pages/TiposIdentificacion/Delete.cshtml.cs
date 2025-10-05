using EjemploCoreWeb.Entities;
using EjemploCoreWeb.Services.Abstract;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Linq;
using System.Threading.Tasks;

namespace EjemploProyecto.Pages.ADM_Identificacion
{
    public class DeleteModel : PageModel
    {
        private readonly ITipoIdentificacionService _service;
        private readonly IBitacoraService _bitacoraService;

        public DeleteModel(ITipoIdentificacionService service, IBitacoraService bitacoraService)
        {
            _service = service;
            _bitacoraService = bitacoraService;
        }

        // 🔹 Se permite null para evitar advertencias de referencia nula (CS8600)
        [BindProperty]
        public TipoIdentificacion? Tipo { get; set; }

        public void OnGet(int id)
        {
            Tipo = _service.ObtenerTodos().FirstOrDefault(t => t.ID_Tipo_Identificacion == id);
        }

        public async Task<IActionResult> OnPostAsync()
        {
            try
            {
                // 🔹 Validar que el tipo exista antes de eliminar
                var eliminado = _service.ObtenerTodos()
                    .FirstOrDefault(t => t.ID_Tipo_Identificacion == Tipo?.ID_Tipo_Identificacion);

                if (eliminado == null)
                {
                    ViewData["ModalType"] = "error";
                    ViewData["ModalTitle"] = "Error";
                    ViewData["ModalMessage"] = "No se encontró el registro a eliminar.";
                    return Page();
                }

                // 🔹 Eliminar el registro
                _service.Eliminar(eliminado.ID_Tipo_Identificacion);

                // 🔹 Registrar la acción en bitácora (acción 3 = eliminación)
                int idUsuario = 1; // ⚠️ temporal, luego vendrá del login real
                await _bitacoraService.Registrar(
                    idUsuario,
                    idAccion: 3,
                    detalle: eliminado,
                    nombreAccion: "Eliminación de Tipo de Identificación"
                );

                // 🔹 Modal de éxito
                ViewData["ModalType"] = "success";
                ViewData["ModalTitle"] = "Eliminación exitosa";
                ViewData["ModalMessage"] = "El tipo de identificación fue eliminado correctamente.";
                ViewData["RedirectPage"] = "Index";
            }
            catch (InvalidOperationException ex)
            {
                // 🔹 Registrar error técnico en bitácora
                int idUsuario = 1;
                await _bitacoraService.Registrar(
                    idUsuario,
                    idAccion: 99, // 99 = error técnico
                    detalle: new { Error = ex.Message },
                    nombreAccion: "Error al eliminar Tipo de Identificación"
                );

                ViewData["ModalType"] = "error";
                ViewData["ModalTitle"] = "Error";
                ViewData["ModalMessage"] = ex.Message;
            }

            return Page();
        }
    }
}
