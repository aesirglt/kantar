namespace Kantar.TechnicalAssessment.Domain.Errors
{
    public record InvalidObjectError(string Msg = "Entity cant be null") : DomainError(Msg)
    {
        public override int ErrorCode => 409;
    }
}
