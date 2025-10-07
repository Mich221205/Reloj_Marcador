using EjemploCoreWeb.Entities;
using EjemploCoreWeb.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace EjemploProyecto.Pages.Admin.Usuarios;

public class AreasModel : PageModel
{
    private readonly IUsuarioService _usuarios;
    private readonly IUsuarioAreaService _ua;

    public AreasModel(IUsuarioService usuarios, IUsuarioAreaService ua)
    {
        _usuarios = usuarios;
        _ua = ua;
    }

    [BindProperty(SupportsGet = true)] public int Id { get; set; } // usuario
    public Usuario? Usuario { get; set; }
    public IEnumerable<UsuarioArea> Asociadas { get; set; } = Enumerable.Empty<UsuarioArea>();
    public IEnumerable<Area> Disponibles { get; set; } = Enumerable.Empty<Area>();

    public async Task<IActionResult> OnGetAsync()
    {
        Usuario = await _usuarios.ObtenerAsync(Id);
        if (Usuario is null) return RedirectToPage("Index");
        Asociadas = await _ua.ListarPorUsuarioAsync(Id);
        Disponibles = await _ua.ListarNoAsociadasAsync(Id);
        return Page();
    }

    public async Task<IActionResult> OnPostAddAsync(int idArea)
    {
        await _ua.AsociarAsync(Id, idArea);
        return RedirectToPage(new { id = Id });
    }

    public async Task<IActionResult> OnPostRemoveAsync(int idArea)
    {
        var ok = await _ua.DesasociarAsync(Id, idArea);
        if (!ok) TempData["Error"] = "No se puede eliminar un registro con datos relacionados.";
        return RedirectToPage(new { id = Id });
    }
}
