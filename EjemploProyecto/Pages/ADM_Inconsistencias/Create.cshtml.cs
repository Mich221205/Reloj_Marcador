using EjemploCoreWeb.Entities;
using EjemploCoreWeb.Services;
using EjemploCoreWeb.Services.Abstract;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace EjemploProyecto.Pages.ADM_Inconsistencias
{
    public class CreateModel : PageModel
    {

        private readonly IMotivos_Inconsistencia _motivoService;

        public CreateModel(IMotivos_Inconsistencia motivoService)
        {
            _motivoService = motivoService;
            Motivos = new Motivos_Inconsistencias();
        }

        [BindProperty]
        public Motivos_Inconsistencias Motivos { get; set; }

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

            try
            {
                await _motivoService.InsertAsync(Motivos);
                ViewData["ModalType"] = "success";
                ViewData["ModalTitle"] = "Éxito";
                ViewData["ModalMessage"] = "Motivo creado exitosamente.";
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
