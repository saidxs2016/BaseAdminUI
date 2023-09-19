namespace Core.DTO.Helpers;

public class AccessTokenHelper
{
    public string? Token { get; set; }
    public DateTime? Expiration { get; set; }

    public string? ReToken { get; set; }
    private DateTime? _reTokenExpiration;
    public DateTime? ReTokenExpiration
    {
        get
        {
            if (_reTokenExpiration.HasValue)
                return _reTokenExpiration.Value;
            return _reTokenExpiration.Value.AddHours(1);
        }
        set
        {
            _reTokenExpiration = value;
        }
    }
}
