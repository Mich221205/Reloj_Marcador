using EjemploCoreWeb.Entities;
using EjemploCoreWeb.Services.Abstract;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace EjemploProyecto.Pages.Personas
{
    public class CreateModel : PageModel
    {
        private readonly IPersonaService _personaService;

        public CreateModel(IPersonaService personaService)
        {
            _personaService = personaService;
            Persona = new Persona();
        }

        [BindProperty]
        public Persona Persona { get; set; }

        public IActionResult OnGet()
        {
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            await _personaService.InsertAsync(Persona);

            return RedirectToPage("Index");
        }
    }
}
