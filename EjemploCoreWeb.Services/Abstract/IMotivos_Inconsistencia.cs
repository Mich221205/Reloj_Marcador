using EjemploCoreWeb.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EjemploCoreWeb.Services.Abstract
{
    public interface IMotivos_Inconsistencia
    {

        //Motivos de inconsistencias

        Task<IEnumerable<Motivos_Inconsistencias>> CargarMotivosAsync();
        Task<Motivos_Inconsistencias?> Cargar_Motivo_X_IDAsync(int idMotivo);
        Task<int> InsertAsync(Motivos_Inconsistencias motivo);
        Task<int> UpdateAsync(Motivos_Inconsistencias motivo);
        Task<int> DeleteAsync(int idMotivo);


    }
}
