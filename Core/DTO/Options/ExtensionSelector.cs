namespace Core.DTO.Options;

public record ExtensionSelector
{
    public List<string>? Valid { get; set; }
    public List<string>? InValid { get; set; }
}
