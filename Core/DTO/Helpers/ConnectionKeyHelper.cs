namespace Core.DTO.Helpers;

public class ConnectionKeyHelper
{
    public Guid? Key { get; set; }
    //public int ConnectedCount { get; set; } = 0;
    public List<Guid>? Connected { get; set; }
    public string? Description { get; set; }
    public DateTime? ValidTo { get; set; }
} 
