using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace DAL.MainDB.Entities;

/// <summary>
/// roller tablosu
/// </summary>
[Table("role")]
public partial class Role
{
    [Key]
    [Column("uid")]
    public Guid Uid { get; set; }

    [Column("name")]
    [StringLength(255)]
    public string? Name { get; set; }

    [Column("parent_uid")]
    public Guid? ParentUid { get; set; }

    [Column("add_date", TypeName = "timestamp(0) without time zone")]
    public DateTime? AddDate { get; set; }

    [Column("update_date", TypeName = "timestamp(0) without time zone")]
    public DateTime? UpdateDate { get; set; }

    [Column("slug")]
    [StringLength(255)]
    public string? Slug { get; set; }

    [Column("route")]
    [StringLength(255)]
    public string? Route { get; set; }

    /// <summary>
    /// aynı anda kaç farklı cihazda ourum açabilir
    /// </summary>
    [Column("login_count")]
    public int? LoginCount { get; set; }

    /// <summary>
    /// oturum süresi belirleme örn: 1 saat, 2 gün /// 1 Minute gibi
    /// 
    /// 1 m = 1 dakika
    /// 1 h = 60 dakika
    /// 1 d = 24*60 dakika
    /// 1 y = 365*24*60 dakika
    /// </summary>
    [Column("expiration")]
    [StringLength(255)]
    public string? Expiration { get; set; }
}
