namespace Kantar.TechnicalAssessment.Domain.Errors
{
    public record NotFoundError(string Msg = "Entity not found") : DomainError(Msg)
    {
        public override int ErrorCode => 404;
    }
}
