using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;


namespace EjemploCoreWeb.Services
{
    public class PROC1_Automatizado : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;

        public PROC1_Automatizado(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        //--------------------------------------------------------------
        // PROC1 GENERAR INCONSISTENCIAS DE MARCAS (Jocsan Fonseca)
        //--------------------------------------------------------------
        // Aquí es donde se automatiza la ejecución del proceso en segundo plano en horarios específicos
        // 12:20 PM, 5:20 PM, 10:40 PM para barrer inconsistencias diariamente tomando entradas y salidas en cada horario
        // de ejecución.
        //--------------------------------------------------------------

        // Método principal que se ejecuta en segundo plano
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            // Bucle infinito que se detiene cuando se solicita la cancelación
            while (!stoppingToken.IsCancellationRequested)
            {
                var ahora = DateTime.Now;
               
                var horaActual = ahora.ToString("HH:mm");

                // Horarios de ejecución automática
                if (horaActual == "12:20" || horaActual == "17:20" || horaActual == "22:40")
                {
                    using var scope = _serviceProvider.CreateScope();
                    
                    var service = scope.ServiceProvider.GetRequiredService<Proceso_Generar_Inconsistencias_MarcasService>();

                    try
                    {
                        await service.PROC1_Async(DateTime.Today, DateTime.Today);

                        // Mensajito para confirmar ejecución
                        Console.WriteLine($"Proceso de inconsistencias ejecutado automáticamente a las {horaActual}");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error en ejecución automática: {ex.Message}");
                    }

                    // Esperar 1 minuto para evitar doble ejecución en el mismo horario
                    await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
                }

                // Revisar cada 30 segundos
                await Task.Delay(TimeSpan.FromSeconds(30), stoppingToken);
            }
        }
    }
}
