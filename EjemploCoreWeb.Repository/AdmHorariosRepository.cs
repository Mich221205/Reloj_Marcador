using Dapper;
using EjemploCoreWeb.Entities;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EjemploCoreWeb.Repository
{
    public class AdmHorariosRepository
    {
        private readonly IDbConnectionFactory _dbConnectionFactory;

        public AdmHorariosRepository(IDbConnectionFactory dbConnectionFactory)
        {
            _dbConnectionFactory = dbConnectionFactory;
        }
        
        //Seleccionar horario de usuario por ID
        public async Task<IEnumerable<Horarios>> Obtener_Horario_UsuarioAsync(string identificacion)
        {
            using (var connection = _dbConnectionFactory.CreateConnection())
            {
                var sql = @"
            SELECT 
                h.ID_Horario,
                a.Codigo_Area,
                a.Nombre_Area
            FROM Horario h
            INNER JOIN Usuario u ON h.ID_Usuario = u.ID_Usuario
            INNER JOIN Areas a ON h.ID_Area = a.ID_Area
            WHERE u.Identificacion = @Identificacion;
        ";

                return await connection.QueryAsync<Horarios>(
                    sql, new { Identificacion = identificacion });
            }
        }

        //cargar detalle de horario por Id_horario
        public async Task<IEnumerable<Detalle_Horarios>> Obtener_Detalles_HorarioAsync(int idHorario)
        {
            using (var connection = _dbConnectionFactory.CreateConnection())
            {
                var sql = @"
            SELECT 
                dh.ID_Detalle,
                dh.ID_Horario,
                dh.Dia,
                dh.Hora_Ingreso,
                dh.Minuto_Ingreso,
                dh.Hora_Salida,
                dh.Minuto_Salida
            FROM Detalle_Horarios dh
            WHERE dh.ID_Horario = @ID_Horario;
        ";

                return await connection.QueryAsync<Detalle_Horarios>(sql, new { ID_Horario = idHorario });
            }
        }


        //INSERT Horario
        public async Task<int> InsertHorarioAsync(Horarios horario)
        {
            using (var connection = _dbConnectionFactory.CreateConnection())
            {
                try
                {
                  
                    // Buscar el ID_Usuario usando la Identificación
                    const string getUserSql = "SELECT ID_Usuario FROM Usuario WHERE Identificacion = @Identificacion";
                    
                    var idUsuario = await connection.ExecuteScalarAsync<int?>(getUserSql, new { horario.Identificacion });


                    if (idUsuario == null)
                    {
                        throw new InvalidOperationException("No se encontró un usuario con esa identificación.");
                    }

                    const string insertSql = @"
                        INSERT INTO Horario (ID_Usuario, ID_Area, Codigo_Area)
                        VALUES (@ID_Usuario, @ID_Area, @Codigo_Area)";

                    var parameters = new
                    {
                        ID_Usuario = idUsuario,
                        ID_Area = horario.ID_Area,
                        Codigo_Area = horario.Codigo_Area
                    };
    
                    var result = await connection.ExecuteAsync(insertSql, parameters);


                    // Verificar si realmente se insertó
                    const string verifySql = "SELECT COUNT(*) FROM Horario WHERE ID_Usuario = @ID_Usuario AND ID_Area = @ID_Area";
                    
                    var count = await connection.ExecuteScalarAsync<int>(verifySql, new { ID_Usuario = idUsuario, ID_Area = horario.ID_Area });


                    return result;
                }
                catch (Exception ex)
                {
                    throw;
                }
            }
        }

        //INSERT Detalle_Horario
        public async Task<int> InsertDetalleHorarioAsync(Detalle_Horarios detalle)
        {
            using (var connection = _dbConnectionFactory.CreateConnection())
            {
                var sql = @"INSERT INTO detalle_horarios 
            (ID_Horario, Dia, Hora_Ingreso, Minuto_Ingreso, Hora_Salida, Minuto_Salida)
            VALUES (@ID_Horario, @Dia, @Hora_Ingreso, @Minuto_Ingreso, @Hora_Salida, @Minuto_Salida)";

                try
                {
                    return await connection.ExecuteAsync(sql, detalle);
                }
                catch (MySqlException ex) when (ex.Number == 1062)
                {
                    throw new InvalidOperationException("Ya existe un detalle relacionado");
                }
            }
        }


        //DELETE Horario

        public async Task<int> DeleteHorarioAsync(int IDHorario)
        {
            using var connection = _dbConnectionFactory.CreateConnection();

            // Primero verificamos si hay detalles asociados
            const string checkSql = "SELECT COUNT(*) FROM Detalle_Horarios WHERE ID_Horario = @ID_Horario;";
            int detallesAsociados = await connection.ExecuteScalarAsync<int>(checkSql, new { ID_Horario = IDHorario });

            if (detallesAsociados > 0)
                throw new InvalidOperationException("No se puede eliminar el horario porque tiene detalles asociados.");

            // Si no tiene detalles, eliminar normalmente
            const string sql = "DELETE FROM Horario WHERE ID_Horario = @ID_Horario;";
            return await connection.ExecuteAsync(sql, new { ID_Horario = IDHorario });
        }


        public async Task<int> DeleteDetalleHorarioAsync(int IDDetalle)
        {
            using (var connection = _dbConnectionFactory.CreateConnection())
            {
                var sql = "DELETE FROM Detalle_Horarios WHERE ID_Detalle = @ID_Detalle;";
                return await connection.ExecuteAsync(sql, new { ID_Detalle = IDDetalle });
            }
        }

        //Obtener las áreas en las que trabaja un usuario(por Identificación)
        public async Task<IEnumerable<Horarios>> Obtener_Areas_UsuarioAsync(string identificacion)
        {
            using (var connection = _dbConnectionFactory.CreateConnection())
            {
                const string sql = @"
            SELECT 
                a.ID_Area, 
                a.Nombre_Area, 
                a.Codigo_Area
            FROM Usuario_Area ua
            INNER JOIN Areas a ON ua.ID_Area = a.ID_Area
            INNER JOIN Usuario u ON ua.ID_Usuario = u.ID_Usuario
            WHERE u.Identificacion = @Identificacion;
        ";

                var parameters = new { Identificacion = identificacion };
                return await connection.QueryAsync<Horarios>(sql, parameters);
            }
        }

        public async Task<int> Obtener_IdUsuario_Por_IdentificacionAsync(string identificacion)
        {
            using (var connection = _dbConnectionFactory.CreateConnection())
            {
                const string sql = "SELECT ID_Usuario FROM Usuario WHERE Identificacion = @Identificacion";
                var result = await connection.ExecuteScalarAsync<int?>(sql, new { Identificacion = identificacion });
                return result ?? 0;
            }
        }


    }
}
