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
    public class MotivosRepository
    {

        private readonly IDbConnectionFactory _dbConnectionFactory;

        public MotivosRepository(IDbConnectionFactory dbConnectionFactory)
        {
            _dbConnectionFactory = dbConnectionFactory;
        }


        //MOTIVOS DE INCONSISTENCIAS
        // GET ALL
        public async Task<IEnumerable<Motivos_Inconsistencias>> CargarMotivosAsync()
        {
            using (var connection = _dbConnectionFactory.CreateConnection())
            {
                var sql = "SELECT ID_Motivo, Nombre_Motivo FROM MOTIVOS_AUSENCIA;";
                return await connection.QueryAsync<Motivos_Inconsistencias>(sql);
            }
        }

        // GET BY ID
        public async Task<Motivos_Inconsistencias?> Cargar_Motivo_X_IDAsync(int id)
        {
            using (var connection = _dbConnectionFactory.CreateConnection())
            {
                var sql = "SELECT ID_Motivo, Nombre_Motivo FROM MOTIVOS_AUSENCIA WHERE ID_Motivo = @ID_Motivo;";
                return await connection.QuerySingleOrDefaultAsync<Motivos_Inconsistencias>(sql, new { ID_Motivo = id });
            }
        }

        // INSERT
        public async Task<int> InsertMotivoAsync(Motivos_Inconsistencias motivo)
        {
            using (var connection = _dbConnectionFactory.CreateConnection())
            {
                var sql = "INSERT INTO MOTIVOS_AUSENCIA (Nombre_Motivo) VALUES (@Nombre_Motivo)";
                try
                {
                    return await connection.ExecuteAsync(sql, motivo);
                }
                catch (MySqlException ex) when (ex.Number == 1062) // Duplicado
                {
                    throw new InvalidOperationException("Ya existe un motivo con ese nombre.");
                }
            }
        }

        // UPDATE
        public async Task<int> UpdateMotivoAsync(Motivos_Inconsistencias motivo)
        {
            using (var connection = _dbConnectionFactory.CreateConnection())
            {
                var sql = "UPDATE MOTIVOS_AUSENCIA SET Nombre_Motivo = @Nombre_Motivo WHERE ID_Motivo = @ID_Motivo";
                return await connection.ExecuteAsync(sql, new {ID_Motivo = motivo.ID_Motivo,Nombre_Motivo = motivo.Nombre_Motivo});

            }
        }

        // DELETE
        public async Task<int> DeleteMotivoAsync(int idMotivo)
        {
            using (var connection = _dbConnectionFactory.CreateConnection())
            {
                var sql = "DELETE FROM MOTIVOS_AUSENCIA WHERE ID_Motivo = @ID_Motivo;";
                return await connection.ExecuteAsync(sql, new { ID_Motivo = idMotivo });
            }
        }






    }
}
