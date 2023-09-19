﻿using NpgsqlTypes;
using Serilog.Events;
using Serilog.Sinks.PostgreSQL;
namespace UI.Extensions.SerilogExtended;

public class RoleNameColumnWriter : ColumnWriterBase
{
    public RoleNameColumnWriter() : base(NpgsqlDbType.Text)
    {

    }

    public override object GetValue(LogEvent logEvent, IFormatProvider formatProvider = null)
    {
        var (key, value) = logEvent.Properties.FirstOrDefault(i => i.Key == "RoleName");
        if (value != null && !string.IsNullOrEmpty(value.ToString()) && value.ToString() != "null")
            return value.ToString().Replace("\"", "");
        return null;
    }
}
