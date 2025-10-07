using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;
using System.Security.Claims;

namespace EjemploProyecto.Pages.ADM_Bienvenida
{
    public class BienvenidaModel : PageModel
    {

        public string Nombre { get; set; } = "Invitado";
        public string Apellido1 { get; set; } = "";
        public string Apellido2 { get; set; } = "";
        public string RolUsuario { get; set; } = "Invitado";
        public string AvatarUrl { get; set; } = "/images/avatar.png";

        public void OnGet()
        {
            // Leer datos de sesión (o de la autenticación real)
            var nombre = HttpContext.Session.GetString("Nombre") ?? "";
            var apellido1 = HttpContext.Session.GetString("Apellido1") ?? "";
            var apellido2 = HttpContext.Session.GetString("Apellido2") ?? "";
            var rol = HttpContext.Session.GetString("RolUsuario") ?? "Invitado";
            var avatar = HttpContext.Session.GetString("AvatarUrl") ?? "/images/avatar.png";

            Nombre = nombre;
            Apellido1 = apellido1;
            Apellido2 = apellido2;
            RolUsuario = rol;
            AvatarUrl = avatar;
        }

        // Propiedad combinada para mostrar en la vista
        public string NombreCompleto => $"{Nombre} {Apellido1} {Apellido2}";
    }
}

