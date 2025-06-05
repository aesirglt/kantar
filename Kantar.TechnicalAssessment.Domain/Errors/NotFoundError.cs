namespace Kantar.TechnicalAssessment.Domain.Errors
{
    public record NotFoundError : DomainError
    {
        public override int ErrorCode => 404;
        public NotFoundError(string msg = "Entity not found") : base(msg)
        {
        }
    }
}
