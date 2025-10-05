using EjemploCoreWeb.Entities;
using EjemploCoreWeb.Services;
using EjemploCoreWeb.Services.Abstract;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace EjemploProyecto.Pages.ADM_Ausencias
{
    public class EditModel : PageModel
    {
        private readonly IMotivos_Ausencia _motivoService;
        private readonly IBitacoraService _bitacoraService;

        public EditModel(IMotivos_Ausencia motivoService, IBitacoraService bitacoraService)
        {
            _motivoService = motivoService;
            Motivo = new Motivos_Ausencia();
            _bitacoraService = bitacoraService;
        }

        [BindProperty]
        public Motivos_Ausencia Motivo { get; set; }

        public async Task<IActionResult> OnGetAsync(int id)
        {

            Motivo = await _motivoService.Cargar_Motivo_X_IDAsync(id);

            if (Motivo == null)
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

            var anterior = await _motivoService.Cargar_Motivo_X_IDAsync(Motivo.ID_Motivo);

            await _motivoService.UpdateAsync(Motivo);

            // Seteamos mensaje para el modal
            ViewData["SuccessMessage"] = "El motivo se actualizó correctamente.";

            await _bitacoraService.Registrar(1, 2, new { Antes = anterior, Despues = Motivo }, "UPDATE");

            // Nos quedamos en la misma página para mostrar el modal
            return Page();
        }


    }
}
