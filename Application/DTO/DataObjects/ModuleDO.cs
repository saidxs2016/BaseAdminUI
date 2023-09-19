using System.Text.Encodings.Web;
using System.Text.Json;

namespace Application.DTO.DataObjects;
public partial class ModuleDO
{
    public ModuleDO()
    {
        Uid = Guid.NewGuid();
    }
    public Guid Uid { get; set; } 
    public string? Name { get; set; } 
    public string? Controller { get; set; } 
    public string? Action { get; set; }
    public string? Address { get; set; }
    public string? Icon { get; set; } 
    public Guid? ParentUid { get; set; } 
    public bool? IsMenu { get; set; } 
    public DateTime? AddDate { get; set; } 
    public DateTime? UpdateDate { get; set; }  
    public int? Order { get; set; }
    public string? Type { get; set; }

    

    ///////////////////////////// EXTERNAL /////////////////////////////
    public bool IsChecked { get; set; } = false;
    public ModuleDO ParentModule { get; set; }
    public List<ModuleDO> SubModuleList { get; set; }

    public PermissionDO Permission { get; set; }

    public string IgnoredSectionAsString { get; set; }
    public List<string> IgnoredSectionList { get; set; }

    public override string ToString() => JsonSerializer.Serialize(this, new JsonSerializerOptions
    {
        IgnoreReadOnlyFields = true,
        IgnoreReadOnlyProperties = true,
        Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
    });
}
