using EjemploCoreWeb.Entities;
using EjemploCoreWeb.Services.Abstract;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EjemploProyecto.Pages.Inconsistencias
{
    public class IndexModel : PageModel
    {
        private readonly IInconsistenciaService _service;
        private readonly IBitacoraService _bitacoraService;

        public IEnumerable<Inconsistencia> Inconsistencias { get; set; }
        public int TotalRegistros { get; set; }
        public int PaginaActual { get; set; } = 1;
        public int TamañoPagina { get; set; } = 10;

        public IndexModel(IInconsistenciaService service, IBitacoraService bitacoraService)
        {
            _service = service;
            _bitacoraService = bitacoraService;
        }

        public async Task OnGetAsync(int pagina = 1)
        {
            PaginaActual = pagina;
            Inconsistencias = await _service.Listar(PaginaActual, TamañoPagina);
            TotalRegistros = await _service.Contar();

            await _bitacoraService.Registrar(1, 4, "El usuario consultó Inconsistencias", "CONSULTA");

        }
    }
}

