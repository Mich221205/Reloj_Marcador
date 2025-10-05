namespace EjemploCoreWeb.Services.Abstract
{
    public interface IBitacoraService
    {
        Task Registrar(int idUsuario, int idAccion, object detalle, string nombreAccion);
    }
}
