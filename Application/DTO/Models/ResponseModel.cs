using Application.DTO.DataObjects;
using Core.DTO.Models;

namespace Application.DTO.Models;

public partial class ResponseModel : BaseResponseModel
{
    public AdminDO? Admin { get; set; }
    public List<AdminDO>? AdminList { get; set; }
    public RoleDO? Role { get; set; }
    public List<RoleDO>? RoleList { get; set; }
    public ModuleDO? Module { get; set; }
    public List<ModuleDO>? ModuleList { get; set; }

    public PermissionDO? Permission { get; set; }
    public List<PermissionDO>? PermissionList { get; set; }

    public List<Guid>? CheckedModules { get; set; }
}
