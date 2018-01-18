using Dapper;
using System;
using System.Collections.Generic;
using System.Data;

namespace Known.Data
{
    public class DapperProvider : IDatabaseProvider
    {
        private IDbConnection connection;

        public DapperProvider(IDbConnection connection, string providerName)
        {
            this.connection = connection;
            ProviderName = providerName;
            ConnectionString = connection.ConnectionString;
        }

        public string ProviderName { get; }
        public string ConnectionString { get; }

        public void Execute(Command command)
        {
            try
            {
                var sql = GetCommandText(command);
                var param = GetDynamicParameters(command.Parameters);
                OpenConnection();
                connection.Execute(sql, param);
            }
            catch (Exception ex)
            {
                throw new DatabaseException(new List<Command> { command }, ex.Message, ex);
            }
            finally
            {
                CloseConnection();
            }
        }

        public void Execute(List<Command> commands)
        {
            try
            {
                OpenConnection();
                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        foreach (var command in commands)
                        {
                            var sql = GetCommandText(command);
                            var param = GetDynamicParameters(command.Parameters);
                            connection.Execute(sql, param, transaction);
                        }
                        transaction.Commit();
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        throw new DatabaseException(commands, ex.Message, ex);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new DatabaseException(commands, ex.Message, ex);
            }
            finally
            {
                CloseConnection();
            }
        }

        public object Scalar(Command command)
        {
            try
            {
                var sql = GetCommandText(command);
                var param = GetDynamicParameters(command.Parameters);
                OpenConnection();
                return connection.ExecuteScalar(sql, param);
            }
            catch (Exception ex)
            {
                throw new DatabaseException(new List<Command> { command }, ex.Message, ex);
            }
            finally
            {
                CloseConnection();
            }
        }

        public DataTable Query(Command command)
        {
            try
            {
                var table = new DataTable();
                var sql = GetCommandText(command);
                var param = GetDynamicParameters(command.Parameters);
                OpenConnection();
                using (var reader = connection.ExecuteReader(sql, param))
                {
                    table.Load(reader);
                }
                return table;
            }
            catch (Exception ex)
            {
                throw new DatabaseException(new List<Command> { command }, ex.Message, ex);
            }
            finally
            {
                CloseConnection();
            }
        }

        public void WriteTable(DataTable table)
        {
            try
            {
                OpenConnection();
                var command = CommandCache.GetInsertCommand(table);
                var sql = GetCommandText(command);
                var param = new List<DynamicParameters>();
                foreach (DataRow row in table.Rows)
                {
                    param.Add(GetDynamicParameters(row));
                }
                connection.Execute(sql, param);
            }
            catch (Exception ex)
            {
                throw new DataException(ex.Message, ex);
            }
            finally
            {
                CloseConnection();
            }
        }

        private void OpenConnection()
        {
            if (connection.State != ConnectionState.Open)
            {
                connection.Open();
            }
        }

        private void CloseConnection()
        {
            if (connection.State != ConnectionState.Closed)
            {
                connection.Close();
            }
        }

        private string GetCommandText(Command command)
        {
            if (ProviderName.Contains("Oracle"))
            {
                return command.Text.Replace("@", ":");
            }
            return command.Text;
        }

        private DynamicParameters GetDynamicParameters(Dictionary<string, object> parameters)
        {
            if (parameters == null || parameters.Count == 0)
                return null;

            var dynamicParameters = new DynamicParameters();
            foreach (var item in parameters)
            {
                dynamicParameters.Add(item.Key, item.Value);
            }
            return dynamicParameters;
        }

        private DynamicParameters GetDynamicParameters(DataRow row)
        {
            var dynamicParameters = new DynamicParameters();
            foreach (DataColumn item in row.Table.Columns)
            {
                dynamicParameters.Add(item.ColumnName, row[item]);
            }
            return dynamicParameters;
        }
    }
}
