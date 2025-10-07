using Dapper;
using EjemploCoreWeb.Services.Abstract;
using EjemploCoreWeb.Repository;
using System.Text.Json;
using System.Threading.Tasks;

namespace EjemploCoreWeb.Services
{
    public class BitacoraService : IBitacoraService
    {
        private readonly IDbConnectionFactory _connectionFactory;

        public BitacoraService(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task Registrar(int idUsuario, int idAccion, object detalle, string nombreAccion)
        {
            using var connection = _connectionFactory.CreateConnection();

            string sql = @"INSERT INTO Bitacora 
                           (Fecha_Registro, ID_Usuario, ID_Accion, Descripcion_Accion) 
                           VALUES (CURDATE(), @ID_Usuario, @ID_Accion, @Descripcion)";

            // Creamos JSON con acción + detalle
            var wrapper = new
            {
                Accion = nombreAccion,
                Detalle = detalle
            };

            var jsonDetalle = JsonSerializer.Serialize(wrapper);

            await connection.ExecuteAsync(sql, new
            {
                ID_Usuario = idUsuario,
                ID_Accion = idAccion,
                Descripcion = jsonDetalle
            });
        }
    }
}

