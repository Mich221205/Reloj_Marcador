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

    public IEnumerable<SelectListItem> Jefes { get; private set; } = Enumerable.Empty<SelectListItem>();

    public class VM
    {
        public int ID_Area { get; set; }

        [Required, StringLength(40), RegularExpression("^[A-Za-zÁÉÍÓÚáéíóúñÑ ]+$",
            ErrorMessage = "Solo letras y espacios, máximo 40.")]
        public string Nombre_Area { get; set; } = "";

        [Required] public int Jefe_Area { get; set; }
    }

    [BindProperty] public VM Input { get; set; } = new();

    public async Task<IActionResult> OnGetAsync(int id)
    {
        var a = await _areas.ObtenerAsync(id);
        if (a is null) return RedirectToPage("Index");

        Input = new VM
        {
            ID_Area = a.ID_Area,
            Nombre_Area = a.Nombre_Area,
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
            Jefe_Area = Input.Jefe_Area
        };

        var ok = await _areas.ActualizarAsync(a);
        if (!ok) TempData["Error"] = "No fue posible actualizar el área.";
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
