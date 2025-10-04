using EjemploCoreWeb.Entities;
using EjemploCoreWeb.Services.Abstract;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace EjemploProyecto.Pages.ADM_Identificacion
{
    public class EditModel : PageModel
    {
        private readonly ITipoIdentificacionService _service;

        public EditModel(ITipoIdentificacionService service)
        {
            _service = service;
        }

        [BindProperty]
        public TipoIdentificacion Tipo { get; set; }

        public void OnGet(int id)
        {
            Tipo = _service.ObtenerTodos().FirstOrDefault(t => t.ID_Tipo_Identificacion == id);
        }

        public IActionResult OnPost()
        {
            if (!ModelState.IsValid)
                return Page();

            _service.Actualizar(Tipo);
            return RedirectToPage("Index");
        }
    }
}
