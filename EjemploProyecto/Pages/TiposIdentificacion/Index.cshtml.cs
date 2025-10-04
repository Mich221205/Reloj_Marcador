using EjemploCoreWeb.Entities;
using EjemploCoreWeb.Services.Abstract;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace EjemploProyecto.Pages.ADM_Identificacion
{
    public class IndexModel : PageModel
    {
        private readonly ITipoIdentificacionService _service;

        public IndexModel(ITipoIdentificacionService service)
        {
            _service = service;
        }

        public IEnumerable<TipoIdentificacion> Tipos { get; set; }

        public void OnGet()
        {
            Tipos = _service.ObtenerTodos();
        }
    }
}

