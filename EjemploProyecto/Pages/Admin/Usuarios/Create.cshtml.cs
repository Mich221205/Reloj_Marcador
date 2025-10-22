using EjemploCoreWeb.Entities;
using EjemploCoreWeb.Services.Interfaces;      // IUsuarioService
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
            [Required(ErrorMessage = "El tipo de identificación es obligatorio.")]
            public int ID_Tipo_Identificacion { get; set; }   // UI-only, se pasa como parámetro

            [Required, StringLength(30)]
            public string Identificacion { get; set; } = "";

            [Required, StringLength(50), RegularExpression("^[A-Za-zÁÉÍÓÚáéíóúñÑ ]+$",
                ErrorMessage = "El nombre solo puede contener letras y espacios y máx. 50 caracteres.")]
            public string Nombre { get; set; } = "";

            [Required, StringLength(50), RegularExpression("^[A-Za-zÁÉÍÓÚáéíóúñÑ ]+$",
                ErrorMessage = "El primer apellido solo puede contener letras y espacios y máx. 50 caracteres.")]
            public string Apellido_1 { get; set; } = "";

            [Required, StringLength(50), RegularExpression("^[A-Za-zÁÉÍÓÚáéíóúñÑ ]+$",
                ErrorMessage = "El segundo apellido solo puede contener letras y espacios y máx. 50 caracteres.")]
            public string Apellido_2 { get; set; } = "";

            [Required, EmailAddress]
            public string Correo { get; set; } = "";

            public string? Telefono { get; set; }

            [Required]
            public int ID_Rol_Usuario { get; set; }

            public string Nom_Usuario { get; set; } = "";     // UI-only

            [Required, StringLength(100, MinimumLength = 6)]
            [RegularExpression(@"^(?=.*\d)(?=.*[^\w\s]).{6,}$",
                ErrorMessage = "La contraseña debe contener números y al menos un símbolo.")]
            public string Password { get; set; } = "";

            [Required] public bool Estado { get; set; } = true;   // en la entidad es bool
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
                await OnGetAsync();
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
                Id_Rol_Usuario = Input.ID_Rol_Usuario,
                Estado = Input.Estado
            };

            // OJO: la interfaz debe tener CrearAsync(Usuario u, int tipoId, string plainPassword)
            await _svc.CrearAsync(u, Input.ID_Tipo_Identificacion, Input.Password);

            TempData["Ok"] = "Usuario creado correctamente.";
            return RedirectToPage("Index");
        }
    }
}
