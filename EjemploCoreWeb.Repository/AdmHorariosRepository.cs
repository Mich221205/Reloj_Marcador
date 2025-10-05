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
        public async Task<int> InsertHorarioAsync(Horarios Horario)
        {
            using (var connection = _dbConnectionFactory.CreateConnection())
            {
                var sql = @"INSERT INTO Horario 
                    (ID_Usuario, ID_Area, Codigo_Area)
                    VALUES (@ID_Usuario, @ID_Area, @Codigo_Area)";

                try
                {
                    return await connection.ExecuteAsync(sql, Horario);
                }
                catch (MySqlException ex) when (ex.Number == 1062) // Error de clave duplicada
                {
                    throw new InvalidOperationException("Horario ya existente");
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

        // Obtener las áreas en las que trabaja un usuario (por Identificación)
        public async Task<IEnumerable<Horarios>> Obtener_Areas_UsuarioAsync()
        {
            using (var connection = _dbConnectionFactory.CreateConnection())
            {
                const string sql = @"
            SELECT 
                ID_Area, 
                Nombre_Area, 
                Codigo_Area
            FROM Areas;
        ";

                return await connection.QueryAsync<Horarios>(sql);
            }
        }




    }
}
