using EjemploCoreWeb.Entities;
using EjemploCoreWeb.Services.Abstract;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace EjemploProyecto.Pages.Personas
{
    public class IndexModel : PageModel
    {
        private readonly IPersonaService _personaService;

        public IndexModel(IPersonaService personaService)
        {
            _personaService = personaService;
            Personas = new List<Persona>();
        }

        public IEnumerable<Persona> Personas { get; set; }

        public async Task OnGetAsync()
        {
            Personas = await _personaService.GetAllAsync();
        }
    }
}
