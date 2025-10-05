using EjemploCoreWeb.Entities;
using EjemploCoreWeb.Services.Abstract;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EjemploProyecto.Pages.ADM_Horarios
{
    public class Create_HorarioModel : PageModel
    {
        private readonly IHorarios _horarioService;

        public Create_HorarioModel(IHorarios horarioService)
        {
            _horarioService = horarioService;
            Horario = new Horarios();
            AreasDisponibles = new List<Horarios>();
        }

        [BindProperty]
        public Horarios Horario { get; set; }

        public List<Horarios> AreasDisponibles { get; set; }

        // Cargar todas las áreas (o las del usuario si se quisiera)
        public async Task OnGetAsync(string id)
        {
            // Cargar todas las áreas del sistema
            AreasDisponibles = (await _horarioService.Obtener_Todas_AreasAsync()).ToList();

            // Si viene la identificación del usuario, se precarga
            if (!string.IsNullOrWhiteSpace(id))
            {
                Horario.Identificacion = id;
            }
        }

        // Crear nuevo horario
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
                return Page();

            await _horarioService.InsertHorarioAsync(Horario);
            return RedirectToPage("Detalle_Horarios");
        }
    }
}
