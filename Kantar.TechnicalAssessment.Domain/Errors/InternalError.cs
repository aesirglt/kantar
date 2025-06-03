namespace Kantar.TechnicalAssessment.Domain.Errors
{
    public record InternalError(string Msg = "Internal server error") : DomainError(Msg)
    {
        public override int ErrorCode => 500;
    }
}
