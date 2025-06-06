using Kantar.TechnicalAssessment.Domain;
using Microsoft.FSharp.Core;
using System.Text.Json;

namespace Kantar.TechnicalAssessment.WebApi.Endpoints
{
    public class EndpointBaseExt
    {
        public static IResult AsResult<T>(FSharpResult<T, DomainError> value)
            => value.IsOk ? Results.Ok(value.ResultValue) : ProblemResult(value.ErrorValue);

        public static IResult AsResult(FSharpResult<Unit, DomainError> value)
            => value.IsOk ? Results.Ok(value.ResultValue) : ProblemResult(value.ErrorValue);

        public static IResult ProblemResult(DomainError domainError)
            => Results.Problem(detail: JsonSerializer.Serialize(domainError), statusCode: domainError.ErrorCode);
    }
}
