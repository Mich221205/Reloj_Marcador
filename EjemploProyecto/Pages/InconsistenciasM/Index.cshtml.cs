using EjemploCoreWeb.Entities;
using EjemploCoreWeb.Services.Abstract;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EjemploProyecto.Pages.Inconsistencias
{
    public class IndexModel : PageModel
    {
        private readonly IInconsistenciaService _service;
        private readonly IBitacoraService _bitacoraService;

        public IEnumerable<Inconsistencia> Inconsistencias { get; set; } = new List<Inconsistencia>();
        public int TotalRegistros { get; set; }
        public int PaginaActual { get; set; } = 1;
        public int TamañoPagina { get; set; } = 10;

        public IndexModel(IInconsistenciaService service, IBitacoraService bitacoraService)
        {
            _service = service;
            _bitacoraService = bitacoraService;
        }

        public async Task OnGetAsync(int pagina = 1)
        {
            try
            {
                PaginaActual = pagina;
                Inconsistencias = await _service.Listar(PaginaActual, TamañoPagina);
                TotalRegistros = await _service.Contar();

                await _bitacoraService.Registrar(1, 4, "El usuario consult� Inconsistencias", "CONSULTA");

                // Mensajes pasados desde Create/Edit/Delete
                if (TempData.ContainsKey("SuccessMessage"))
                {
                    TempData["ModalType"] = "success";
                    TempData["ModalTitle"] = "�xito";
                    TempData["ModalMessage"] = TempData["SuccessMessage"];
                }
                else if (TempData.ContainsKey("ErrorMessage"))
                {
                    TempData["ModalType"] = "error";
                    TempData["ModalTitle"] = "Error";
                    TempData["ModalMessage"] = TempData["ErrorMessage"];
                }
            }
            catch (MySql.Data.MySqlClient.MySqlException ex)
            {
                // Error de base de datos (como el Check constraint)
                await _bitacoraService.Registrar(1, 4, $"Error MySQL: {ex.Message}", "ERROR");

                TempData["ModalType"] = "error";
                TempData["ModalTitle"] = "Error de Base de Datos";
                TempData["ModalMessage"] = "Ocurri� un problema al consultar las inconsistencias. Verifique los datos o contacte al administrador.";
            }
            catch (Exception ex)
            {
                // Cualquier otro tipo de error
                await _bitacoraService.Registrar(1, 4, $"Excepci�n general: {ex.Message}", "ERROR");

                TempData["ModalType"] = "error";
                TempData["ModalTitle"] = "Error inesperado";
                TempData["ModalMessage"] = "Ocurri� un error inesperado al cargar la informaci�n.";
            }
        }
    }
}

