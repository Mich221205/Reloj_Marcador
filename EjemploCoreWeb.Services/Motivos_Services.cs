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
    public class Motivos_Services : IMotivos_Inconsistencia
    {
        private readonly MotivosRepository _motivosRepository;

        public Motivos_Services(MotivosRepository motivosRepository)
        {
            _motivosRepository = motivosRepository;
        }

        public Task<IEnumerable<Motivos_Inconsistencias>> CargarMotivosAsync()
        {
            return _motivosRepository.CargarMotivosAsync();
        }

        public Task<Motivos_Inconsistencias?> Cargar_Motivo_X_IDAsync(int idMotivo)
        {
            return _motivosRepository.Cargar_Motivo_X_IDAsync(idMotivo);
        }

        public Task<int> InsertAsync(Motivos_Inconsistencias motivo)
        {

            if (motivo == null)
                throw new ArgumentNullException(nameof(motivo), "El motivo no puede ser nulo.");

            if (string.IsNullOrWhiteSpace(motivo.Nombre_Motivo))
                throw new InvalidOperationException("El nombre del motivo es obligatorio.");

            if (motivo.Nombre_Motivo.Length > 40)
                throw new InvalidOperationException("El nombre del motivo no debe superar los 40 caracteres.");

            // Validar solo letras y espacios (incluyendo acentos y ñ/Ñ)
            if (!System.Text.RegularExpressions.Regex.IsMatch(
                    motivo.Nombre_Motivo,
                    @"^[A-Za-zÁÉÍÓÚáéíóúÑñ\s]+$"))
            {
                throw new InvalidOperationException("El nombre del motivo solo puede contener letras y espacios.");
            }

            // Si pasa las validaciones -> se inserta en la DB :)
            return _motivosRepository.InsertMotivoAsync(motivo);
        }

        public Task<int> UpdateAsync(Motivos_Inconsistencias motivo)
        {
            return _motivosRepository.UpdateMotivoAsync(motivo);
        }

        public Task<int> DeleteAsync(int idMotivo)
        {

            return _motivosRepository.DeleteMotivoAsync(idMotivo);
        }


    }
}
