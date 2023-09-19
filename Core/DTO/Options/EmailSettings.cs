namespace Core.DTO.Options;


public class EmailSettings
{
    public string? Host { get; set; }
    public int Port { get; set; }
    public string? Email { get; set; }
    public string? Password { get; set; }
    public string? DisplayName { get; set; }
    public string? Subject { get; set; }
}
