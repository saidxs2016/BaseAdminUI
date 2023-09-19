using NpgsqlTypes;
using Serilog.Events;
using Serilog.Sinks.PostgreSQL;
namespace UI.Extensions.SerilogExtended;

public class FullNameColumnWriter : ColumnWriterBase
{
    public FullNameColumnWriter() : base(NpgsqlDbType.Text)
    {

    }

    public override object GetValue(LogEvent logEvent, IFormatProvider formatProvider = null)
    {
        var (key, value) = logEvent.Properties.FirstOrDefault(i => i.Key == "FullName");
        if (value != null && !string.IsNullOrEmpty(value.ToString()) && value.ToString() != "null")
            return value.ToString().Replace("\"", "");
        return null;
    }
}
