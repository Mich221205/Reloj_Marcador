using Dapper;
using EjemploCoreWeb.Entities;
using EjemploCoreWeb.Repository.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EjemploCoreWeb.Repository
{
    //Jocsan
    //ADM 16 REPORTE DE MARCAS
    public class MarcaRepository : IMarca
    {
        private readonly IDbConnectionFactory _dbConnectionFactory;

        public MarcaRepository(IDbConnectionFactory dbConnectionFactory)
        {
            _dbConnectionFactory = dbConnectionFactory;
        }


        public async Task<IEnumerable<Marca>> Reporte_Marcas(
            int page,
            int pageSize,
            string? identificacion,
            DateTime? fecha)
        {
            using var connection = _dbConnectionFactory.CreateConnection();

            var result = await connection.QueryAsync<Marca>(
                "sp_reporte_marcas_listar",
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

        public async Task<int> Contar_Reporte_Marca(string? identificacion, DateTime? fecha)
        {
            using var connection = _dbConnectionFactory.CreateConnection();

            return await connection.ExecuteScalarAsync<int>(
                "sp_reporte_marcas_contar",
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
