using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Application.DTO.DataObjects;

public partial class AdminDO
{
    public AdminDO()
    {
        Uid = Guid.NewGuid();
    }
    public Guid Uid { get; set; }
    public string? Name { get; set; }
    public string? Surname { get; set; }
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public string? Username { get; set; }
    public string? Password { get; set; }
    public bool? IsConfirmed { get; set; }
    public Guid? RoleUid { get; set; }
    public DateTime? AddDate { get; set; }
    public DateTime? UpdateDate { get; set; }
    public bool? IsSuspend { get; set; }
    public string? Title { get; set; }
    [JsonIgnore]
    public string? RefreshToken { get; set; }
    [JsonIgnore]
    public DateTime? RefreshTokenExpiration { get; set; }
    [JsonIgnore]
    public byte[]? PasswordHash { get; set; }
    [JsonIgnore]
    public byte[]? PasswordSalt { get; set; }
    public string? Token { get; set; }
    [JsonIgnore]
    public string? DeviceKey { get; set; }
    public string? ConnectionKeys { get; set; }



    ///////////////////////////// EXTERNAL /////////////////////////////

    public bool CanEdit { get; set; } = true;
    public RoleDO? Role { get; set; }
    public List<PermissionDO>? PermissionList { get; set; }
    public List<ModuleDO> ModuleList { get; set; }

    public override string ToString() => JsonSerializer.Serialize(this, new JsonSerializerOptions
    {
        IgnoreReadOnlyFields = true,
        IgnoreReadOnlyProperties = true,
        Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
    });
}
