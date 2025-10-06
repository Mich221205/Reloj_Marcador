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

        public async Task<int> InsertDetalleHorarioAsync(Detalle_Horarios detalle)
        {
            // Validación de día
            if (string.IsNullOrWhiteSpace(detalle.Dia))
                throw new InvalidOperationException("Debe seleccionar un día.");

            // Validación de horas
            if (detalle.Hora_Ingreso <= 0 || detalle.Hora_Ingreso > 24)
                throw new InvalidOperationException("La hora de ingreso debe ser mayor que 0 y menor o igual a 24.");

            if (detalle.Hora_Salida <= 0 || detalle.Hora_Salida > 24)
                throw new InvalidOperationException("La hora de salida debe ser mayor que 0 y menor o igual a 24.");

            // Validación de minutos
            if (detalle.Minuto_Ingreso < 0 || detalle.Minuto_Ingreso > 59)
                throw new InvalidOperationException("El minuto de ingreso debe estar entre 0 y 59.");

            if (detalle.Minuto_Salida < 0 || detalle.Minuto_Salida > 59)
                throw new InvalidOperationException("El minuto de salida debe estar entre 0 y 59.");

            // Validación lógica adicional: la salida no puede ser antes que la entrada
            var horaInicioTotal = detalle.Hora_Ingreso * 60 + detalle.Minuto_Ingreso;
            var horaFinTotal = detalle.Hora_Salida * 60 + detalle.Minuto_Salida;

            if (horaFinTotal <= horaInicioTotal)
                throw new InvalidOperationException("La hora de salida debe ser posterior a la hora de ingreso.");

            // Si todo está bien, proceder con la inserción
            return await _admHorariosRepository.InsertDetalleHorarioAsync(detalle);
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


        public Task<IEnumerable<Horarios>> Obtener_Areas_UsuarioAsync(string identificacion)
        {
            return _admHorariosRepository.Obtener_Areas_UsuarioAsync(identificacion);
        }


    }
}
