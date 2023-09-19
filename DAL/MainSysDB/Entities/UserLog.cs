using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace DAL.MainSysDB.Entities;

[Keyless]
[Table("user_log")]
public partial class UserLog
{
    [Column("admin_uid")]
    public Guid? AdminUid { get; set; }

    [Column("exception")]
    public string? Exception { get; set; }

    [Column("full_name")]
    public string? FullName { get; set; }

    [Column("log_level")]
    [StringLength(255)]
    public string? LogLevel { get; set; }

    [Column("message")]
    public string? Message { get; set; }

    [Column("path")]
    public string? Path { get; set; }

    [Column("raise_date", TypeName = "timestamp without time zone")]
    public DateTime? RaiseDate { get; set; }

    [Column("remote_ip")]
    [StringLength(50)]
    public string? RemoteIp { get; set; }

    [Column("request_info", TypeName = "jsonb")]
    public string? RequestInfo { get; set; }

    [Column("request_interval")]
    [StringLength(255)]
    public string? RequestInterval { get; set; }

    [Column("role_name")]
    public string? RoleName { get; set; }

    [Column("status_code")]
    public int? StatusCode { get; set; }

    [Column("user_log")]
    public bool? UserLog1 { get; set; }
}
