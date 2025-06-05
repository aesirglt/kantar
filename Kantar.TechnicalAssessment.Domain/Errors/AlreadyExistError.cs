namespace Kantar.TechnicalAssessment.Domain.Errors
{
    public record AlreadyExistError : DomainError
    {
        public override int ErrorCode => 422;

        public AlreadyExistError(string msg = "Entity already exists") : base(msg)
        {
            
        }
    }
}
