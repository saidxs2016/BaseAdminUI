using Application.DTO.Models;

namespace UI.Models;

public class GetRequestModel : RequestModel
{
    //public int Id { get; set; }
    public Guid? Uid { get; set; }
    public string? Key { get; set; }
}
