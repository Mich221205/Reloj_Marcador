using EjemploCoreWeb.Entities;
using EjemploCoreWeb.Services.Abstract;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Linq;
using System.Threading.Tasks;

namespace EjemploProyecto.Pages.ADM_Identificacion
{
    public class EditModel : PageModel
    {
        private readonly ITipoIdentificacionService _service;
        private readonly IBitacoraService _bitacoraService;

        public EditModel(ITipoIdentificacionService service, IBitacoraService bitacoraService)
        {
            _service = service;
            _bitacoraService = bitacoraService;
        }

        [BindProperty]
        public TipoIdentificacion Tipo { get; set; }

        public void OnGet(int id)
        {
            Tipo = _service.ObtenerTodos().FirstOrDefault(t => t.ID_Tipo_Identificacion == id);
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                var errores = string.Join("<br>",
                    ModelState.Values.SelectMany(v => v.Errors)
                                     .Select(e => e.ErrorMessage));

                ViewData["ModalType"] = "error";
                ViewData["ModalTitle"] = "Error de validación";
                ViewData["ModalMessage"] = errores;

                return Page();
            }

            try
            {
             
                var anterior = _service.ObtenerTodos()
                    .FirstOrDefault(t => t.ID_Tipo_Identificacion == Tipo.ID_Tipo_Identificacion);

             
                _service.Actualizar(Tipo);

           
                var cambios = new
                {
                    Antes = anterior,
                    Despues = Tipo
                };

               
                int idUsuario = 1; // Temporal, luego se obtiene del login
                await _bitacoraService.Registrar(
                    idUsuario,
                    idAccion: 2,
                    detalle: cambios,
                    nombreAccion: "Actualización de Tipo de Identificación"
                );

            
                ViewData["ModalType"] = "success";
                ViewData["ModalTitle"] = "Actualización exitosa";
                ViewData["ModalMessage"] = "El tipo de identificación fue actualizado correctamente.";
                ViewData["RedirectPage"] = "Index";
            }
            catch (InvalidOperationException ex)
            {
                
                int idUsuario = 1;
                await _bitacoraService.Registrar(
                    idUsuario,
                    idAccion: 99, // 99 = error técnico
                    detalle: new { Error = ex.Message },
                    nombreAccion: "Error al actualizar Tipo de Identificación"
                );

                ViewData["ModalType"] = "error";
                ViewData["ModalTitle"] = "Error";
                ViewData["ModalMessage"] = ex.Message;
            }

            return Page();
        }
    }
}
