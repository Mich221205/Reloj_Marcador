using Dapper;
using EjemploCoreWeb.Entities;
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
                var sql = "SELECT ID_Motivo, Nombre_Motivo FROM MOTIVOS_AUSENCIA WHERE ID_Motivo = @Id;";
                return await connection.QuerySingleOrDefaultAsync<Motivos_Inconsistencias>(sql, new { ID_Motivo = id });
            }
        }

        // INSERT
        public async Task<int> InsertMotivoAsync(Motivos_Inconsistencias motivo)
        {
            using (var connection = _dbConnectionFactory.CreateConnection())
            {
                var sql = "INSERT INTO MOTIVOS_AUSENCIA (Nombre_Motivo) VALUES (@Nombre_Motivo);";
                return await connection.ExecuteAsync(sql, new { Nombre_Motivo = motivo });
            }
        }

        // UPDATE
        public async Task<int> UpdateMotivoAsync(Motivos_Inconsistencias motivo)
        {
            using (var connection = _dbConnectionFactory.CreateConnection())
            {
                var sql = "UPDATE MOTIVOS_AUSENCIA SET Nombre_Motivo = @Nombre WHERE ID_Motivo = @ID_Motivo;";
                return await connection.ExecuteAsync(sql, new { Nombre = motivo });
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
