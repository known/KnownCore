using Known.Extensions;
using Known.Mapping;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Known.Data
{
    public class Database : IDisposable
    {
        private IDatabaseProvider provider;
        private List<Command> commands = new List<Command>();

        public Database(IDatabaseProvider provider)
        {
            this.provider = provider ?? throw new ArgumentNullException("provider");
            ConnectionString = provider.ConnectionString;
        }

        public string ConnectionString { get; }

        public void Execute(string sql, object param = null)
        {
            var command = CommandCache.GetCommand(sql, param);
            commands.Add(command);
        }

        public T Scalar<T>(string sql, object param = null)
        {
            var command = CommandCache.GetCommand(sql, param);
            return (T)provider.Scalar(command);
        }

        public T Query<T>(string sql, object param = null) where T : EntityBase
        {
            var command = CommandCache.GetCommand(sql, param);
            var data = provider.Query(command);
            if (data == null || data.Rows.Count == 0)
                return default(T);

            return GetEntity<T>(data.Rows[0]);
        }

        public List<T> QueryList<T>(string sql, object param = null) where T : EntityBase
        {
            var command = CommandCache.GetCommand(sql, param);
            var data = provider.Query(command);
            if (data == null || data.Rows.Count == 0)
                return null;

            var lists = new List<T>();
            foreach (DataRow row in data.Rows)
            {
                lists.Add(GetEntity<T>(row));
            }
            return lists;
        }

        public void Save<T>(T entity) where T : EntityBase
        {
            var command = CommandCache.GetSaveCommand(entity);
            commands.Add(command);
        }

        public void Save<T>(List<T> entities) where T : EntityBase
        {
            foreach (var entity in entities)
            {
                Save(entity);
            }
        }

        public void Delete<T>(T entity) where T : EntityBase
        {
            var command = CommandCache.GetDeleteCommand(entity);
            commands.Add(command);
        }

        public void Delete<T>(List<T> entities) where T : EntityBase
        {
            foreach (var entity in entities)
            {
                Delete(entity);
            }
        }

        public void SubmitChanges()
        {
            if (commands.Count > 1)
            {
                provider.Execute(commands);
            }
            else if (commands.Count > 0)
            {
                provider.Execute(commands[0]);
            }
        }

        public void WriteTable(DataTable table)
        {
            provider.WriteTable(table);
        }

        public void Dispose()
        {
            commands.Clear();
        }

        private static T GetEntity<T>(DataRow row) where T : EntityBase
        {
            if (row == null)
                return default(T);

            var entity = Activator.CreateInstance<T>();
            var properties = typeof(T).GetColumnProperties();
            foreach (var property in properties)
            {
                var fieldName = ColumnInfo.GetColumnName(property);
                if (row.Table.Columns.Contains(fieldName))
                {
                    var value = GetPropertyValue(property.PropertyType, row[fieldName]);
                    property.SetValue(entity, value, null);
                }
            }
            entity.IsNew = false;
            return entity;
        }

        private static object GetPropertyValue(Type type, object value)
        {
            if (type.IsSubclassOf(typeof(EntityBase)))
            {
                var entity = Activator.CreateInstance(type);
                return entity;
            }

            return value;
        }
    }
}
