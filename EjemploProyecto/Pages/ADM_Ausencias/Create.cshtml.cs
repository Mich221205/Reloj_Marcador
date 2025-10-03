using EjemploCoreWeb.Entities;
using EjemploCoreWeb.Services;
using EjemploCoreWeb.Services.Abstract;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace EjemploProyecto.Pages.ADM_Ausencias
{
    public class CreateModel : PageModel
    {

        private readonly IMotivos_Ausencia _motivoService;

        public CreateModel(IMotivos_Ausencia motivoService)
        {
            _motivoService = motivoService;
            Motivos = new Motivos_Ausencia();
        }

        [BindProperty]
        public Motivos_Ausencia Motivos { get; set; }

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
