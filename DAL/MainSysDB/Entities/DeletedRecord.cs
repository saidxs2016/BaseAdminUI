using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DAL.MainSysDB.Entities;

[Table("deleted_record")]
public partial class DeletedRecord
{
    [Key]
    [Column("id")]
    public long Id { get; set; }

    [Column("entity")]
    [StringLength(255)]
    public string? Entity { get; set; }

    [Column("date", TypeName = "timestamp(0) without time zone")]
    public DateTime? Date { get; set; }

    [Column("data", TypeName = "jsonb")]
    public string? Data { get; set; }

    /// <summary>
    /// // dbcontext unique uid
    /// </summary>
    [Column("instance_id")]
    public Guid? InstanceId { get; set; }

    [Column("confirmed")]
    public bool? Confirmed { get; set; }
}
