using System.Text.Encodings.Web;
using System.Text.Json;

namespace Application.DTO.DataObjects;
public partial class RoleDO
{
    public RoleDO()
    {
        Uid = Guid.NewGuid();
    }
    public Guid Uid { get; set; } 
    public string? Name { get; set; } 
    public Guid? ParentUid { get; set; } 
    public DateTime? AddDate { get; set; } 
    public DateTime? UpdateDate { get; set; }  
    public string? Slug { get; set; }
    public string? Route { get; set; }
    public int? LoginCount { get; set; }
    public string? Expiration { get; set; }


    ///////////////////////////// EXTERNAL /////////////////////////////
    public List<AdminDO> AdminList{ get; set; }
    public int AdminsCount { get; set; } = 0;
    public int PermissionsCount { get; set; } = 0;
    public List<PermissionDO> PermissionList{ get; set; }
    public List<ModuleDO> ModuleList{ get; set; } 
    public RoleDO ParentRole { get; set; }
    public List<RoleDO> SubRoleList { get; set; }



    public override string ToString() => JsonSerializer.Serialize(this, new JsonSerializerOptions
    {
        IgnoreReadOnlyFields = true,
        IgnoreReadOnlyProperties = true,
        Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
    });
}
