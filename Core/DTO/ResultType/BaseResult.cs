using System.Text.Json.Serialization;


namespace Core.DTO.ResultType;

public partial class BaseResult<T>
{
    public bool? IsSuccess { get; set; }
    public T? Data { get; set; }
    public string? Message { get; set; }

    public PaginateHelper? Paginate { get; set; }
    public long? RecordsTotal { get; set; }
    public long? RecordsFiltered { get; set; }
    public string? Redirect { get; set; }
    public bool? ResponseLogging { get; set; }
    public bool? IsLastPackage { get; set; }

    [JsonIgnore]
    public ResultTypeEnum? ResultType { get; set; }
    public string? Html { get; set; }

    public string[]? MessagesParameters { get; set; }

    public BaseResult(bool? IsSuccess = false, T? Data = default, string? Message = null,
        PaginateHelper? Paginate = null, long? RecordsTotal = 0, long? RecordsFiltered = 0,
        bool? IsLastPackage = null, ResultTypeEnum? ResultType = ResultTypeEnum.None,
        string? Html = null, string? Redirect = null,
        bool? ResponseLogging = false, params string[]? parameters
         )
    {
        this.IsSuccess = IsSuccess ?? false;
        this.Data = Data;
        this.Paginate = Paginate;
        this.RecordsTotal = RecordsTotal ?? 0;
        this.RecordsFiltered = RecordsFiltered ?? 0;
        this.IsLastPackage = IsLastPackage ?? false;
        this.ResultType = ResultType ?? ResultTypeEnum.None;
        this.Html = Html;
        this.Message = parameters != null ? string.Format(Message ?? "", parameters) : Message;
        this.Redirect = Redirect;
        this.ResponseLogging = ResponseLogging;
    }
}




