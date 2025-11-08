using EjemploCoreWeb.Entities;
using EjemploCoreWeb.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EjemploCoreWeb.Services
{
    public class Proceso_Generar_Inconsistencias_MarcasService
    {

        private readonly Proceso_Generar_Inconsistencias_Marcas _procesoRepository;

        public Proceso_Generar_Inconsistencias_MarcasService(Proceso_Generar_Inconsistencias_Marcas ProcesoRepository)
        {
            _procesoRepository = ProcesoRepository;
        }

        public async Task EjecutarAsync(DateTime fechaInicio, DateTime fechaFin, int? areaId = null, int? usuarioId = null)
        {
            var tolerancia = await _procesoRepository.ObtenerToleranciaAsync();
            var funcionarios = await _procesoRepository.ObtenerFuncionariosAsync(areaId, usuarioId);

            int totalInconsistencias = 0;

            foreach (var fecha in RangoFechas(fechaInicio, fechaFin))
            {
                string dia = fecha.ToString("dddd", new System.Globalization.CultureInfo("es-ES"));

                foreach (var fun in funcionarios)
                {
                    if (await _procesoRepository.EsDiaExcluidoAsync(fun.ID_Usuario, fecha))
                        continue;

                    var horario = await _procesoRepository.ObtenerHorarioAsync(fun.ID_Usuario, dia);
                    var marcas = await _procesoRepository.ObtenerMarcasAsync(fun.ID_Usuario, fecha);

                    var inconsistencias = EvaluarInconsistencias(fecha, fun, horario, marcas, tolerancia);

                    await _procesoRepository.GuardarInconsistenciasAsync(inconsistencias, fun.Identificacion);

                    totalInconsistencias += inconsistencias.Count;
                }
            }

            await _procesoRepository.RegistrarBitacoraAsync(fechaInicio, fechaFin, totalInconsistencias);
        }

        // Lógica para evaluar inconsistencias con los siguientes tipos:
        // - Atraso
        // - Salida Temprana
        // - Ausencia (Sin marcas del todo)
        // - Marca faltante (Solo entrada o solo salida)
        // - Marca fuera de horario/área (Entrada antes del horario permitido o salida después del horario permitido)

        private List<InconsistenciaDetectada> EvaluarInconsistencias(
            DateTime fecha,
            Usuario funcionario,
            Detalle_Horarios? horario,
            IEnumerable<Marca> marcas,
            ParametrosInconsistencia tolerancia){

            //Declaramos la lista con las inconsistencias
            var lista = new List<InconsistenciaDetectada>();

            // No evaluar fines de semana (Si se registran horarios para sábado y domingo, pero por si acaso que Domingo no jajaj)
            if (fecha.DayOfWeek == DayOfWeek.Sunday)
                return lista;

            // Sin un horario registrado:
            if (horario == null)
            {
                lista.Add(new InconsistenciaDetectada(

                    "Sin horario registrado",
                    fecha,
                    funcionario.ID_Usuario,
                    "El funcionario no tiene un horario registrado para este día.",
                    null));
                
                return lista;
            }

            var entrada = marcas.FirstOrDefault(m => m.Tipo_Marca == "Entrada");
            var salida = marcas.FirstOrDefault(m => m.Tipo_Marca == "Salida");

            var horaEntradaProg = new TimeSpan(horario.Hora_Ingreso, horario.Minuto_Ingreso, 0);
            var horaSalidaProg = new TimeSpan(horario.Hora_Salida, horario.Minuto_Salida, 0);

            // Ausenciaaa total
            if (entrada == null && salida == null)
            {
                lista.Add(new InconsistenciaDetectada(

                    "Ausencia",
                    fecha,
                    funcionario.ID_Usuario,
                    "No existen marcas de entrada ni salida registradas para este día.",
                    null));

                return lista;
            }

            // Marca faltante (solo entrada o solo salida)
            if (entrada == null || salida == null)
            {
                string detalle = entrada == null
                    ? "Falta marca de entrada."
                    : "Falta marca de salida.";

                lista.Add(new InconsistenciaDetectada(
                    "Marca faltante",
                    fecha,
                    funcionario.ID_Usuario,
                    detalle,
                    entrada == null ? (salida != null ? $"MarcaID:{salida.ID_Marca}" : null)
                                    : $"MarcaID:{entrada.ID_Marca}"
                ));
            }

            // Atrasos
            if (entrada != null)
            {
                var atrasoMin = (entrada.Fecha_Hora.TimeOfDay - horaEntradaProg).TotalMinutes;
                if (atrasoMin > tolerancia.Tolerancia_Atraso)
                {
                    lista.Add(new InconsistenciaDetectada(
                        "Atraso",
                        fecha,
                        funcionario.ID_Usuario,
                        $"El funcionario marcó entrada {Math.Round(atrasoMin)} minutos después de la hora programada ({horaEntradaProg:hh\\:mm}).",
                        $"MarcaID:{entrada.ID_Marca}"
                    ));
                }
            }

            // Salida temprana
            if (salida != null)
            {
                var salidaTempranaMin = (horaSalidaProg - salida.Fecha_Hora.TimeOfDay).TotalMinutes;
                if (salidaTempranaMin > tolerancia.Tolerancia_Salida_Temprana)
                {
                    lista.Add(new InconsistenciaDetectada(
                        "Salida temprana",
                        fecha,
                        funcionario.ID_Usuario,
                        $"El funcionario registró salida {Math.Round(salidaTempranaMin)} minutos antes de la hora programada ({horaSalidaProg:hh\\:mm}).",
                        $"MarcaID:{salida.ID_Marca}"
                    ));
                }
            }

            // Marcas fueras del horario establecido 
            if (entrada != null)
            {
                var minutosAntes = (horaEntradaProg - entrada.Fecha_Hora.TimeOfDay).TotalMinutes;
                if (minutosAntes > 0 && minutosAntes > tolerancia.Tolerancia_Atraso)
                {
                    lista.Add(new InconsistenciaDetectada(
                        "Marca fuera de horario",
                        fecha,
                        funcionario.ID_Usuario,
                        $"La marca de entrada se registró {Math.Round(minutosAntes)} minutos antes del horario permitido ({horaEntradaProg:hh\\:mm}).",
                        $"MarcaID:{entrada.ID_Marca}"
                    ));
                }
            }

            if (salida != null)
            {
                var minutosDespues = (salida.Fecha_Hora.TimeOfDay - horaSalidaProg).TotalMinutes;
                if (minutosDespues > 0 && minutosDespues > tolerancia.Tolerancia_Salida_Temprana)
                {
                    lista.Add(new InconsistenciaDetectada(
                        "Marca fuera de horario",
                        fecha,
                        funcionario.ID_Usuario,
                        $"La marca de salida se registró {Math.Round(minutosDespues)} minutos después del horario permitido ({horaSalidaProg:hh\\:mm}).",
                        $"MarcaID:{salida.ID_Marca}"
                    ));
                }
            }

            return lista;
        }


        //HELPER: Generar rango de fechas
        private IEnumerable<DateTime> RangoFechas(DateTime inicio, DateTime fin)
        {
            for (var f = inicio.Date; f <= fin.Date; f = f.AddDays(1))
                yield return f;
        }


    }
}
