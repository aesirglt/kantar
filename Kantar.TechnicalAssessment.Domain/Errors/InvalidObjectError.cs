namespace Kantar.TechnicalAssessment.Domain.Errors
{
    public record InvalidObjectError : DomainError
    {
        public override int ErrorCode => 409;
        public InvalidObjectError(string msg = "Entity cant be null") : base(msg)
        {
        }
    }
}
