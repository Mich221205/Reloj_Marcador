using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using MySql.Data.MySqlClient;


namespace EjemploCoreWeb.Repository
{
    
    //ESTABLECER UN CONTRATO 
    public interface IDbConnectionFactory
    {
        IDbConnection CreateConnection();

    }




}
