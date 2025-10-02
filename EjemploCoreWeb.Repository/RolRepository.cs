using System.Collections.Generic;
using MySql.Data.MySqlClient;
using EjemploCoreWeb.Entities;
using System.Data;

namespace EjemploCoreWeb.Repository
{
    public class RolRepository
    {
        private readonly IDbConnectionFactory _connectionFactory;

        public RolRepository(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public IEnumerable<Rol> GetAll()
        {
            var roles = new List<Rol>();
            using (var connection = _connectionFactory.CreateConnection())
            {
                connection.Open();
                var command = connection.CreateCommand();
                command.CommandText = "SELECT ID_Rol_Usuario, Nombre_Rol FROM Roles";
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        roles.Add(new Rol
                        {
                            ID_Rol_Usuario = Convert.ToInt32(reader["ID_Rol_Usuario"]),
                            Nombre_Rol = reader["Nombre_Rol"].ToString()
                        });
                    }
                }
            }
            return roles;
        }

        public void Insert(Rol rol)
        {
            using (var connection = _connectionFactory.CreateConnection())
            {
                connection.Open();
                var command = connection.CreateCommand();
                command.CommandText = "INSERT INTO Roles (Nombre_Rol) VALUES (@NombreRol)";
                var param = command.CreateParameter();
                param.ParameterName = "@NombreRol";
                param.Value = rol.Nombre_Rol;
                command.Parameters.Add(param);
                command.ExecuteNonQuery();
            }
        }

        public void Update(Rol rol)
        {
            using (var connection = _connectionFactory.CreateConnection())
            {
                connection.Open();
                var command = connection.CreateCommand();
                command.CommandText = "UPDATE Roles SET Nombre_Rol=@NombreRol WHERE ID_Rol_Usuario=@IdRol";

                var paramId = command.CreateParameter();
                paramId.ParameterName = "@IdRol";
                paramId.Value = rol.ID_Rol_Usuario;
                command.Parameters.Add(paramId);

                var paramNombre = command.CreateParameter();
                paramNombre.ParameterName = "@NombreRol";
                paramNombre.Value = rol.Nombre_Rol;
                command.Parameters.Add(paramNombre);

                command.ExecuteNonQuery();
            }
        }

        public void Delete(int idRol)
        {
            using (var connection = _connectionFactory.CreateConnection())
            {
                connection.Open();
                var command = connection.CreateCommand();
                command.CommandText = "DELETE FROM Roles WHERE ID_Rol_Usuario=@IdRol";

                var param = command.CreateParameter();
                param.ParameterName = "@IdRol";
                param.Value = idRol;
                command.Parameters.Add(param);

                command.ExecuteNonQuery();
            }
        }
    }
}