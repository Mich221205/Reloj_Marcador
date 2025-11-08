using EjemploCoreWeb.Services;
using EjemploCoreWeb.Services.Interfaces;
using EjemploCoreWeb.Services.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace EjemploProyecto.Pages.Reportes.Inconsistencias
{
    public class Proceso_Mar_IncModel : PageModel
    {


        private readonly Proceso_Generar_Inconsistencias_MarcasService _service;
        private readonly IAreaService _areaService;


        public Proceso_Mar_IncModel(Proceso_Generar_Inconsistencias_MarcasService service, IAreaService areaService)
        {
            _service = service;
            _areaService = areaService;
        }


        [BindProperty] public DateTime FechaInicio { get; set; } = DateTime.Now;
        [BindProperty] public DateTime FechaFin { get; set; } = DateTime.Now;
        [BindProperty] public int? AreaId { get; set; }
        [BindProperty] public int? UsuarioId { get; set; }

        public List<SelectListItem> Areas { get; set; } = new();
        public List<SelectListItem> Funcionarios { get; set; } = new();
        public string? Mensaje { get; set; }

        public async Task OnGetAsync()
        {
            //var areas = await _areaService.ListarAsync();
            var usuarios = await _areaService.ListarFuncionariosAsync();

            var areas = await _areaService.ListarAsync(null);
            Areas = areas.Select(a => new SelectListItem
            {
                Value = a.ID_Area.ToString(),
                Text = a.Nombre_Area
            }).ToList();

            Funcionarios = usuarios.Select(u => new SelectListItem
            {
                Value = u.Id.ToString(),
                Text = $"{u.Nombre}"
            }).ToList();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            try
            {
                // Validación de fechas
                if (FechaInicio > DateTime.Now.Date || FechaFin > DateTime.Now.Date)
                {
                    ViewData["ModalType"] = "error";
                    ViewData["ModalTitle"] = "Fechas no válidas";
                    ViewData["ModalMessage"] = "No se puede ejecutar el proceso con fechas futuras.";
                    await OnGetAsync();
                    return Page();
                }

                if (FechaInicio > FechaFin)
                {
                    ViewData["ModalType"] = "error";
                    ViewData["ModalTitle"] = "Rango incorrecto";
                    ViewData["ModalMessage"] = "La fecha de inicio no puede ser posterior a la fecha de fin.";
                    await OnGetAsync();
                    return Page();
                }

                await _service.EjecutarAsync(FechaInicio, FechaFin, AreaId, UsuarioId);

                ViewData["ModalType"] = "success";
                ViewData["ModalTitle"] = "Proceso completado";
                ViewData["ModalMessage"] = $"Proceso ejecutado correctamente del {FechaInicio:dd/MM/yyyy} al {FechaFin:dd/MM/yyyy}.";
            }
            catch (Exception ex)
            {
                ViewData["ModalType"] = "error";
                ViewData["ModalTitle"] = "Error al ejecutar";
                ViewData["ModalMessage"] = ex.Message;
            }

            await OnGetAsync();
            return Page();
        }

    }
}
