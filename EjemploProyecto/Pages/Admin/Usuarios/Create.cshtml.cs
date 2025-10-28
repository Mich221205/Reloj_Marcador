using System;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;
using EjemploCoreWeb.Entities;
using System.Data;
using System.Text.Json;
using Dapper;
using EjemploCoreWeb.Repository;
using IBitacoraService = EjemploCoreWeb.Services.Abstract.IBitacoraService;
using IUsuarioService = EjemploCoreWeb.Services.Interfaces.IUsuarioService;

namespace EjemploProyecto.Pages.Admin.Usuarios
{
    public class CreateModel : PageModel
    {
        private readonly IUsuarioService _svc;
        private readonly IBitacoraService _bitacora;
        private readonly ILogger<CreateModel> _logger;
        private readonly IWebHostEnvironment _env;
        private readonly IDbConnectionFactory _db;

        private const int ACCION_CREAR = 1;
        private const int ACCION_CONSULTA = 4;
        private const int ACCION_ERROR = 5;

        public CreateModel(IUsuarioService svc,
                           IBitacoraService bitacora,
                           ILogger<CreateModel> logger,
                           IWebHostEnvironment env,
                           IDbConnectionFactory db)
        {
            _svc = svc;
            _bitacora = bitacora;
            _logger = logger;
            _env = env;
            _db = db;
        }

        public SelectList TiposSelectList { get; set; } = default!;
        public SelectList RolesSelectList { get; set; } = default!;

        public class VM
        {
            [Display(Name = "Tipo de identificación")]
            [Required(ErrorMessage = "El tipo de identificación es obligatorio.")]
            public int ID_Tipo_Identificacion { get; set; }

            [Display(Name = "Identificación (nombre de usuario)")]
            [Required(ErrorMessage = "La identificación es obligatoria.")]
            [StringLength(20, ErrorMessage = "La identificación no debe superar 20 caracteres.")]
            public string Identificacion { get; set; } = "";

            [Display(Name = "Nombre")]
            [Required(ErrorMessage = "El nombre es obligatorio.")]
            [StringLength(50, ErrorMessage = "El nombre no debe superar 50 caracteres.")]
            [RegularExpression("^[A-Za-zÁÉÍÓÚáéíóúñÑ ]+$", ErrorMessage = "El nombre solo puede contener letras y espacios.")]
            public string Nombre { get; set; } = "";

            [Display(Name = "Apellido")]
            [Required(ErrorMessage = "El apellido es obligatorio.")]
            [StringLength(50, ErrorMessage = "El apellido no debe superar 50 caracteres.")]
            [RegularExpression("^[A-Za-zÁÉÍÓÚáéíóúñÑ ]+$", ErrorMessage = "El apellido solo puede contener letras y espacios.")]
            public string Apellido { get; set; } = "";

            [Display(Name = "Correo")]
            [Required(ErrorMessage = "El correo es obligatorio.")]
            [EmailAddress(ErrorMessage = "Ingrese un correo electrónico válido.")]
            [StringLength(100, ErrorMessage = "El correo no debe superar 100 caracteres.")]
            public string Correo { get; set; } = "";

            [Display(Name = "Contraseña")]
            [Required(ErrorMessage = "La contraseña es obligatoria.")]
            [StringLength(100, MinimumLength = 8, ErrorMessage = "La contraseña debe tener al menos 8 caracteres.")]
            [RegularExpression(@"^(?=.*\d)(?=.*[^\w\s]).{8,}$", ErrorMessage = "La contraseña debe incluir al menos un número y un símbolo.")]
            public string Password { get; set; } = "";

            [Display(Name = "Rol")]
            [Required(ErrorMessage = "Debe seleccionar un rol.")]
            public int ID_Rol_Usuario { get; set; }

            [Display(Name = "Estado")]
            [Required(ErrorMessage = "Debe seleccionar el estado.")]
            public bool Estado { get; set; } = true;
        }

        [BindProperty] public VM Input { get; set; } = new();

        public async Task OnGetAsync()
        {
            RolesSelectList = new SelectList(await _svc.ListarRolesAsync(), "ID_Rol_Usuario", "Nombre_Rol");
            TiposSelectList = new SelectList(await _svc.ListarTiposAsync(), "ID_Tipo_Identificacion", "Tipo_Identificacion");

            var actorId = await ResolveActorIdAsync();
            if (actorId > 0)
            {
                try
                {
                    await _bitacora.Registrar(actorId, ACCION_CONSULTA, new { Mensaje = "El usuario consulta 'Nuevo Usuario'." }, "Consulta Usuarios - Nuevo");
                }
                catch
                {
                    await InsertBitacoraRawAsync(actorId, ACCION_CONSULTA, new { Mensaje = "El usuario consulta 'Nuevo Usuario'." }, "Consulta Usuarios - Nuevo");
                }
            }
            else
            {
                await InsertBitacoraRawAsync(0, ACCION_CONSULTA, new { Mensaje = "El usuario consulta 'Nuevo Usuario' sin actor." }, "Consulta Usuarios - Nuevo");
                TempData["Warn"] = "No se pudo identificar al usuario para bitácora (consulta).";
            }
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
                Apellido_1 = Input.Apellido,
                Apellido_2 = null,
                Correo = Input.Correo,
                Id_Rol_Usuario = Input.ID_Rol_Usuario,
                Estado = Input.Estado
            };

