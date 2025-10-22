using MySql.Data.MySqlClient;
using Microsoft.Extensions.Configuration;
using System.Data;

namespace EjemploCoreWeb.Repository
{
    public class DbConnectionFactory : IDbConnectionFactory
    {
        private readonly IConfiguration _configuration;
        public DbConnectionFactory(IConfiguration configuration) => _configuration = configuration;

        public IDbConnection CreateConnection()
        {
            // 🔧 FIX URGENTE: usar exactamente la misma cadena que funciona en Login.
            // (Luego, cuando tengas tiempo, volvemos a leerla desde secrets/appsettings).
            var cs = "Server=127.0.0.1;Port=3306;Database=reloj_marcador;" +
                     "Uid=reloj_user;Pwd=RELOJ123;AllowPublicKeyRetrieval=True;SslMode=None;";

            // Limpia pools por si quedaron conexiones fallidas cacheadas
            MySqlConnection.ClearAllPools();

            return new MySqlConnection(cs);
        }
    }
}
