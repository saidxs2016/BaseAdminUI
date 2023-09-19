using Application.DTO.DataObjects;

namespace Application.DTO.Helpers;

public class CacheAuthInfoHelper
{
    public List<PermissionDO> Permissions { get; set; }
    public string RoleRoute  { get; set; }

    //public string Phone { get; set; } // ola da bilir
}
