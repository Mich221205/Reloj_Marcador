using Dapper;
using EjemploCoreWeb.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EjemploCoreWeb.Repository
{
    public class UsuarioRepository
    {

        private readonly IDbConnectionFactory _dbConnectionFactory;

        public UsuarioRepository(IDbConnectionFactory dbConnectionFactory)
        {
            _dbConnectionFactory = dbConnectionFactory;
        }

        public async Task<IEnumerable<Usuario>> GetAllAsync()
        {
            using (var connection = _dbConnectionFactory.CreateConnection())
            {
                return await connection.QueryAsync<Usuario>("SELECT Nombre,Identificacion, Apellido_1 , Apellido_2 FROM Usuario WHERE ID_ROL_USUARIO = 2");
            }
        }
        public async Task<Usuario> Obtener_Usuario_X_Identificacion(string id)
        {
            using (var connection = _dbConnectionFactory.CreateConnection())
            {
                return await connection.QuerySingleOrDefaultAsync<Usuario>("SELECT Identificacion, Nombre, Apellido_1, Apellido_2, Contrasena FROM Usuario WHERE Identificacion = @Identificacion", new { Identificacion = id });
            }
        }


        public async Task<int> Cambiar_Clave(Usuario usuario)
        {
            using (var connection = _dbConnectionFactory.CreateConnection())
            {
                var sql = "UPDATE Usuario SET Contrasena = @Contrasena WHERE Identificacion = @Identificacion";
                
                return await connection.ExecuteAsync(sql, usuario);
            }
        }


        


    }
}
