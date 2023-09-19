using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace DAL.MainDB.Entities;

/// <summary>
/// adminler tablosu
/// </summary>
[Table("admin")]
public partial class Admin
{
    [Key]
    [Column("uid")]
    public Guid Uid { get; set; }

    [Column("name")]
    [StringLength(255)]
    public string? Name { get; set; }

    [Column("surname")]
    [StringLength(255)]
    public string? Surname { get; set; }

    [Column("email")]
    [StringLength(255)]
    public string? Email { get; set; }

    [Column("phone")]
    [StringLength(255)]
    public string? Phone { get; set; }

    [Column("username")]
    [StringLength(255)]
    public string? Username { get; set; }

    [Column("password")]
    [StringLength(255)]
    public string? Password { get; set; }

    /// <summary>
    /// hesab aktifleştirildi mi
    /// </summary>
    [Column("is_confirmed")]
    public bool? IsConfirmed { get; set; }

    [Column("role_uid")]
    public Guid? RoleUid { get; set; }

    [Column("add_date", TypeName = "timestamp(0) without time zone")]
    public DateTime? AddDate { get; set; }

    [Column("update_date", TypeName = "timestamp(0) without time zone")]
    public DateTime? UpdateDate { get; set; }

    /// <summary>
    /// hesap askıya alındı mı
    /// </summary>
    [Column("is_suspend")]
    public bool? IsSuspend { get; set; }

    [Column("title")]
    [StringLength(255)]
    public string? Title { get; set; }

    [Column("refresh_token")]
    [StringLength(255)]
    public string? RefreshToken { get; set; }

    [Column("refresh_token_expiration", TypeName = "timestamp(0) without time zone")]
    public DateTime? RefreshTokenExpiration { get; set; }

    [Column("password_hash")]
    public byte[]? PasswordHash { get; set; }

    [Column("password_salt")]
    public byte[]? PasswordSalt { get; set; }

    [Column("token")]
    public string? Token { get; set; }

    [Column("device_key", TypeName = "jsonb")]
    public string? DeviceKey { get; set; }

    [Column("connection_keys", TypeName = "jsonb")]
    public string? ConnectionKeys { get; set; }
}
