using NpgsqlTypes;
using Serilog.Events;
using Serilog.Sinks.PostgreSQL;
using System.Xml.Linq;

namespace UI.Extensions.SerilogExtended;

public class AdminUidColumnWriter : ColumnWriterBase
{
    public AdminUidColumnWriter() : base(NpgsqlDbType.Uuid)
    {

    }

    public override object GetValue(LogEvent logEvent, IFormatProvider formatProvider = null)
    {
        var (key, value) = logEvent.Properties.FirstOrDefault(i => i.Key == "AdminUid");

        if (value != null && !string.IsNullOrEmpty(value.ToString()) && value.ToString() != "\"\"" && value.ToString() != "null")
            return Guid.Parse(value.ToString().Replace("\"", ""));

        return null;
    }
}
