namespace Kantar.TechnicalAssessment.ApplicationService.Features.Managements.Queries
{
    public class GetAllBasketQuery
    {
        public int Skip { get; set; } = 1;
        public int Take { get; set; } = 10;
    }
}
