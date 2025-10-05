using EjemploCoreWeb.Entities;
using EjemploCoreWeb.Services.Abstract;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace EjemploProyecto.Pages.ADM_Identificacion
{
    public class DeleteModel : PageModel
    {
        private readonly ITipoIdentificacionService _service;

        public DeleteModel(ITipoIdentificacionService service)
        {
            _service = service;
        }

        [BindProperty]
        public TipoIdentificacion Tipo { get; set; }

        [TempData]
        public string ErrorMessage { get; set; }

        public void OnGet(int id)
        {
            Tipo = _service.ObtenerTodos().FirstOrDefault(t => t.ID_Tipo_Identificacion == id);
        }

        public IActionResult OnPost()
        {
            try
            {
                _service.Eliminar(Tipo.ID_Tipo_Identificacion);

                ViewData["ModalType"] = "success";
                ViewData["ModalTitle"] = "Eliminación exitosa";
                ViewData["ModalMessage"] = "El tipo de identificación fue eliminado correctamente.";
                ViewData["RedirectPage"] = "Index"; // redirige
            }
            catch (InvalidOperationException ex)
            {
                ViewData["ModalType"] = "error";
                ViewData["ModalTitle"] = "Error";
                ViewData["ModalMessage"] = ex.Message;
            }

            // Vuelve a la página actual 
            return Page();
        }
    }
}

