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
        private readonly IBitacoraService _bitacoraService;

        public CreateModel(IMotivos_Ausencia motivoService, IBitacoraService bitacoraService)
        {
            _motivoService = motivoService;
            Motivos = new Motivos_Ausencia();
            _bitacoraService = bitacoraService;
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

                await _bitacoraService.Registrar(1, 1, Motivos, "INSERT");
                
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
