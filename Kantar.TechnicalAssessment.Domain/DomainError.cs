namespace Kantar.TechnicalAssessment.Domain
{
    public abstract record DomainError
    {
        protected DomainError(string msg)
        {
            Message = msg;
        }
        public abstract int ErrorCode { get; }
        public string Message { get; init; }
    }
}
