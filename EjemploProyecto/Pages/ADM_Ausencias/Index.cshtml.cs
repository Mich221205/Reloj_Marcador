using EjemploCoreWeb.Entities;
using EjemploCoreWeb.Services;
using EjemploCoreWeb.Services.Abstract;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace EjemploProyecto.Pages.ADM_Ausencias
{
    public class IndexModel : PageModel
    {

        private readonly IMotivos_Ausencia _motivos_Inconsistencia;

        private readonly IBitacoraService _bitacoraService;

        public IndexModel(IMotivos_Ausencia MotivosService, IBitacoraService bitacoraService)
        {
            _motivos_Inconsistencia = MotivosService;
            Motivos = new List<Motivos_Ausencia>();
            _bitacoraService = bitacoraService;
        }

        public IEnumerable<Motivos_Ausencia> Motivos { get; set; }

        public async Task OnGetAsync()
        {
            Motivos = await _motivos_Inconsistencia.CargarMotivosAsync();

            await _bitacoraService.Registrar(1, 4, "El usuario consulta motivos ausencias", "CONSULTA");
        }

    }
}
