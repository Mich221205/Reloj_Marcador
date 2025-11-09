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

        //--------------------------------------------------------------
        // PROC1 GENERAR INCONSISTENCIAS DE MARCAS (Jocsan Fonseca)
        //--------------------------------------------------------------
        // Lógica de negocio para generar las inconsistencias 
        //--------------------------------------------------------------

        // PROC1: Proceso principal que genera las inconsistencias de las marcas de los usuarios valorando absolutamente todo
        // Costó pero se logró :D
        public async Task PROC1_Async(DateTime fechaInicio, DateTime fechaFin, int? areaId = null, int? usuarioId = null)
        {
            // Obtenemos los parámetros de tolerancia de la bd
            var tolerancia = await _procesoRepository.ObtenerToleranciaAsync();

            // Obtenemos los funcionarios a evaluar
            var funcionarios = await _procesoRepository.ObtenerFuncionariosAsync(areaId, usuarioId);

            // Llevamos un contador con las inconsistencias totales generadas
            int totalInconsistencias = 0;

            // Recorremos cada fecha en el rango
            foreach (var fecha in RangoFechas(fechaInicio, fechaFin))
            {
                // Obtenemos el día de la semana en español para poder pasarselo al horario
                string dia = fecha.ToString("dddd", new System.Globalization.CultureInfo("es-ES"));

                // Recorremos cada funcionario ahora jajaj
                foreach (var fun in funcionarios)
                {
                    // Verificamos si el día es alguna exclusión para el funcionario
                    if (await _procesoRepository.EsDiaExcluidoAsync(fun.ID_Usuario, fecha))
                        continue;


                    // Obtenemos el horario del funcionario para ese comparar con ese día
                    var horario = await _procesoRepository.ObtenerHorarioAsync(fun.ID_Usuario, dia);

                    // Obtenemos las marcas del funcionario para la fecha
                    var marcas = await _procesoRepository.ObtenerMarcasAsync(fun.ID_Usuario, fecha);

                    // Evaluamos las inconsistencias para ese funcionario en esa fecha
                    var inconsistencias = EvaluarInconsistencias(fecha, fun, horario, marcas, tolerancia);

                    // Registramos las inconsistencias encontradasssss
                    await _procesoRepository.GuardarInconsistenciasAsync(inconsistencias, fun.Identificacion);

                    // Actualizamos nuestro contador total
                    totalInconsistencias += inconsistencias.Count;
                }
            }

            // Finalmente, registramos en la bitácora todo el proceso
            await _procesoRepository.RegistrarBitacoraAsync(fechaInicio, fechaFin, totalInconsistencias);
        }


        // Lógica para evaluar inconsistencias con los siguientes tipos:
        // - Atraso
        // - Salida Temprana
        // - Ausencia (Sin marcas del todo)
        // - Marca faltante (Solo entrada o solo salida)
        // - Marca fuera de horario/área (Entrada antes del horario permitido o salida después del horario permitido)

        //Aqui es donde evalúo que tipo de inconsistencia es
        private List<InconsistenciaDetectada> EvaluarInconsistencias(
            
            DateTime fecha,
            Usuario funcionario,
            Detalle_Horarios? horario,
            IEnumerable<Marca> marcas,
            ParametrosInconsistencia tolerancia)
        
        {

            //Declaramos una lista con las inconsistencias
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

            // Obtener marcas de entrada
            var entrada = marcas.FirstOrDefault(m => m.Tipo_Marca == "Entrada");

            // Obtener marcas de salida
            var salida = marcas.FirstOrDefault(m => m.Tipo_Marca == "Salida");

            // Definir hora de entrada programada
            var horaEntradaProg = new TimeSpan(horario.Hora_Ingreso, horario.Minuto_Ingreso, 0);

            // Definir hora de salida programada
            var horaSalidaProg = new TimeSpan(horario.Hora_Salida, horario.Minuto_Salida, 0);


            // Tipo: Ausenciaaa total
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

            // Tipo: Marca faltante (solo entrada o solo salida)
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

            // Tipo: Atrasos
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

            // Tipo: Salida temprana
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

            // Tipo: Marcas fueras del horario establecido 
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
