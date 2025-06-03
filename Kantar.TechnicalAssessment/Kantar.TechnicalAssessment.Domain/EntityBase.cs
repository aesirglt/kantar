namespace Kantar.TechnicalAssessment.Domain
{

    public record EntityBase<T> where T : EntityBase<T>
    {
        public Guid Id { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
