using EjemploCoreWeb.Entities;
using EjemploCoreWeb.Services.Abstract;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Threading.Tasks;

namespace EjemploProyecto.Pages.ADM_Identificacion
{
    public class IndexModel : PageModel
    {
        private readonly ITipoIdentificacionService _service;
        private readonly IBitacoraService _bitacoraService;

        public IndexModel(ITipoIdentificacionService service, IBitacoraService bitacoraService)
        {
            _service = service;
            _bitacoraService = bitacoraService;
        }

        public IEnumerable<TipoIdentificacion> Tipos { get; set; }

        public async Task OnGetAsync()
        {
           
            Tipos = _service.ObtenerTodos();

          
            int idUsuario = 1; //Temporal — luego se reemplaza con el usuario logueado
            await _bitacoraService.Registrar(
                idUsuario,
                idAccion: 4, 
                detalle: null,
                nombreAccion: "El usuario consulta Tipos de Identificación"
            );
        }
    }
}
