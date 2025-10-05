using EjemploCoreWeb.Entities;
using EjemploCoreWeb.Services;
using EjemploCoreWeb.Services.Abstract;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace EjemploProyecto.Pages.ADM_Ausencias
{
    public class DeleteModel : PageModel
    {

        private readonly IMotivos_Ausencia _motivoService;
        private readonly IBitacoraService _bitacoraService;

        public DeleteModel(IMotivos_Ausencia motivoService, IBitacoraService bitacoraService)
        {
            _motivoService = motivoService;
            Motivo = new Motivos_Ausencia();
            _bitacoraService = bitacoraService;
        }

        [BindProperty]
        public Motivos_Ausencia? Motivo { get; set; }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            Motivo = await _motivoService.Cargar_Motivo_X_IDAsync(id);

            if (Motivo == null)
            {
                return NotFound();
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int id)
        {
            if (Motivo != null)
            {

                var eliminado = await _motivoService.Cargar_Motivo_X_IDAsync(Motivo.ID_Motivo);

                await _motivoService.DeleteAsync(Motivo.ID_Motivo);

                await _bitacoraService.Registrar(1, 3, eliminado, "DELETE");
            }
            return RedirectToPage("Index");
        }

    }
}
