using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace DAL.MainDB.Entities;

/// <summary>
/// yetkiler tablosu
/// </summary>
[Table("permission")]
public partial class Permission
{
    [Key]
    [Column("uid")]
    public Guid Uid { get; set; }

    [Column("role_uid")]
    public Guid? RoleUid { get; set; }

    [Column("module_uid")]
    public Guid? ModuleUid { get; set; }

    [Column("add_date", TypeName = "timestamp(0) without time zone")]
    public DateTime? AddDate { get; set; }

    [Column("update_date", TypeName = "timestamp(0) without time zone")]
    public DateTime? UpdateDate { get; set; }

    [Column("by_admin")]
    public Guid? ByAdmin { get; set; }

    /// <summary>
    /// [.authorization-key,input[type=&quot;password&quot;], input[name=&quot;City&quot;]]
    /// </summary>
    [Column("ignored_sections", TypeName = "jsonb")]
    public string? IgnoredSections { get; set; }
}
