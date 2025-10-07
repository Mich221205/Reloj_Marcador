using EjemploCoreWeb.Entities;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EjemploCoreWeb.Repository
{
    public class IdentificacionRepository
    {

        private readonly IDbConnectionFactory _connectionFactory;

        public IdentificacionRepository(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public IEnumerable<TipoIdentificacion> GetAll()
        {
            var lista = new List<TipoIdentificacion>();
            using (var connection = _connectionFactory.CreateConnection())
            {
                connection.Open();
                var command = connection.CreateCommand();
                command.CommandText = "SELECT ID_Tipo_Identificacion, Tipo_Identificacion FROM Tipos_Identificacion";
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        lista.Add(new TipoIdentificacion
                        {
                            ID_Tipo_Identificacion = reader.GetInt32(0),
                            Tipo_Identificacion = reader.GetString(1)
                        });
                    }
                }
            }
            return lista;
        }

        public void Insert(TipoIdentificacion tipo)
        {
            using (var connection = _connectionFactory.CreateConnection())
            {
                connection.Open();
                var command = connection.CreateCommand();
                command.CommandText = "INSERT INTO Tipos_Identificacion (Tipo_Identificacion) VALUES (@nombre)";
                var param = command.CreateParameter();
                param.ParameterName = "@nombre";
                param.Value = tipo.Tipo_Identificacion;
                command.Parameters.Add(param);
                command.ExecuteNonQuery();
            }
        }

        public void Update(TipoIdentificacion tipo)
        {
            using (var connection = _connectionFactory.CreateConnection())
            {
                connection.Open();
                var command = connection.CreateCommand();
                command.CommandText = "UPDATE Tipos_Identificacion SET Tipo_Identificacion=@nombre WHERE ID_Tipo_Identificacion=@id";

                var paramNombre = command.CreateParameter();
                paramNombre.ParameterName = "@nombre";
                paramNombre.Value = tipo.Tipo_Identificacion;
                command.Parameters.Add(paramNombre);

                var paramId = command.CreateParameter();
                paramId.ParameterName = "@id";
                paramId.Value = tipo.ID_Tipo_Identificacion;
                command.Parameters.Add(paramId);

                command.ExecuteNonQuery();
            }
        }

        public void Delete(int idTipo)
        {
            using (var connection = _connectionFactory.CreateConnection())
            {
                connection.Open();
                var command = connection.CreateCommand();
                command.CommandText = "DELETE FROM Tipos_Identificacion WHERE ID_Tipo_Identificacion=@id";

                var param = command.CreateParameter();
                param.ParameterName = "@id";
                param.Value = idTipo;
                command.Parameters.Add(param);

                try
                {
                    command.ExecuteNonQuery();
                }
                catch (MySqlException ex)
                {
                    if (ex.Number == 1451) // FK error
                        throw new InvalidOperationException("No se puede eliminar un registro con datos relacionados.");
                    else
                        throw;
                }
            }
        }



    }
}
