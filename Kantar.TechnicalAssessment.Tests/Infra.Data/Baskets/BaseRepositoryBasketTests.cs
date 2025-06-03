using Kantar.TechnicalAssessment.Domain.Features;
using Kantar.TechnicalAssessment.Domain.Interfaces.Repositories;
using Kantar.TechnicalAssessment.Infra.Data.Contexts;
using Kantar.TechnicalAssessment.Infra.Data.Repositories;
using Microsoft.Extensions.Logging;
using Moq;

namespace Kantar.TechnicalAssessment.Tests.Infra.Data.Baskets
{
    [TestFixture]
    public class BaseRepositoryBasketTests
    {
        private Mock<GroceryShoppingContext> _dbContextMock;
        private Mock<ILogger<BaseRepository<Basket>>> _loggerMock;
        private IBaseRepository<Basket> _basketRepository;


        [SetUp]
        public void Setup()
        {
            _dbContextMock = new();
            _loggerMock = new();
            _basketRepository = new BaseRepository<Basket>(_dbContextMock.Object, new Logger<BaseRepository<Basket>>(new LoggerFactory()));
        }
    }
}
