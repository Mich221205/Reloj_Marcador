using EjemploCoreWeb.Entities;
using EjemploCoreWeb.Repository;
using EjemploCoreWeb.Services.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EjemploCoreWeb.Services
{
    public class HorariosServices : IHorarios
    {
        private readonly AdmHorariosRepository _admHorariosRepository;

        public HorariosServices(AdmHorariosRepository HorariosRepository)
        {
            _admHorariosRepository = HorariosRepository;
        }

        public Task<IEnumerable<Horarios>> Obtener_Horario_UsuarioAsync(string identificacion)
        {
            return _admHorariosRepository.Obtener_Horario_UsuarioAsync(identificacion);
        }

        public Task<IEnumerable<Detalle_Horarios>> Obtener_Detalles_HorarioAsync(int idHorario)
        {
            return _admHorariosRepository.Obtener_Detalles_HorarioAsync(idHorario);
        }

        // INSERTS
        public Task<int> InsertHorarioAsync(Horarios horario)
        {
            return _admHorariosRepository.InsertHorarioAsync(horario);
        }

        public Task<int> InsertDetalleHorarioAsync(Detalle_Horarios detalle)
        {
            return _admHorariosRepository.InsertDetalleHorarioAsync(detalle);
        }

        // DELETES
        public Task<int> DeleteHorarioAsync(int idHorario)
        {
            return _admHorariosRepository.DeleteHorarioAsync(idHorario);
        }

        public Task<int> DeleteDetalleHorarioAsync(int idDetalle)
        {
            return _admHorariosRepository.DeleteDetalleHorarioAsync(idDetalle);
        }


    }
}
