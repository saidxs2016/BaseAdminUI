using System.Text.Encodings.Web;
using System.Text.Json;

namespace Application.DTO.DataObjects;
public partial class PermissionDO
{
    public PermissionDO()
    {
        Uid = Guid.NewGuid();
    }
    public Guid Uid { get; set; } 
    public Guid? RoleUid { get; set; } 
    public Guid? ModuleUid { get; set; } 
    public DateTime? AddDate { get; set; } 
    public DateTime? UpdateDate { get; set; }  
    public Guid? ByAdmin { get; set; }
    public string? IgnoredSections { get; set; }


    ///////////////////////////// EXTERNAL /////////////////////////////
    public RoleDO Role { get; set; }
    public ModuleDO Module{ get; set; }
    public AdminDO Admin { get; set; }


    public override string ToString() => JsonSerializer.Serialize(this, new JsonSerializerOptions
    {
        IgnoreReadOnlyFields = true,
        IgnoreReadOnlyProperties = true,
        Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
    });
}
