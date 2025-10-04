using EjemploCoreWeb.Entities;
using EjemploCoreWeb.Services.Abstract;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace EjemploProyecto.Pages.Personas
{
    public class EditModel : PageModel
    {
        private readonly IPersonaService _personaService;

        public EditModel(IPersonaService personaService)
        {
            _personaService = personaService;
            Persona = new Persona();
        }

        [BindProperty]
        public Persona Persona { get; set; }

        public async Task<IActionResult> OnGetAsync(string id)
        {
            Persona = await _personaService.GetByIdAsync(id);

            if (Persona == null)
            {
                return NotFound();
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            try
            {
                await _personaService.UpdateAsync(Persona);

                ViewData["ModalType"] = "success";
                ViewData["ModalTitle"] = "Éxito";
                ViewData["ModalMessage"] = "La persona se actualizó correctamente.";
            }
            catch (Exception ex)
            {
                ViewData["ModalType"] = "error";
                ViewData["ModalTitle"] = "Error";
                ViewData["ModalMessage"] = $"No se pudo actualizar la persona: {ex.Message}";
            }

            return Page();
        }
    }
}
