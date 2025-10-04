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
                return RedirectToPage("Index");
            }
            catch (InvalidOperationException ex)
            {
                ErrorMessage = ex.Message;
                return Page();
            }
        }
    }
}
