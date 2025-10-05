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
                var eliminado = _service.ObtenerTodos()
                    .FirstOrDefault(t => t.ID_Tipo_Identificacion == Tipo.ID_Tipo_Identificacion);

                if (eliminado == null)
                {
                    ViewData["ModalType"] = "error";
                    ViewData["ModalTitle"] = "Error";
                    ViewData["ModalMessage"] = "No se encontró el registro a eliminar.";
                    return Page();
                }

                _service.Eliminar(eliminado.ID_Tipo_Identificacion);

                //Registrar en bitácora 
                await _bitacoraService.Registrar(1, 3, eliminado, "DELETE");

                ViewData["ModalType"] = "success";
                ViewData["ModalTitle"] = "Eliminación exitosa";
                ViewData["ModalMessage"] = "El tipo de identificación fue eliminado correctamente.";
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
