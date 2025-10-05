using EjemploCoreWeb.Entities;
using EjemploCoreWeb.Services.Abstract;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace EjemploProyecto.Pages.ADM_Horarios
{
    public class Detalle_HorariosModel : PageModel
    {
        private readonly IHorarios _horariosService;

        public Detalle_HorariosModel(IHorarios horariosService)
        {
            _horariosService = horariosService;
            Horarios = new List<Horarios>();
            Detalles = new List<Detalle_Horarios>();
            DetalleHorario = new Detalle_Horarios();
        }

        [BindProperty]
        public List<Horarios> Horarios { get; set; }

        [BindProperty]
        public List<Detalle_Horarios> Detalles { get; set; }

        [BindProperty]
        public Detalle_Horarios DetalleHorario { get; set; }

        [BindProperty]
        public int HorarioSeleccionadoId { get; set; }

        public bool MostrarFormulario { get; set; }

        public string ModalType { get; set; }
        public string ModalTitle { get; set; }
        public string ModalMessage { get; set; }

        [BindProperty(SupportsGet = true)]
        public string IdentificacionUsuario { get; set; }

        public async Task<IActionResult> OnGetAsync(string id)
        {
            IdentificacionUsuario = id; // guarda la identificación del usuario actual

            var horarios = await _horariosService.Obtener_Horario_UsuarioAsync(id);
            if (horarios == null || !horarios.Any())
                return Page(); // sigue mostrando la vista vacía, pero mantiene la identificación

            Horarios = horarios.ToList();
            return Page();
        }

        public async Task<PartialViewResult> OnPostSeleccionarAsync(int id)
        {
            HorarioSeleccionadoId = id;
            MostrarFormulario = true;

            Detalles = (await _horariosService.Obtener_Detalles_HorarioAsync(id)).ToList();

            return Partial("_DetallesPanel", this);
        }

        public async Task<PartialViewResult> OnPostAgregarDetalleAsync()
        {
            if (int.TryParse(Request.Form["HorarioSeleccionadoId"], out int idHorario))
                HorarioSeleccionadoId = idHorario;

            MostrarFormulario = true;

            try
            {
                if (HorarioSeleccionadoId == 0)
                {
                    ModalType = "warning";
                    ModalTitle = "Advertencia";
                    ModalMessage = "Debe seleccionar un horario antes de agregar detalles.";
                }
                else
                {
                    DetalleHorario.ID_Horario = HorarioSeleccionadoId;

                    await _horariosService.InsertDetalleHorarioAsync(DetalleHorario);

                    Detalles = (await _horariosService.Obtener_Detalles_HorarioAsync(HorarioSeleccionadoId)).ToList();

                    ModalType = "success";
                    ModalTitle = "Éxito";
                    ModalMessage = "Detalle agregado correctamente.";
                }
            }
            catch (InvalidOperationException ex)
            {
                ModalType = "error";
                ModalTitle = "Error";
                ModalMessage = ex.Message;

                Detalles = (await _horariosService.Obtener_Detalles_HorarioAsync(HorarioSeleccionadoId)).ToList();
            }
            catch (Exception ex)
            {
                ModalType = "error";
                ModalTitle = "Error inesperado";
                ModalMessage = $"Se produjo un error: {ex.Message}";

                Detalles = (await _horariosService.Obtener_Detalles_HorarioAsync(HorarioSeleccionadoId)).ToList();
            }

            return Partial("_DetallesPanel", this);
        }

        public async Task<PartialViewResult> OnPostEliminarHorarioAsync(int id_Horario)
        {
            try
            {
                // Validar el ID del horario recibido
                if (id_Horario <= 0)
                {
                    ModalType = "warning";
                    ModalTitle = "Advertencia";
                    ModalMessage = "Debe seleccionar un horario válido para eliminar.";
                    return Partial("_DetallesPanel", this);
                }

                // Eliminar horario en BD
                await _horariosService.DeleteHorarioAsync(id_Horario);

                // Actualizar la lista de horarios después de eliminar
                var idUsuario = Request.Query["id"].ToString();
                Horarios = (await _horariosService.Obtener_Horario_UsuarioAsync(idUsuario))?.ToList() ?? new List<Horarios>();

                ModalType = "success";
                ModalTitle = "Éxito";
                ModalMessage = "El horario fue eliminado correctamente.";
            }
            catch (Exception ex)
            {
                ModalType = "error";
                ModalTitle = "Error";
                ModalMessage = $"No se pudo eliminar el horario: {ex.Message}";
            }

            // Devuelve el panel de detalles para mantener la vista actual
            return Partial("_DetallesPanel", this);
        }



        public async Task<PartialViewResult> OnPostEliminarDetalleAsync(int idDetalle)
        {
            if (int.TryParse(Request.Form["HorarioSeleccionadoId"], out int idHorario))
                HorarioSeleccionadoId = idHorario;

            MostrarFormulario = true;

            try
            {
                await _horariosService.DeleteDetalleHorarioAsync(idDetalle);

                Detalles = (await _horariosService.Obtener_Detalles_HorarioAsync(HorarioSeleccionadoId)).ToList();

                ModalType = "success";
                ModalTitle = "Éxito";
                ModalMessage = "El detalle fue eliminado correctamente.";
            }
            catch (Exception ex)
            {
                ModalType = "error";
                ModalTitle = "Error";
                ModalMessage = $"No se pudo eliminar el detalle: {ex.Message}";
            }

            return Partial("_DetallesPanel", this);
        }


        public async Task<PartialViewResult> OnPostCrearHorarioAsync(Horarios horario)
        {
            try
            {
                await _horariosService.InsertHorarioAsync(horario);
                ModalType = "success";
                ModalTitle = "Éxito";
                ModalMessage = "Horario creado correctamente.";

                // Refrescamos la lista
                Horarios = (await _horariosService.Obtener_Horario_UsuarioAsync(horario.Identificacion)).ToList();
            }
            catch (Exception ex)
            {
                ModalType = "error";
                ModalTitle = "Error";
                ModalMessage = ex.Message;
            }

            return Partial("_HorariosLista", this);
        }
    }
}
