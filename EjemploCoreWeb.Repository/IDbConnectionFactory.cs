using System.Data;

namespace EjemploCoreWeb.Repository;

public interface IDbConnectionFactory
{
    IDbConnection CreateConnection();
}
