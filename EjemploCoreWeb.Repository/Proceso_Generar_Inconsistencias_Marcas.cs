using Dapper;
using EjemploCoreWeb.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EjemploCoreWeb.Repository
{
    public class Proceso_Generar_Inconsistencias_Marcas
    {

        private readonly IDbConnectionFactory _dbConnectionFactory;

        public Proceso_Generar_Inconsistencias_Marcas(IDbConnectionFactory dbConnectionFactory)
        {
            _dbConnectionFactory = dbConnectionFactory;
        }

        //--------------------------------------------------------------
        // PROC1 GENERAR INCONSISTENCIAS DE MARCAS (Jocsan Fonseca)
        //--------------------------------------------------------------


        // Obtener parámetros de tolerancia desde la base de datos
        // Reglas mínimas parametrizables: Tolerancia de atraso y salida temprana (En bd está en 10 min)
        public async Task<ParametrosInconsistencia> ObtenerToleranciaAsync()
        {
            using var connection = _dbConnectionFactory.CreateConnection();
            
            connection.Open();


            var sql = @"SELECT Tolerancia_Atraso, Tolerancia_Salida_Temprana
                        FROM parametros_inconsistencias
                        WHERE Activo = '1' LIMIT 1;";
            
            var result = await connection.QueryFirstOrDefaultAsync<ParametrosInconsistencia>(sql);
            
            return result ?? new ParametrosInconsistencia { Tolerancia_Atraso = 10, Tolerancia_Salida_Temprana = 10 }; // Valores por defecto si no se encuentran en la BD
        }


        // Obtener funcionarios según área o su ID de usuario
        public async Task<IEnumerable<Usuario>> ObtenerFuncionariosAsync(int? areaId = null, int? usuarioId = null)
        {
            using var connection = _dbConnectionFactory.CreateConnection();
            
            connection.Open();
            
            var sql = @"
                SELECT 
                    u.ID_Usuario, 
                    u.Identificacion, 
                    u.Nombre, 
                    u.Apellido_1, 
                    u.Apellido_2
                FROM usuario u
                INNER JOIN usuario_area ua ON u.ID_Usuario = ua.ID_Usuario
                WHERE (@AreaId IS NULL OR ua.ID_Area = @AreaId)
                        AND (@UsuarioId IS NULL OR u.ID_Usuario = @UsuarioId);";
            
            return await connection.QueryAsync<Usuario>(sql, new { AreaId = areaId, UsuarioId = usuarioId });
        }


        // Obtener horario detallado para un usuario específico y el día de la semana
        public async Task<Detalle_Horarios?> ObtenerHorarioAsync(int idUsuario, string dia)
        {
            using var connection = _dbConnectionFactory.CreateConnection();
            
            connection.Open();
            
            var sql = @"SELECT dh.* FROM detalle_horarios dh
                        INNER JOIN horario h ON dh.ID_Horario = h.ID_Horario
                        WHERE h.ID_Usuario = @IdUsuario AND dh.Dia = @Dia;";
            
            return await connection.QueryFirstOrDefaultAsync<Detalle_Horarios>(sql, new { IdUsuario = idUsuario, Dia = dia }); // Retorna null si no se encuentra
        }


        // Obtener las marcas de un usuario en una fecha específica
        public async Task<IEnumerable<Marca>> ObtenerMarcasAsync(int idUsuario, DateTime fecha)
        {
            using var connection = _dbConnectionFactory.CreateConnection();
            
            connection.Open();
            
            var sql = @"SELECT * FROM marca 
                        WHERE ID_Usuario = @IdUsuario
                            AND DATE(Fecha_Hora) = @Fecha ORDER BY Fecha_Hora;";
           
            return await connection.QueryAsync<Marca>(sql, new { IdUsuario = idUsuario, Fecha = fecha.Date });
        }


        //EXLUSIONES DE INCONSISTENCIAS
        // Verificar si un día es excluido (asueto, feriado, vacaciones, permisos)
        public async Task<bool> EsDiaExcluidoAsync(int idUsuario, DateTime fecha)
        {
            using var connection = _dbConnectionFactory.CreateConnection();
            
            connection.Open();

            // Asuetos y feriados
            var asueto = await connection.ExecuteScalarAsync<int?>(
                
                "SELECT 1 FROM asuetos_y_feriados WHERE Fecha = @Fecha LIMIT 1;", new { Fecha = fecha });
            
            if (asueto.HasValue) return true;

            // Vacaciones
            var vacaciones = await connection.ExecuteScalarAsync<int?>(
                
                "SELECT 1 FROM vacaciones WHERE ID_Usuario = @IdUsuario AND Estado='Aprobado' AND @Fecha BETWEEN Fecha_Inicio AND Fecha_Fin;",
                
                new { IdUsuario = idUsuario, Fecha = fecha });
            
            if (vacaciones.HasValue) return true;

            // Permisos
            var permiso = await connection.ExecuteScalarAsync<int?>(
                
                "SELECT 1 FROM permisos WHERE ID_Usuario = @IdUsuario AND Estado='Aprobado' AND @Fecha BETWEEN Fecha_Inicio AND Fecha_Fin;",
                
                new { IdUsuario = idUsuario, Fecha = fecha });
            
            if (permiso.HasValue) return true;

            return false;
        }

        // Guardar inconsistencias detectadas en la bd utilizando transacciones por aquello jajaj
        public async Task GuardarInconsistenciasAsync(IEnumerable<InconsistenciaDetectada> inconsistencias, string identificacion)
        {
            using var connection = _dbConnectionFactory.CreateConnection();
            
            connection.Open();

            using var trans = connection.BeginTransaction();

            foreach (var inc in inconsistencias)
            {
                var existe = await connection.ExecuteScalarAsync<int?>(@"
                    
                    SELECT 1 FROM inconsistencias_usuario 
                    WHERE Identificacion = @Identificacion 
                        AND DATE(Fecha_Inconsistencia) = @Fecha 
                            AND ID_Inconsistencia = (SELECT ID_Inconsistencia FROM inconsistencia WHERE Nombre_Inconsistencia = @Tipo);",
                    
                    new { Identificacion = identificacion, Fecha = inc.Fecha, Tipo = inc.Tipo }, trans);

                if (existe == null)
                {
                    await connection.ExecuteAsync(@"
                                    INSERT INTO inconsistencias_usuario 
                                        (Identificacion, ID_Inconsistencia, Fecha_Inconsistencia, Estado, Detalle, Referencia)
                                    SELECT 
                                        @Identificacion, 
                                        ID_Inconsistencia, 
                                        @Fecha, 
                                        'No Justificada',
                                        @Detalle,
                                        @Referencia
                                    FROM inconsistencia 
                                    WHERE Nombre_Inconsistencia = @Tipo;",

                                    new
                                    {
                                        Identificacion = identificacion,
                                        Fecha = inc.Fecha,
                                        Tipo = inc.Tipo,
                                        Detalle = inc.Detalle,
                                        Referencia = inc.Referencia
                                    }, trans);
                }
            }

            trans.Commit();
        }


        // Registrar en la bitácora el resumen de todo el proceso
        public async Task RegistrarBitacoraAsync(DateTime inicio, DateTime fin, int total)
        {
            using var connection = _dbConnectionFactory.CreateConnection();
            
            connection.Open();
            
            var resumen = new
            {
            
                Rango = $"{inicio:yyyy-MM-dd} a {fin:yyyy-MM-dd}",
                
                Fecha_Ejecucion = DateTime.Now,
                
                Total_Inconsistencias = total
            };

            string json = System.Text.Json.JsonSerializer.Serialize(resumen);
            
            await connection.ExecuteAsync(@"
                INSERT INTO bitacora (Fecha_Registro, ID_Usuario, ID_Accion, Descripcion_Accion)
                VALUES (CURDATE(), 1, 5, @Json);", new { Json = json });
        }

    }
}
