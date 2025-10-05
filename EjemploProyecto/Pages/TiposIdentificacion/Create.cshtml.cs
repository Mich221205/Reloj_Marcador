using EjemploCoreWeb.Entities;
using EjemploCoreWeb.Services;
using EjemploCoreWeb.Services.Abstract;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace EjemploProyecto.Pages.ADM_Identificacion
{
    public class CreateModel : PageModel
    {
        private readonly ITipoIdentificacionService _service;

        private readonly IBitacoraService _bitacoraService;



        public CreateModel(ITipoIdentificacionService service, IBitacoraService bitacoraService)
        {
            _service = service;
            _bitacoraService = bitacoraService;
        }

        [BindProperty]
        public TipoIdentificacion Tipo { get; set; }

        public void OnGet() { }

        public async Task<IActionResult> OnPostAsync()
        {
            //Si el modelo no es válido (más de 40 caracteres, símbolos, etc.)
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
               
                _service.Crear(Tipo);

                ViewData["ModalType"] = "success";
                ViewData["ModalTitle"] = "Éxito";
                ViewData["ModalMessage"] = "Tipo de identificación creado correctamente.";
                ViewData["RedirectPage"] = "Index"; // redirige 
            }
            catch (InvalidOperationException ex)
            {
                ViewData["ModalType"] = "error";
                ViewData["ModalTitle"] = "Error";
                ViewData["ModalMessage"] = ex.Message;
            }

            return Page(); // recarga y muestra el modal
        }
    }
}
