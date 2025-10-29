using Dapper;
using EjemploCoreWeb.Entities;
using EjemploCoreWeb.Repository.Interfaces;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace EjemploCoreWeb.Repository
{
    public class InconsistenciaRepository : IInconsistenciaRepository
    {
        private readonly IDbConnectionFactory _connectionFactory;

        public InconsistenciaRepository(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<bool> EstaAsignadoAsync(int id)
        {
            using var connection = _connectionFactory.CreateConnection();

            var tablas = await connection.QueryAsync<(string TableName, string ColumnName)>(@"
        SELECT TABLE_NAME, COLUMN_NAME
        FROM INFORMATION_SCHEMA.KEY_COLUMN_USAGE
        WHERE REFERENCED_TABLE_NAME = 'Inconsistencia'
          AND REFERENCED_COLUMN_NAME = 'ID_Inconsistencia'
          AND TABLE_SCHEMA = 'Reloj_Marcador'");

            foreach (var t in tablas)
            {
                string sql = $"SELECT COUNT(*) FROM {t.TableName} WHERE {t.ColumnName} = @Id";
                int count = await connection.ExecuteScalarAsync<int>(sql, new { Id = id });
                if (count > 0) return true;
            }

            return false;
        }

        public async Task<IEnumerable<Inconsistencia>> GetAllAsync(int page, int pageSize)
        {
            using var connection = _connectionFactory.CreateConnection();
            string sql = @"SELECT ID_Inconsistencia, Nombre_Inconsistencia 
                           FROM Inconsistencia 
                           ORDER BY ID_Inconsistencia 
                           LIMIT @PageSize OFFSET @Offset";
            return await connection.QueryAsync<Inconsistencia>(sql, new
            {
                PageSize = pageSize,
                Offset = (page - 1) * pageSize
            });
        }

        public async Task<Inconsistencia?> GetByIdAsync(int id)
        {
            using var connection = _connectionFactory.CreateConnection();
            string sql = "SELECT * FROM Inconsistencia WHERE ID_Inconsistencia = @Id";
            return await connection.QueryFirstOrDefaultAsync<Inconsistencia>(sql, new { Id = id });
        }

        public async Task<int> InsertAsync(Inconsistencia inconsistencia)
        {
            using var connection = _connectionFactory.CreateConnection();
            string sql = @"INSERT INTO Inconsistencia (Nombre_Inconsistencia) 
                           VALUES (@Nombre_Inconsistencia)";
            return await connection.ExecuteAsync(sql, inconsistencia);
        }

        public async Task<int> UpdateAsync(Inconsistencia inconsistencia)
        {
            using var connection = _connectionFactory.CreateConnection();
            string sql = @"UPDATE Inconsistencia 
                           SET Nombre_Inconsistencia = @Nombre_Inconsistencia
                           WHERE ID_Inconsistencia = @ID_Inconsistencia";
            return await connection.ExecuteAsync(sql, inconsistencia);
        }

        public async Task<int> DeleteAsync(int id)
        {
            using var connection = _connectionFactory.CreateConnection();
            string sql = "DELETE FROM Inconsistencia WHERE ID_Inconsistencia = @Id";
            return await connection.ExecuteAsync(sql, new { Id = id });
        }

        public async Task<int> CountAsync()
        {
            using var connection = _connectionFactory.CreateConnection();
            string sql = "SELECT COUNT(*) FROM Inconsistencia";
            return await connection.ExecuteScalarAsync<int>(sql);
        }



        //Jocsan
        //Reportes de inconsistencias ADM 15

        public async Task<IEnumerable<Reporte_Inconsistencia>> Reporte_Inconsistencias(
            int page,
            int pageSize,
            string? identificacion,
            DateTime? fecha)
        {
            using var connection = _connectionFactory.CreateConnection();

            var result = await connection.QueryAsync<Reporte_Inconsistencia>(
                "sp_reporte_inconsistencias_listar",
                new
                {
                    p_page = page,
                    p_pageSize = pageSize,
                    p_identificacion = identificacion,
                    p_fecha = fecha
                },
                commandType: CommandType.StoredProcedure
            );

            return result;
        }

        public async Task<int> Contar_Reporte(string? identificacion, DateTime? fecha)
        {
            using var connection = _connectionFactory.CreateConnection();

            return await connection.ExecuteScalarAsync<int>(
                "sp_reporte_inconsistencias_contar",
                new
                {
                    p_identificacion = identificacion,
                    p_fecha = fecha
                },
                commandType: CommandType.StoredProcedure
            );
        }



    }
}