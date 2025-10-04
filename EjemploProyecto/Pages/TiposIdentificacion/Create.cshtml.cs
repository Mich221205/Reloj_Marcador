using EjemploCoreWeb.Entities;
using EjemploCoreWeb.Services.Abstract;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace EjemploProyecto.Pages.ADM_Identificacion
{
    public class CreateModel : PageModel
    {
        private readonly ITipoIdentificacionService _service;

        public CreateModel(ITipoIdentificacionService service)
        {
            _service = service;
        }

        [BindProperty]
        public TipoIdentificacion Tipo { get; set; }

        public void OnGet() { }

        public IActionResult OnPost()
        {
            if (!ModelState.IsValid)
                return Page();

            _service.Crear(Tipo);
            return RedirectToPage("Index");
        }
    }
}
