using EjemploCoreWeb.Entities;
using EjemploCoreWeb.Services;
using EjemploCoreWeb.Services.Abstract;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace EjemploProyecto.Pages.ADM_Ausencias
{
    public class Index_InconsistenciasModel : PageModel
    {

        private readonly IMotivos_Ausencia _motivos_Inconsistencia;

        public Index_InconsistenciasModel(IMotivos_Ausencia MotivosService)
        {
            _motivos_Inconsistencia = MotivosService;
            Motivos = new List<Motivos_Ausencia>();
        
        }

        public IEnumerable<Motivos_Ausencia> Motivos { get; set; }

        public async Task OnGetAsync()
        {
            Motivos = await _motivos_Inconsistencia.CargarMotivosAsync();
        }

    }
}
