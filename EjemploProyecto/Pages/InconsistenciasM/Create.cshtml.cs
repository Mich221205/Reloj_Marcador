using EjemploCoreWeb.Entities;
using EjemploCoreWeb.Services.Abstract;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Threading.Tasks;

namespace EjemploProyecto.Pages.Inconsistencias
{
    public class CreateModel : PageModel
    {
        private readonly IInconsistenciaService _service;
        private readonly IBitacoraService _bitacoraService;

        [BindProperty]
        public Inconsistencia Inconsistencia { get; set; }

        public CreateModel(IInconsistenciaService service, IBitacoraService bitacoraService)
        {
            _service = service;
            _bitacoraService = bitacoraService;
        }

        public void OnGet() { }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
                return Page();

            await _service.Crear(Inconsistencia);

            await _bitacoraService.Registrar(1, 1, Inconsistencia, "INSERT");

            TempData["SuccessMessage"] = "Inconsistencia creada correctamente.";
            return RedirectToPage("Index");
        }
    }
}

