using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace DAL.MainDB.Entities;

/// <summary>
/// admin panel modulleri
/// </summary>
[Table("module")]
public partial class Module
{
    [Key]
    [Column("uid")]
    public Guid Uid { get; set; }

    [Column("name")]
    [StringLength(255)]
    public string? Name { get; set; }

    [Column("controller")]
    [StringLength(255)]
    public string? Controller { get; set; }

    [Column("action")]
    [StringLength(255)]
    public string? Action { get; set; }

    [Column("address")]
    [StringLength(255)]
    public string? Address { get; set; }

    [Column("icon")]
    [StringLength(255)]
    public string? Icon { get; set; }

    [Column("parent_uid")]
    public Guid? ParentUid { get; set; }

    [Column("is_menu")]
    public bool? IsMenu { get; set; }

    [Column("add_date", TypeName = "timestamp(0) without time zone")]
    public DateTime? AddDate { get; set; }

    [Column("update_date", TypeName = "timestamp(0) without time zone")]
    public DateTime? UpdateDate { get; set; }

    [Column("order")]
    public int? Order { get; set; }

    /// <summary>
    /// module tipi: genel de 3 tip var: Category, Page, Feature
    /// </summary>
    [Column("type")]
    [StringLength(255)]
    public string? Type { get; set; }
}
