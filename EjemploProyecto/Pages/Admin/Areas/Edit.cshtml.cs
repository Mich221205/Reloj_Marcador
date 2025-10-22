using EjemploCoreWeb.Entities;
using EjemploCoreWeb.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace EjemploProyecto.Pages.Admin.Areas;

public class EditModel : PageModel
{
    private readonly IAreaService _areas;
    private readonly IUsuarioService _usuarios;

    public EditModel(IAreaService areas, IUsuarioService usuarios)
    {
        _areas = areas;
        _usuarios = usuarios;
    }

    [BindProperty(SupportsGet = true)]
    public string? Q { get; set; }

    public IEnumerable<SelectListItem> Jefes { get; private set; } = Enumerable.Empty<SelectListItem>();

    public class VM
    {
        [Required]
        public int ID_Area { get; set; }

        [Required(ErrorMessage = "El nombre del área es obligatorio.")]
        [StringLength(40, ErrorMessage = "El nombre del área no debe ser mayor a 40 caracteres.")]
        [RegularExpression("^[A-Za-zÁÉÍÓÚáéíóúñÑ ]+$",
            ErrorMessage = "El nombre del área solo puede contener letras y espacios.")]
        public string Nombre_Area { get; set; } = "";

        [Required(ErrorMessage = "El código del área es obligatorio.")]
        [StringLength(15, ErrorMessage = "El código del área no debe ser mayor a 15 caracteres.")]
        [RegularExpression(@"^[A-Za-z0-9_-]{1,15}$",
            ErrorMessage = "El código puede incluir letras, números, guion (-) y guion bajo (_).")]
        public string Codigo_Area { get; set; } = "";

        [Required(ErrorMessage = "Seleccione el jefe del área.")]
        public int Jefe_Area { get; set; }
    }

    [BindProperty] public VM Input { get; set; } = new();

    public async Task<IActionResult> OnGetAsync(int id)
    {
        if (id <= 0) return RedirectToPage("Index", new { Q });

        var a = await _areas.ObtenerAsync(id);
        if (a is null) return RedirectToPage("Index", new { Q });

        Input = new VM
        {
            ID_Area = a.ID_Area,
            Nombre_Area = a.Nombre_Area,
            Codigo_Area = a.Codigo_Area,
            Jefe_Area = a.Jefe_Area
        };

        await CargarJefesAsync();
        return Page();
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
            ID_Area = Input.ID_Area,
            Nombre_Area = Input.Nombre_Area,
            Codigo_Area = Input.Codigo_Area,
            Jefe_Area = Input.Jefe_Area
        };

        var ok = await _areas.ActualizarAsync(a);
        TempData[ok ? "Ok" : "Error"] = ok
            ? "Área actualizada correctamente."
            : "No fue posible actualizar el área.";

        return RedirectToPage("Index", new { Q });
    }

    private async Task CargarJefesAsync()
    {
        var usuarios = await _usuarios.ListarAsync(null) ?? Enumerable.Empty<Usuario>();
        Jefes = usuarios
            .Select(u => new SelectListItem
            {
                Value = u.Id_Usuario.ToString(),
                Text = $"{u.Nombre} {u.Apellido_1} {u.Apellido_2}".Trim()
            })
            .OrderBy(i => i.Text)
            .ToList();
    }
}
