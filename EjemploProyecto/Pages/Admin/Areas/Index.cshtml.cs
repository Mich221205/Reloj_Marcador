using EjemploCoreWeb.Entities;
using EjemploCoreWeb.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace EjemploProyecto.Pages.Admin.Areas;

public class IndexModel : PageModel
{
    private readonly IAreaService _areas;

    public IndexModel(IAreaService areas) => _areas = areas;

    [BindProperty(SupportsGet = true)]
    public string? Q { get; set; }

    public IEnumerable<Area> Lista { get; private set; } = Enumerable.Empty<Area>();

    public async Task OnGetAsync()
    {
        Lista = await _areas.ListarAsync(Q);
    }

    public async Task<IActionResult> OnPostDeleteAsync(int id)
    {
        var ok = await _areas.EliminarAsync(id);
        if (ok)
            TempData["Ok"] = "Área eliminada correctamente.";
        else
            TempData["Error"] = "No se puede eliminar un registro con datos relacionados.";
        // preserva el filtro actual
        return RedirectToPage(new { Q });
    }
}
