using EjemploCoreWeb.Entities;
using EjemploCoreWeb.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace EjemploProyecto.Pages.Admin.Usuarios
{
    public class CreateModel : PageModel
    {
        private readonly IUsuarioService _svc;
        public CreateModel(IUsuarioService svc) => _svc = svc;

        public SelectList TiposSelectList { get; set; } = default!;
        public SelectList RolesSelectList { get; set; } = default!;

        public class VM
        {
            [Required] public int ID_Tipo_Identificacion { get; set; } // UI-only

            [Required, StringLength(30)]
            public string Identificacion { get; set; } = "";

            [Required, StringLength(50), RegularExpression("^[A-Za-zÁÉÍÓÚáéíóúñÑ ]+$")]
            public string Nombre { get; set; } = "";

            [Required, StringLength(50), RegularExpression("^[A-Za-zÁÉÍÓÚáéíóúñÑ ]+$")]
            public string Apellido_1 { get; set; } = "";

            [Required, StringLength(50), RegularExpression("^[A-Za-zÁÉÍÓÚáéíóúñÑ ]+$")]
            public string Apellido_2 { get; set; } = "";

            [Required, EmailAddress]
            public string Correo { get; set; } = "";

            public string? Telefono { get; set; }

            [Required]
            public int ID_Rol_Usuario { get; set; }

            // UI-only para formularios; la entidad no tiene esta propiedad
            public string Nom_Usuario { get; set; } = "";

            [Required, StringLength(100, MinimumLength = 6)]
            [RegularExpression(@"^(?=.*\d)(?=.*[^\w\s]).{6,}$",
                ErrorMessage = "La contraseña debe contener números y al menos un símbolo.")]
            public string Password { get; set; } = "";

            // ⚠️ En la entidad es bool
            [Required]
            public bool Estado { get; set; } = true;
        }

        [BindProperty] public VM Input { get; set; } = new();

        public async Task OnGetAsync()
        {
            RolesSelectList = new SelectList(await _svc.ListarRolesAsync(),
                "ID_Rol_Usuario", "Nombre_Rol");

            TiposSelectList = new SelectList(await _svc.ListarTiposAsync(),
                "ID_Tipo_Identificacion", "Tipo_Identificacion");
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                await OnGetAsync(); // recargar combos
                return Page();
            }

            var u = new Usuario
            {
                Identificacion = Input.Identificacion,
                Nombre = Input.Nombre,
                Apellido_1 = Input.Apellido_1,
                Apellido_2 = Input.Apellido_2,
                Correo = Input.Correo,
                Telefono = Input.Telefono,
                // ⚠️ entidad usa Id_Rol_Usuario
                Id_Rol_Usuario = Input.ID_Rol_Usuario,
                // La entidad NO tiene Nom_Usuario
                Estado = Input.Estado // bool -> bool
            };

            await _svc.CrearAsync(u, Input.Password);
            TempData["Ok"] = "Usuario creado correctamente.";
            return RedirectToPage("Index");
        }
    }
}
