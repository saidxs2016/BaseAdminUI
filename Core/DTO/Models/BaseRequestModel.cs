namespace Core.DTO.Models;

public partial class BaseRequestModel
{
    // =============== genel =============== 
    public virtual long Id { get; set; } = 0;
    public virtual string? SearchValue { get; set; } = null;
    public virtual string? OrderBy { get; set; } = null;
    public virtual string? OrderByDirection { get; set; } = "asc"; // asc | desc
    public virtual int Page { get; set; } = 1;
    public virtual int PageSize { get; set; } = 10; // default == 10 , if take == -1 its meean take all 

    public virtual string? SecretKey { get; set; } = null; //  

    public virtual string? Lang { get; set; } = "tr";
    public string? Slug { get; set; } = "/";

    private int _skip = -1;
    public virtual int Skip
    {
        get => _skip != -1 ? _skip : (Page - 1) * PageSize;
        set { _skip = value; }
    }

}