            var actorId = await ResolveActorIdAsync();

            try
            {
                await _svc.CrearAsync(u, Input.ID_Tipo_Identificacion, Input.Password);

                if (actorId > 0)
                {
                    try
                    {
                        await _bitacora.Registrar(actorId, ACCION_CREAR, new { Nuevo = u }, "Crear Usuario");
                    }
                    catch
                    {
                        await InsertBitacoraRawAsync(actorId, ACCION_CREAR, new { Nuevo = u }, "Crear Usuario");
                    }
                }
                else
                {
                    await InsertBitacoraRawAsync(0, ACCION_CREAR, new { Nuevo = u }, "Crear Usuario");
                    TempData["Warn"] = "Usuario creado, pero no se registró bitácora con actor real.";
                }

                TempData["Ok"] = "Usuario creado correctamente.";
                return RedirectToPage("Index");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear usuario {@Input}", new
                {
                    Input.ID_Tipo_Identificacion,
                    Input.Identificacion,
                    Input.Nombre,
                    Input.Apellido,
                    Input.Correo,
                    Input.ID_Rol_Usuario,
                    Input.Estado
                });

                if (actorId > 0)
                {
                    try
                    {
                        await _bitacora.Registrar(actorId, ACCION_ERROR, new
                        {
                            Mensaje = "Error al crear usuario",
                            Ex = ex.Message,
                            Input = new
                            {
                                Input.ID_Tipo_Identificacion,
                                Input.Identificacion,
                                Input.Nombre,
                                Input.Apellido,
                                Input.Correo,
                                Input.ID_Rol_Usuario,
                                Input.Estado
                            }
                        }, "Error Crear Usuario");
                    }
                    catch
                    {
                        await InsertBitacoraRawAsync(actorId, ACCION_ERROR, new
                        {
                            Mensaje = "Error al crear usuario",
                            Ex = ex.Message
                        }, "Error Crear Usuario");
                    }
                }
                else
                {
                    await InsertBitacoraRawAsync(0, ACCION_ERROR, new
                    {
                        Mensaje = "Error al crear usuario sin actor",
                        Ex = ex.Message
                    }, "Error Crear Usuario");
                }

                TempData["Error"] = _env.IsDevelopment()
                    ? $"Ocurrió un error al registrar el usuario: {ex.Message}"
                    : "Ocurrió un error al registrar el usuario.";

                await OnGetAsync();
                return Page();
            }
        }

        private int? GetIntClaim(params string[] types)
        {
            foreach (var t in types)
            {
                var v = User.FindFirstValue(t);
                if (int.TryParse(v, out var id) && id > 0) return id;
            }
            return null;
        }

        private async Task<int> ResolveActorIdAsync()
        {
            var v = User.FindFirstValue(ClaimTypes.NameIdentifier)
                    ?? User.FindFirstValue("ID_Usuario")
                    ?? User.FindFirstValue("id_usuario");
            if (int.TryParse(v, out var idFromClaim) && idFromClaim > 0)
                return idFromClaim;

            var sid = HttpContext.Session?.GetInt32("ID_Usuario");
            if (sid.HasValue && sid.Value > 0) return sid.Value;

            var username = User.Identity?.Name;
            if (!string.IsNullOrWhiteSpace(username))
            {
                try
                {
                    using var cn = _db.CreateConnection();
                    const string sql = @"SELECT ID_Usuario FROM usuario WHERE Identificacion = @user LIMIT 1;";
                    var actorId = await cn.ExecuteScalarAsync<int?>(sql, new { user = username });
                    if (actorId.HasValue && actorId.Value > 0) return actorId.Value;
                }
                catch { }
            }

            return 0;
        }

        private async Task<int> GetSafeActorIdAsync(int actorId)
        {
            if (actorId > 0) return actorId;
            var retry = await ResolveActorIdAsync();
            if (retry > 0) return retry;
            using var cn = _db.CreateConnection();
            var any = await cn.ExecuteScalarAsync<int?>("SELECT MIN(ID_Usuario) FROM usuario;");
            return any ?? 1;
        }

        private async Task InsertBitacoraRawAsync(int actorId, int idAccion, object detalle, string nombreAccion)
        {
            var uid = await GetSafeActorIdAsync(actorId);
            var json = JsonSerializer.Serialize(detalle);
            using var cn = _db.CreateConnection();
            const string sql = @"INSERT INTO bitacora (Fecha_Registro, ID_Usuario, ID_Accion, Descripcion_Accion)
                                 VALUES (NOW(), @uid, @acc, @json);";
            await cn.ExecuteAsync(sql, new { uid, acc = idAccion, json });
        }
    }
}

