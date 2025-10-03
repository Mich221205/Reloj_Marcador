using EjemploCoreWeb.Entities;
using EjemploCoreWeb.Services;
using EjemploCoreWeb.Services.Abstract;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace EjemploProyecto.Pages.ADM_Inconsistencias
{
    public class Index_InconsistenciasModel : PageModel
    {

        private readonly IMotivos_Inconsistencia _motivos_Inconsistencia;

        public Index_InconsistenciasModel(IMotivos_Inconsistencia MotivosService)
        {
            _motivos_Inconsistencia = MotivosService;
            Motivos = new List<Motivos_Inconsistencias>();
        
        }

        public IEnumerable<Motivos_Inconsistencias> Motivos { get; set; }

        public async Task OnGetAsync()
        {
            Motivos = await _motivos_Inconsistencia.CargarMotivosAsync();
        }

    }
}
