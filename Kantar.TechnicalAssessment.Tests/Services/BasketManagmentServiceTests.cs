using FluentAssertions;
using Kantar.TechnicalAssessment.ApplicationService.Features.Baskets;
using Kantar.TechnicalAssessment.ApplicationService.Features.Managements;
using Kantar.TechnicalAssessment.ApplicationService.Features.Managements.Commands;
using Kantar.TechnicalAssessment.ApplicationService.Interfaces;
using Kantar.TechnicalAssessment.Domain;
using Kantar.TechnicalAssessment.Domain.Enums;
using Kantar.TechnicalAssessment.Domain.Errors;
using Kantar.TechnicalAssessment.Domain.Features;
using Kantar.TechnicalAssessment.Domain.Interfaces.DomainServices;
using Kantar.TechnicalAssessment.Domain.Interfaces.Services;
using Kantar.TechnicalAssessment.Infra.Data.EntityConfigurations;
using Microsoft.FSharp.Core;
using Moq;

namespace Kantar.TechnicalAssessment.Tests.Services
{
    [TestFixture]
    public class BasketManagmentServiceTests
    {
        private IBasketManagmentService _basketManagmentService;
        private Mock<IBasketItemService> _basketItemServiceMock;
        private Mock<IBasketService> _basketServiceMock;
        private Mock<IItemService> _itemServiceMoc;
        private Mock<IDiscountService> _discountServiceMock;
        private Mock<IApplyDiscountDomainService> _applyDiscountDomainService;

        [SetUp]
        public void Setup()
        {
            _basketItemServiceMock = new();
            _basketServiceMock = new();
            _itemServiceMoc = new();
            _discountServiceMock = new();
            _applyDiscountDomainService = new();
            _basketManagmentService = new BasketManagmentService(_basketServiceMock.Object,
                _itemServiceMoc.Object,
                _basketItemServiceMock.Object,
                _discountServiceMock.Object,
                _applyDiscountDomainService.Object);
        }

