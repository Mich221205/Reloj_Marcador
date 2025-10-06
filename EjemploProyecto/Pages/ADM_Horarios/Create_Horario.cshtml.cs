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

        // 🔹 Cargar las áreas del usuario según su identificación
        public async Task OnGetAsync(string id)
        {
            if (!string.IsNullOrWhiteSpace(id))
            {
                // Aquí sí: el método devuelve una lista
                AreasDisponibles = (await _horarioService.Obtener_Areas_UsuarioAsync(id)).ToList();
                Horario.Identificacion = id; // precargar la identificación en el form
            }
        }

        // 🔹 Crear nuevo horario
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
                return Page();

            try
            {
                // Buscar ID_Usuario real en base a la Identificación
                var horarios = await _horarioService.Obtener_Horario_UsuarioAsync(Horario.Identificacion);
                var usuario = horarios.FirstOrDefault(); // si ya tienes el usuario cargado
                if (usuario == null)
                    throw new InvalidOperationException("No se encontró el usuario asociado.");

                // Insertar horario nuevo
                await _horarioService.InsertHorarioAsync(Horario);

                TempData["SuccessMessage"] = "Horario creado correctamente.";
                return RedirectToPage("Detalle_Horarios", new { id = Horario.Identificacion });
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error al crear horario: {ex.Message}";
                return Page();
            }
        }
    }
}
