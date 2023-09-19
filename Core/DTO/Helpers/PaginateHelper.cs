namespace Core.DTO.Helpers;

public record PaginateHelper
{
    public long Current { get; set; }
    public long Prev { get; set; }
    public long Next { get; set; }
    public List<string>? Pages { get; set; }
    public string? PagesStr { get; set; }
}