        [Test]
        public async Task AddAsync_ShouldReturnOk_WhenBasketAndItemsAreValid()
        {
            // Arrange
            var cancellationToken = CancellationToken.None;
            var itemId = BasicValue.AppleId;
            const string itemName = "Apple";
            var command = new CreateBasketCommand
            {
                BasketId = Guid.NewGuid(),
                Items =
                [
                    new() { ItemId = itemId, Quantity = 10 }
                ]
            };

            var basked = new Basket
            {
                Id = command.BasketId,
                CreatedAt = DateTime.UtcNow,
                BasketItems = [],
            };

            _basketServiceMock.Setup(x => x.GetAsync(command.BasketId, cancellationToken))
                .ReturnsAsync(FSharpResult<Basket, DomainError>.NewOk(basked));

            _basketItemServiceMock.Setup(x => x.CreateAsync(It.IsAny<List<BasketItem>>(), cancellationToken))
                .ReturnsAsync(FSharpResult<Unit, DomainError>.NewOk(default!));
            _basketItemServiceMock.Setup(x => x.UpdateAsync(It.IsAny<List<BasketItem>>(), cancellationToken))
                .ReturnsAsync(FSharpResult<Unit, DomainError>.NewOk(default!));

            _itemServiceMoc.Setup(Setup => Setup.GetAllAsync(It.IsAny<List<Guid>>(), cancellationToken))
                .ReturnsAsync(FSharpResult<IQueryable<Item>, DomainError>.NewOk(new List<Item>
                {
                    new ()
                    {
                        Id = itemId,
                        Name = itemName,
                        Price = 0.30m
                    }
                }.AsQueryable()));

            List<Discount> discounts = [
                new ()
                {
                    Id = Guid.NewGuid(),
                    Name = "10% off Apple",
                    ItemId = itemId,
                    DiscountType = DiscountType.Percentage,
                    Value = 0.10m,
                    CreatedAt = DateTime.UtcNow
                }
            ];
            _discountServiceMock.Setup(x => x.GetByItemIdsAsync(It.IsAny<List<Guid>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(FSharpResult<IQueryable<Discount>, DomainError>.NewOk(discounts.AsQueryable()));

            _applyDiscountDomainService.Setup(x => x.ApplyDiscounts(basked.BasketItems, It.IsAny<List<Discount>>()))
                .Returns((IEnumerable<BasketItem> basketItems, List<Discount> discounts) => basketItems);

            _basketItemServiceMock.Setup(x => x.CreateAsync(basked.BasketItems, cancellationToken))
                .ReturnsAsync(FSharpResult<Unit, DomainError>.NewOk(default!));

            _basketItemServiceMock.Setup(x => x.CreateAsync(basked.BasketItems, cancellationToken))
                .ReturnsAsync(FSharpResult<Unit, DomainError>.NewOk(default!));

            // Act
            var result = await _basketManagmentService.AddAsync(command, CancellationToken.None);

            // Assert
            result.IsOk.Should().BeTrue();
            _discountServiceMock.Verify(x => x.GetByItemIdsAsync(It.IsAny<List<Guid>>(), It.IsAny<CancellationToken>()));
            _discountServiceMock.VerifyNoOtherCalls();
            _basketItemServiceMock.Verify(x => x.CreateAsync(It.IsAny<List<BasketItem>>(), cancellationToken));
            _basketItemServiceMock.Verify(x => x.UpdateAsync(It.IsAny<List<BasketItem>>(), cancellationToken));
            _basketItemServiceMock.VerifyNoOtherCalls();
            _applyDiscountDomainService.Verify(x => x.ApplyDiscounts(It.IsAny<List<BasketItem>>(), It.IsAny<List<Discount>>()));
            _applyDiscountDomainService.VerifyNoOtherCalls();
        }

        [Test]
        public async Task AddAsync_ShouldReturnOk_WhenBasketAlreadyHaveTheItem()
        {
            // Arrange
            var cancellationToken = CancellationToken.None;
            var itemId = BasicValue.AppleId;
            const string itemName = "Apple";
            var command = new CreateBasketCommand
            {
                BasketId = Guid.NewGuid(),
                Items =
                [
                    new() { ItemId = itemId, Quantity = 10 }
                ]
            };

            var basked = new Basket
            {
                Id = command.BasketId,
                CreatedAt = DateTime.UtcNow,
                BasketItems = [ new() {
                    ItemId = itemId,
                    Quantity = 10,
                    UnitPrice = 0.30m,
                    BasketId = command.BasketId,
                }],
            };

            _basketServiceMock.Setup(x => x.GetAsync(command.BasketId, cancellationToken))
                .ReturnsAsync(FSharpResult<Basket, DomainError>.NewOk(basked));

            _basketItemServiceMock.Setup(x => x.CreateAsync(It.IsAny<List<BasketItem>>(), cancellationToken))
                .ReturnsAsync(FSharpResult<Unit, DomainError>.NewOk(default!));

            _itemServiceMoc.Setup(Setup => Setup.GetAllAsync(It.IsAny<List<Guid>>(), cancellationToken))
                .ReturnsAsync(FSharpResult<IQueryable<Item>, DomainError>.NewOk(new List<Item>
                {
                    new ()
                    {
                        Id = itemId,
                        Name = itemName,
                        Price = 0.30m
                    }
                }.AsQueryable()));

            List<Discount> discounts = [
                new ()
                {
                    Id = Guid.NewGuid(),
                    Name = "10% off Apple",
                    ItemId = itemId,
                    DiscountType = DiscountType.Percentage,
                    Value = 0.10m,
                    CreatedAt = DateTime.UtcNow
                }
            ];
            List<Guid> itemIds = [itemId];

            _discountServiceMock.Setup(x => x.GetByItemIdsAsync(It.IsAny<List<Guid>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(FSharpResult<IQueryable<Discount>, DomainError>.NewOk(discounts.AsQueryable()));

            _applyDiscountDomainService.Setup(x => x.ApplyDiscounts(basked.BasketItems, It.IsAny<List<Discount>>()))
                .Returns((IEnumerable<BasketItem> basketItems, List<Discount> discounts) => basketItems);

            _basketItemServiceMock.Setup(x => x.CreateAsync(basked.BasketItems, cancellationToken))
                .ReturnsAsync(FSharpResult<Unit, DomainError>.NewOk(default!));

            // Act
            var result = await _basketManagmentService.AddAsync(command, CancellationToken.None);

            // Assert
            result.IsOk.Should().BeTrue();
            _discountServiceMock.VerifyNoOtherCalls();
            _basketItemServiceMock.VerifyNoOtherCalls();
            _applyDiscountDomainService.VerifyNoOtherCalls();
        }
        [Test]
        public async Task AddAsync_ShouldReturnError_WhenCommandIsNull()
        {
            // Act
            var result = await _basketManagmentService.AddAsync(null!, CancellationToken.None);

            // Assert
            result.IsError.Should().BeTrue();
            result.ErrorValue.Should().BeOfType<InvalidObjectError>();
        }

        [Test]
        public async Task AddAsync_ShouldReturnError_WhenItemServiceFails()
        {
            var command = new CreateBasketCommand
            {
                BasketId = Guid.NewGuid(),
                Items = [new() { ItemId = Guid.NewGuid(), Quantity = 1 }]
            };

            _itemServiceMoc.Setup(x => x.GetAllAsync(It.IsAny<List<Guid>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(FSharpResult<IQueryable<Item>, DomainError>.NewError(new InternalError("Item retrieval failed")));

            var result = await _basketManagmentService.AddAsync(command, CancellationToken.None);

            result.IsError.Should().BeTrue();
            result.ErrorValue.Should().BeOfType<InternalError>();
        }
        [Test]
        public async Task AddAsync_ShouldReturnError_WhenCreatingNewBasketFails()
        {
            var command = new CreateBasketCommand
            {
                BasketId = Guid.NewGuid(),
                Items = [new() { ItemId = BasicValue.AppleId, Quantity = 1 }]
            };

            var notFound = FSharpResult<Basket, DomainError>.NewError(new NotFoundError("Basket not found"));
            var createFailed = FSharpResult<Basket, DomainError>.NewError(new InternalError("Create failed"));

            _itemServiceMoc.Setup(x => x.GetAllAsync(It.IsAny<List<Guid>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(FSharpResult<IQueryable<Item>, DomainError>.NewOk(new List<Item>
                {
            new () { Id = BasicValue.AppleId, Name = "Apple", Price = 0.30m }
                }.AsQueryable()));

            _basketServiceMock.Setup(x => x.GetAsync(command.BasketId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(notFound);

            _basketServiceMock.Setup(x => x.CreateAsync(It.IsAny<Basket>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(createFailed);

            var result = await _basketManagmentService.AddAsync(command, CancellationToken.None);

            result.IsError.Should().BeTrue();
            result.ErrorValue.Should().BeOfType<InternalError>();
        }

        [Test]
        public async Task AddAsync_ShouldReturnError_WhenBasketItemCreationFails()
        {
            var command = new CreateBasketCommand
            {
                BasketId = Guid.NewGuid(),
                Items = [new() { ItemId = BasicValue.AppleId, Quantity = 1 }]
            };

            var basket = new Basket
            {
                Id = command.BasketId,
                CreatedAt = DateTime.UtcNow,
                BasketItems = []
            };

            _itemServiceMoc.Setup(x => x.GetAllAsync(It.IsAny<List<Guid>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(FSharpResult<IQueryable<Item>, DomainError>.NewOk(new List<Item>
                {
            new () { Id = BasicValue.AppleId, Name = "Apple", Price = 0.30m }
                }.AsQueryable()));

            _basketServiceMock.Setup(x => x.GetAsync(command.BasketId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(FSharpResult<Basket, DomainError>.NewOk(basket));

            _discountServiceMock.Setup(x => x.GetByItemIdsAsync(It.IsAny<List<Guid>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(FSharpResult<IQueryable<Discount>, DomainError>.NewOk(Enumerable.Empty<Discount>().AsQueryable()));

            _applyDiscountDomainService.Setup(x => x.ApplyDiscounts(It.IsAny<IEnumerable<BasketItem>>(), It.IsAny<List<Discount>>()))
                .Returns((IEnumerable<BasketItem> b, List<Discount> d) => b);

            _basketItemServiceMock.Setup(x => x.CreateAsync(It.IsAny<List<BasketItem>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(FSharpResult<Unit, DomainError>.NewError(new NotFoundError("Basket item failed")));

            var result = await _basketManagmentService.AddAsync(command, CancellationToken.None);

            result.IsError.Should().BeTrue();
            result.ErrorValue.Should().BeOfType<NotFoundError>();
        }
    }
}
