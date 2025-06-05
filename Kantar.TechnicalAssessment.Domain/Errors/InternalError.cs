namespace Kantar.TechnicalAssessment.Domain.Errors
{
    public record InternalError : DomainError
    {
        public override int ErrorCode => 500;
        public InternalError(string msg = "Internal server error") : base(msg)
        {
        }
    }
}
