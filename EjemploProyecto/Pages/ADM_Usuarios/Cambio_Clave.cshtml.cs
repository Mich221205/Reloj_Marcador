using EjemploCoreWeb.Entities;
using EjemploCoreWeb.Services;
using EjemploCoreWeb.Services.Abstract;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Text.RegularExpressions;

namespace EjemploProyecto.Pages.ADM_Usuarios
{
    public class Cambio_ClaveModel : PageModel
    {

        private readonly IUsuarioService _usuarioService;



        public Cambio_ClaveModel(IUsuarioService usuarioService)
        {
            _usuarioService = usuarioService;
            Usuario = new Usuario();
        }

        [BindProperty]
        public Usuario Usuario { get; set; }




        //public async Task<IActionResult> OnGetAsync(string id)
        //{
        //}
            


        //public async Task<IActionResult> OnPostAsync()
        //{


        //}






    }
}
