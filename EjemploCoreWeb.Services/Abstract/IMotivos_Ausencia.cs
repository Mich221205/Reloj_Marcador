using EjemploCoreWeb.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EjemploCoreWeb.Services.Abstract
{
    public interface IMotivos_Ausencia
    {

        //Motivos de Ausencia

        Task<IEnumerable<Motivos_Ausencia>> CargarMotivosAsync();
        Task<Motivos_Ausencia?> Cargar_Motivo_X_IDAsync(int idMotivo);
        Task<int> InsertAsync(Motivos_Ausencia motivo);
        Task<int> UpdateAsync(Motivos_Ausencia motivo);
        Task<int> DeleteAsync(int idMotivo);


    }
}
