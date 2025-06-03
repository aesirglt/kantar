namespace Kantar.TechnicalAssessment.Domain.Errors
{
    public record AlreadyExistError(string Msg = "Entity already exists") : DomainError(Msg)
    {
        public override int ErrorCode => 422;
    }
}
