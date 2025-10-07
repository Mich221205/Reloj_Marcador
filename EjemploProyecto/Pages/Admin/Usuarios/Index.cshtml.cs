using EjemploCoreWeb.Entities;
using EjemploCoreWeb.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace EjemploProyecto.Pages.Admin.Usuarios;

public class IndexModel : PageModel
{
    private readonly IUsuarioService _svc;
    public IndexModel(IUsuarioService svc) => _svc = svc;

    [BindProperty(SupportsGet = true)] public string? Q { get; set; }
    [BindProperty(SupportsGet = true)] public int Page { get; set; } = 1;
    public int TotalPages { get; set; }
    public IEnumerable<Usuario> Lista { get; set; } = Enumerable.Empty<Usuario>();
    public IEnumerable<Rol> Roles { get; set; } = Enumerable.Empty<Rol>();

    private const int PageSize = 10;

    public async Task OnGetAsync()
    {
        var (items, total) = await _svc.ListarPaginadoAsync(Q, Page < 1 ? 1 : Page, PageSize);
        Lista = items;
        TotalPages = (int)Math.Ceiling(total / (double)PageSize);
        Roles = await _svc.ListarRolesAsync();
    }

    public async Task<IActionResult> OnPostDeleteAsync(int id)
    {
        var ok = await _svc.EliminarAsync(id);
        TempData[ok ? "Ok" : "Error"] = ok
            ? "Usuario eliminado."
            : "No se puede eliminar un registro con datos relacionados.";
        return RedirectToPage(new { Q, Page });
    }
}
