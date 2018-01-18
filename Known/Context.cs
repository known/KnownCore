using Known.Data;
using Known.Log;
using System.Collections.Generic;

namespace Known
{
    public class Context
    {
        public Context(Database database, ILogger logger)
        {
            Database = database;
            Logger = logger;
            Params = new Dictionary<string, object>();
        }

        public Database Database { get; }
        public ILogger Logger { get; }
        public IDictionary<string, object> Params { get; }
    }
}
