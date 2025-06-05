using FluentAssertions;
using Kantar.TechnicalAssessment.ApplicationService.Features.Baskets;
using Kantar.TechnicalAssessment.Domain.Features;
using Kantar.TechnicalAssessment.Domain.Interfaces.Repositories;
using Microsoft.FSharp.Core;
using Moq;

namespace Kantar.TechnicalAssessment.Tests.Services
{
    [TestFixture]
    public class BasketItemServiceTests
    {
        private Mock<IBaseRepository<BasketItem>> _repositoryMock;
        private readonly BasketItemService _service;

        public BasketItemServiceTests()
        {
            _repositoryMock = new();
            _service = new(_repositoryMock.Object);
        }

        [Test]
        public async Task BasketItemServiceTests_CreateAsync_ShouldBeOk()
        {
            // Arrange
            var cancellationToken = CancellationToken.None;
            List<BasketItem> basket = [
                new (){
                    Id = Guid.NewGuid(),
                    CreatedAt = DateTime.UtcNow,
                    BasketId = Guid.NewGuid(),
                    ItemId = Guid.NewGuid(),
                    UnitPrice = 100.0m,
                    Quantity = 1
                }
            ];

            // Act
            var result = await _service.CreateAsync(basket, cancellationToken);

            // Assert
            ResultModule.IsOk(result).Should().BeTrue();
            ResultModule.IsError(result).Should().BeFalse();
            _repositoryMock.Verify(x => x.Add(It.IsAny<List<BasketItem>>(), cancellationToken), Times.Once);
            _repositoryMock.VerifyNoOtherCalls();
        }

        [Test]
        public async Task BasketItemServiceTests_CreateAsync_BasketItemsNull_ShouldBeError()
        {
            // Arrange
            var cancellationToken = CancellationToken.None;
            List<BasketItem> basket = null!;

            // Act
            var result = await _service.CreateAsync(basket, cancellationToken);

            // Assert
            ResultModule.IsOk(result).Should().BeFalse();
            ResultModule.IsError(result).Should().BeTrue();
            _repositoryMock.Verify(x => x.Add(It.IsAny<List<BasketItem>>(), cancellationToken), Times.Never);
            _repositoryMock.VerifyNoOtherCalls();
        }

        [Test]
        public async Task BasketItemServiceTests_CreateAsync_BasketItemsEmpty_ShouldBeError()
        {
            // Arrange
            var cancellationToken = CancellationToken.None;
            List<BasketItem> basket = [];

            // Act
            var result = await _service.CreateAsync(basket, cancellationToken);

            // Assert
            ResultModule.IsOk(result).Should().BeFalse();
            ResultModule.IsError(result).Should().BeTrue();
            _repositoryMock.Verify(x => x.Add(It.IsAny<List<BasketItem>>(), cancellationToken), Times.Never);
            _repositoryMock.VerifyNoOtherCalls();
        }
    }
}
