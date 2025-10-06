using EjemploCoreWeb.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EjemploCoreWeb.Services.Abstract
{
    public interface IHorarios
    {
        // SELECTS
        Task<IEnumerable<Horarios>> Obtener_Horario_UsuarioAsync(string identificacion);
        Task<IEnumerable<Detalle_Horarios>> Obtener_Detalles_HorarioAsync(int idHorario);

        //Task<IEnumerable<Horarios>> Obtener_Todas_AreasAsync();
        Task<IEnumerable<Horarios>> Obtener_Areas_UsuarioAsync(string identificacion);


        // INSERTS
        Task<int> InsertHorarioAsync(Horarios horario);
        Task<int> InsertDetalleHorarioAsync(Detalle_Horarios detalle);

        // DELETES
        Task<int> DeleteHorarioAsync(int idHorario);
        Task<int> DeleteDetalleHorarioAsync(int idDetalle);
    }
}
