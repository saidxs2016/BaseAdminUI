using Application.DTO.DataObjects;
using AutoMapper;
using DAL.MainDB.Entities;

namespace Application.DTO.Mapper;

public partial class GlobalMapper : Profile
{
    public GlobalMapper()
    {  
        CreateMap<AdminDO, Admin>();
        CreateMap<Admin, AdminDO>()
            .ForMember(d => d.Password, m => m.Ignore())
            .ForMember(d => d.PasswordHash, m => m.Ignore())
            .ForMember(d => d.PasswordSalt, m => m.Ignore()) 
            .ForMember(d => d.RefreshToken, m => m.Ignore()) 
            .ForMember(d => d.RefreshTokenExpiration, m => m.Ignore());
        CreateMap<RoleDO, Role>().ReverseMap();
        CreateMap<ModuleDO, Module>().ReverseMap();
        CreateMap<PermissionDO, Permission>().ReverseMap();


    }


}
