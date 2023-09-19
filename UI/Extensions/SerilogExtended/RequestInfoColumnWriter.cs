using NpgsqlTypes;
using Serilog.Events;
using Serilog.Sinks.PostgreSQL;
namespace UI.Extensions.SerilogExtended;

public class RequestInfoColumnWriter : ColumnWriterBase
{
    public RequestInfoColumnWriter() : base(NpgsqlDbType.Jsonb)
    {

    }

    public override object GetValue(LogEvent logEvent, IFormatProvider formatProvider = null)
    {
        var (key, value) = logEvent.Properties.FirstOrDefault(i => i.Key == "RequestInfo");
        if (value != null && !string.IsNullOrEmpty(value.ToString()) && value.ToString() != "null")
        {
            var val1 = value.ToString().Replace("\\", "")[1..];
            return val1[..(val1.ToString().Length - 1)];
        }
        return null;
    }
}
