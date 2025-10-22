using EjemploCoreWeb.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EjemploCoreWeb.Services.Abstract
{
    /// <summary>
    /// Interfaz para el servicio de autenticación/cambio de clave (NO la de admin).
    /// </summary>
    public interface IUsuarioService
    {
        Task<IEnumerable<Usuario>> GetAllAsync();
        Task<Usuario> Obtener_Usuario_X_Identificacion(string id);
        Task<int> Cambiar_Clave(Usuario usuario);
        string Autogenerar_Clave();
    }
}
