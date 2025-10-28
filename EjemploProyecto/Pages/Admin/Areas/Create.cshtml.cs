using EjemploCoreWeb.Entities;
using EjemploCoreWeb.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace EjemploProyecto.Pages.Admin.Areas
{
    public class CreateModel : PageModel
    {
        private readonly IAreaService _areas;

        public CreateModel(IAreaService areas)
        {
            _areas = areas;
        }

        public IEnumerable<SelectListItem> Jefes { get; private set; } = Enumerable.Empty<SelectListItem>();

        public class VM
        {
            [Required(ErrorMessage = "El nombre del área es obligatorio.")]
            [StringLength(40, ErrorMessage = "El nombre del área no debe ser mayor a 40 caracteres.")]
            [RegularExpression(@"^[A-Za-zÁÉÍÓÚáéíóúñÑ ]+$",
                ErrorMessage = "El nombre del área solo puede contener letras y espacios.")]
            public string Nombre_Area { get; set; } = "";

            [Required(ErrorMessage = "El código del área es obligatorio.")]
            [StringLength(15, ErrorMessage = "El código del área no debe ser mayor a 15 caracteres.")]
            [RegularExpression(@"^[A-Za-z0-9_-]{1,15}$",
                ErrorMessage = "El código del área puede incluir letras, números, guion (-) y guion bajo (_).")]
            public string Codigo_Area { get; set; } = "";

            [Display(Name = "Jefe del área")]
            [Required(ErrorMessage = "Debe seleccionar un jefe de área.")]
            [Range(1, int.MaxValue, ErrorMessage = "Debe seleccionar un jefe de área.")]
            public int? Jefe_Area { get; set; }
        }


        [BindProperty] public VM Input { get; set; } = new();

        public async Task OnGetAsync()
        {
            await CargarJefesAsync();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                await CargarJefesAsync();
                return Page();
            }

            var a = new Area
            {
                Nombre_Area = Input.Nombre_Area,
                Codigo_Area = Input.Codigo_Area,
                Jefe_Area = Input.Jefe_Area!.Value
            };


            await _areas.CrearAsync(a);
            TempData["Ok"] = "Área creada correctamente.";
            return RedirectToPage("Index");
        }

        private async Task CargarJefesAsync()
        {
            // Funcionario activo: Nombre Apellido1 Apellido2 (Identificación)
            var funcionarios = await _areas.ListarFuncionariosAsync();
            Jefes = funcionarios
                .Select(f => new SelectListItem
                {
                    Value = f.Id.ToString(),
                    Text = f.Nombre
                })
                .OrderBy(i => i.Text)
                .ToList();
        }
    }
}
