using EjemploCoreWeb.Entities;
using EjemploCoreWeb.Services.Abstract;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace EjemploProyecto.Pages.ADM_Identificacion
{
    public class EditModel : PageModel
    {
        private readonly ITipoIdentificacionService _service;

        public EditModel(ITipoIdentificacionService service)
        {
            _service = service;
        }

        [BindProperty]
        public TipoIdentificacion Tipo { get; set; }

        public void OnGet(int id)
        {
            Tipo = _service.ObtenerTodos().FirstOrDefault(t => t.ID_Tipo_Identificacion == id);
        }

        public IActionResult OnPost()
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
                _service.Actualizar(Tipo);

                ViewData["ModalType"] = "success";
                ViewData["ModalTitle"] = "Actualización exitosa";
                ViewData["ModalMessage"] = "El tipo de identificación fue actualizado correctamente.";
                ViewData["RedirectPage"] = "Index"; // redirige al cerrar modal
            }
            catch (InvalidOperationException ex)
            {
                ViewData["ModalType"] = "error";
                ViewData["ModalTitle"] = "Error";
                ViewData["ModalMessage"] = ex.Message;
            }

            return Page();
        }
    }
}
