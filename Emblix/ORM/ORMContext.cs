using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace ORM
{
    public class ORMContext<T> where T : class, new()
    {
        private readonly IDbConnection _dbConnection;

        public ORMContext(IDbConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }

        public void Create(T entity, IDbTransaction transaction = null)
        {
            try
            {
                var properties = entity.GetType().GetProperties()
                    .Where(p => p.Name != "Id" &&
                               !p.PropertyType.IsGenericType &&
                               p.PropertyType.Namespace != "System.Collections.Generic")
                    .ToList();

                var columns = string.Join(", ", properties.Select(p => p.Name));
                var values = string.Join(", ", properties.Select(p => "@" + p.Name));

                var query = $"INSERT INTO {typeof(T).Name}s ({columns}) VALUES ({values}); SELECT SCOPE_IDENTITY();";

                using (var command = _dbConnection.CreateCommand())
                {
                    if (transaction != null)
                    {
                        command.Transaction = transaction;
                    }

                    command.CommandText = query;

                    foreach (var property in properties)
                    {
                        var parameter = command.CreateParameter();
                        parameter.ParameterName = "@" + property.Name;
                        parameter.Value = property.GetValue(entity) ?? DBNull.Value;
                        command.Parameters.Add(parameter);
                    }

                    if (transaction == null)
                    {
                        EnsureConnectionOpen();
                    }

                    var result = command.ExecuteScalar();
                    if (result != null && result != DBNull.Value)
                    {
                        var idProperty = typeof(T).GetProperty("Id");
                        if (idProperty != null)
                        {
                            idProperty.SetValue(entity, Convert.ToInt32(result));
                        }
                    }

                    if (transaction == null)
                    {
                        EnsureConnectionClosed();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error creating entity {typeof(T).Name}: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
                throw;
            }
        }

        public T ReadById(int id)
        {
            try
            {
                if (id <= 0)
                {
                    Console.WriteLine($"Invalid ID: {id}");
                    return null;
                }

                var tableName = typeof(T).Name + "s";
                var sql = $"SELECT * FROM {tableName} WHERE Id = @id";

                using (var command = _dbConnection.CreateCommand())
                {
                    command.CommandText = sql;

                    var parameter = command.CreateParameter();
                    parameter.ParameterName = "@id";
                    parameter.Value = id;
                    command.Parameters.Add(parameter);

                    EnsureConnectionOpen();

                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return Map(reader);
                        }
                    }

                    EnsureConnectionClosed();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error reading {typeof(T).Name} with ID {id}: {ex.Message}");
            }

            return null;
        }

        public IEnumerable<T> ReadByAll(string filter = null)
        {
            var result = new List<T>();
            try
            {
                var tableName = typeof(T).Name + "s";
                var sql = $"SELECT * FROM {tableName}";

                if (!string.IsNullOrEmpty(filter))
                {
                    sql += $" WHERE {filter}";
                }

                using (var command = _dbConnection.CreateCommand())
                {
                    command.CommandText = sql;

                    EnsureConnectionOpen();

                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var entity = Map(reader);
                            if (entity != null)
                            {
                                result.Add(entity);
                            }
                        }
                    }

                    EnsureConnectionClosed();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error reading all {typeof(T).Name}: {ex.Message}");
            }

            return result;
        }

        public void Update(int id, T entity)
        {
            try
            {
                if (id <= 0)
                {
                    Console.WriteLine($"Invalid ID for update: {id}");
                    return;
                }

                var properties = typeof(T).GetProperties()
                    .Where(p => p.Name != "Id")
                    .ToList();

                var setClause = string.Join(", ", properties.Select(p => $"{p.Name} = @{p.Name}"));
                var query = $"UPDATE {typeof(T).Name}s SET {setClause} WHERE Id = @id";

                using (var command = _dbConnection.CreateCommand())
                {
                    command.CommandText = query;

                    foreach (var property in properties)
                    {
                        var parameter = command.CreateParameter();
                        parameter.ParameterName = "@" + property.Name;
                        parameter.Value = property.GetValue(entity) ?? DBNull.Value;
                        command.Parameters.Add(parameter);
                    }

                    var idParameter = command.CreateParameter();
                    idParameter.ParameterName = "@id";
                    idParameter.Value = id;
                    command.Parameters.Add(idParameter);

                    EnsureConnectionOpen();
                    command.ExecuteNonQuery();
                    EnsureConnectionClosed();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating {typeof(T).Name} with ID {id}: {ex.Message}");
            }
        }

        public void Delete(int id)
        {
            try
            {
                if (id <= 0)
                {
                    Console.WriteLine($"Invalid ID for delete: {id}");
                    return;
                }

                var query = $"DELETE FROM {typeof(T).Name}s WHERE Id = @id";

                using (var command = _dbConnection.CreateCommand())
                {
                    command.CommandText = query;
                    var parameter = command.CreateParameter();
                    parameter.ParameterName = "@id";
                    parameter.Value = id;
                    command.Parameters.Add(parameter);

                    EnsureConnectionOpen();
                    command.ExecuteNonQuery();
                    EnsureConnectionClosed();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error deleting {typeof(T).Name} with ID {id}: {ex.Message}");
            }
        }

        private T Map(IDataReader reader)
        {
            try
            {
                var entity = new T();
                var properties = typeof(T).GetProperties();

                foreach (var property in properties)
                {
                    try
                    {
                        var ordinal = reader.GetOrdinal(property.Name);
                        if (!reader.IsDBNull(ordinal))
                        {
                            var value = reader[ordinal];
                            var propertyType = Nullable.GetUnderlyingType(property.PropertyType) ?? property.PropertyType;

                            property.SetValue(entity, Convert.ChangeType(value, propertyType));
                        }
                    }
                    catch (IndexOutOfRangeException)
                    {
                        continue;
                    }
                }

                return entity;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error mapping entity {typeof(T).Name}: {ex.Message}");
                return null;
            }
        }

        private void EnsureConnectionOpen()
        {
            try
            {
                if (_dbConnection.State != ConnectionState.Open)
                {
                    _dbConnection.Open();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error opening database connection: {ex.Message}");
                throw;
            }
        }

        private void EnsureConnectionClosed()
        {
            try
            {
                if (_dbConnection.State != ConnectionState.Closed)
                {
                    _dbConnection.Close();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error closing database connection: {ex.Message}");
            }
        }
    }
}
