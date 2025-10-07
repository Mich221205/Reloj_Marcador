using EjemploCoreWeb.Entities;
using EjemploCoreWeb.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace EjemploProyecto.Pages.Admin.Areas;

public class CreateModel : PageModel
{
    private readonly IAreaService _areas;
    private readonly IUsuarioService _usuarios;

    public CreateModel(IAreaService areas, IUsuarioService usuarios)
    {
        _areas = areas;
        _usuarios = usuarios;
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

        [Required(ErrorMessage = "Seleccione el jefe del área.")]
        public int Jefe_Area { get; set; }
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
            Jefe_Area = Input.Jefe_Area
        };

        await _areas.CrearAsync(a);
        return RedirectToPage("Index");
    }

    private async Task CargarJefesAsync()
    {
        var usuarios = await _usuarios.ListarAsync(null);
        Jefes = usuarios
            .Select(u => new SelectListItem
            {
                Value = u.Id_Usuario.ToString(),
                Text = (u.Nombre ?? $"{u.Nombre} {u.Apellido_1} {u.Apellido_2}").Trim()
            })
            .OrderBy(i => i.Text)
            .ToList();
    }
}
