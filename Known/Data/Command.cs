using Known.Extensions;
using System.Collections.Generic;
using System.Text;

namespace Known.Data
{
    public class Command
    {
        public Command(string text)
        {
            Text = text;
            Parameters = new Dictionary<string, object>();
        }

        public string Text { get; }
        public Dictionary<string, object> Parameters { get; }

        public bool HasParameter
        {
            get { return Parameters.Count > 0; }
        }

        public void AddParameter(string name, object value)
        {
            Parameters[name] = value;
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.AppendLine($"Text={Text}");
            if (HasParameter)
            {
                var parameters = Parameters.ToJson();
                sb.AppendLine($"Parameters:{parameters}");
            }
            return sb.ToString();
        }
    }
}
