using Core.DTO.ResultType;

namespace Application.DTO.ResultType;

public class Result<T> : BaseResult<T>
{

    public Result(bool? IsSuccess = false, T? Data = default, string? Message = null,
        PaginateHelper? Paginate = null, long? RecordsTotal = 0, long? RecordsFiltered = 0,
        bool? IsLastPackage = null, ResultTypeEnum? ResultType = ResultTypeEnum.None,
        string? Html = null, string? Redirect = null,
        bool? ResponseLogging = false, params string[]? parameters
         ) : base(IsSuccess, Data, Message, Paginate, RecordsTotal, RecordsFiltered, IsLastPackage, ResultType, Html, Redirect, ResponseLogging, parameters)
    {
    }
}




