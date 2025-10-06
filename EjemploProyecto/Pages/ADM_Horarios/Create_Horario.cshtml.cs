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

        // Crear nuevo horario
        // Crear nuevo horario
        public async Task<IActionResult> OnPostAsync()
        {
            Console.WriteLine($"🔹 OnPostAsync iniciado");

            if (!ModelState.IsValid)
            {
                Console.WriteLine($"❌ ModelState no es válido");
                foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
                {
                    Console.WriteLine($"❌ Error de validación: {error.ErrorMessage}");
                }

                // Recargar áreas si hay error
                if (!string.IsNullOrWhiteSpace(Horario.Identificacion))
                {
                    AreasDisponibles = (await _horarioService.Obtener_Areas_UsuarioAsync(Horario.Identificacion)).ToList();
                }
                return Page();
            }

            try
            {
                Console.WriteLine($"🔹 ModelState válido, procediendo a insertar");
                Console.WriteLine($"🔹 Datos del horario:");
                Console.WriteLine($"   - Identificación: {Horario.Identificacion}");
                Console.WriteLine($"   - ID_Area: {Horario.ID_Area}");
                Console.WriteLine($"   - Codigo_Area: {Horario.Codigo_Area}");

                // Insertar horario nuevo
                var result = await _horarioService.InsertHorarioAsync(Horario);

                Console.WriteLine($"✅ Horario insertado. Resultado: {result}");

                TempData["SuccessMessage"] = "✅ Horario creado correctamente!";
                return RedirectToPage("Detalle_Horarios", new { id = Horario.Identificacion });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ EXCEPCIÓN en OnPostAsync: {ex.Message}");
                Console.WriteLine($"❌ StackTrace: {ex.StackTrace}");

                // Recargar áreas en caso de error
                if (!string.IsNullOrWhiteSpace(Horario.Identificacion))
                {
                    AreasDisponibles = (await _horarioService.Obtener_Areas_UsuarioAsync(Horario.Identificacion)).ToList();
                }

                TempData["ErrorMessage"] = $"❌ Error al crear horario: {ex.Message}";
                return Page();
            }
        }

    }
    
}
