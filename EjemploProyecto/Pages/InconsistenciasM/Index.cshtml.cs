using EjemploCoreWeb.Entities;
using EjemploCoreWeb.Services.Abstract;
using Microsoft.AspNetCore.Mvc;
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

            // Si existen mensajes pasados desde Create/Edit/Delete
            if (TempData.ContainsKey("SuccessMessage"))
            {
                TempData["ModalType"] = "success";
                TempData["ModalTitle"] = "Éxito";
                TempData["ModalMessage"] = TempData["SuccessMessage"];
            }
            else if (TempData.ContainsKey("ErrorMessage"))
            {
                TempData["ModalType"] = "error";
                TempData["ModalTitle"] = "Error";
                TempData["ModalMessage"] = TempData["ErrorMessage"];
            }
        }
    }
}
