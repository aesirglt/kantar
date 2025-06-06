using FluentAssertions;
using Kantar.TechnicalAssessment.ApplicationService.Features.Baskets;
using Kantar.TechnicalAssessment.ApplicationService.Features.Managements;
using Kantar.TechnicalAssessment.ApplicationService.Features.Managements.Commands;
using Kantar.TechnicalAssessment.ApplicationService.Features.Managements.Queries;
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
        [Test]
        public async Task GetAll_ShouldReturnBaskets_WhenQueryIsValid()
        {
            // Arrange
            var cancellationToken = CancellationToken.None;
            var query = new GetAllBasketQuery { Skip = 0, Take = 10 };

            var baskets = new List<Basket>
    {
        new() { Id = Guid.NewGuid(), CreatedAt = DateTime.UtcNow, BasketItems = [] },
        new() { Id = Guid.NewGuid(), CreatedAt = DateTime.UtcNow, BasketItems = [] }
    }.AsQueryable();

            _basketServiceMock.Setup(x => x.GetAllAsync(cancellationToken))
                .ReturnsAsync(FSharpResult<IQueryable<Basket>, DomainError>.NewOk(baskets));

            // Act
            var result = await _basketManagmentService.GetAll(query, cancellationToken);

            // Assert
            result.IsOk.Should().BeTrue();
            result.ResultValue.Count().Should().Be(2);
            _basketServiceMock.Verify(x => x.GetAllAsync(cancellationToken), Times.Once);
        }

        [Test]
        public async Task GetAll_ShouldReturnError_WhenServiceFails()
        {
            // Arrange
            var cancellationToken = CancellationToken.None;
            var query = new GetAllBasketQuery { Skip = 0, Take = 10 };

            _basketServiceMock.Setup(x => x.GetAllAsync(cancellationToken))
                .ReturnsAsync(FSharpResult<IQueryable<Basket>, DomainError>.NewError(new InternalError("Database error")));

            // Act
            var result = await _basketManagmentService.GetAll(query, cancellationToken);

            // Assert
            result.IsError.Should().BeTrue();
            result.ErrorValue.Should().BeOfType<InternalError>();
        }
        [Test]
        public async Task GetById_ShouldReturnBasket_WhenBasketExists()
        {
            // Arrange
            var cancellationToken = CancellationToken.None;
            var basketId = Guid.NewGuid();
            var query = new GetByIdBasketQuery { BasketId = basketId };

            var basket = new Basket
            {
                Id = basketId,
                CreatedAt = DateTime.UtcNow,
                BasketItems = new List<BasketItem>
        {
            new() {
                Id = Guid.NewGuid(),
                ItemId = BasicValue.AppleId,
                Quantity = 2,
                UnitPrice = 0.30m,
                Item = new Item { Id = BasicValue.AppleId, Name = "Apple", Price = 0.30m }
            }
        }
            };

            _basketServiceMock.Setup(x => x.GetAsync(basketId, cancellationToken))
                .ReturnsAsync(FSharpResult<Basket, DomainError>.NewOk(basket));

            // Act
            var result = await _basketManagmentService.GetById(query, cancellationToken);

            // Assert
            result.IsOk.Should().BeTrue();
            result.ResultValue.Id.Should().Be(basketId);
            result.ResultValue.Items.Should().HaveCount(1);
            _basketServiceMock.Verify(x => x.GetAsync(basketId, cancellationToken), Times.Once);
        }

        [Test]
        public async Task GetById_ShouldReturnError_WhenBasketNotFound()
        {
            // Arrange
            var cancellationToken = CancellationToken.None;
            var basketId = Guid.NewGuid();
            var query = new GetByIdBasketQuery { BasketId = basketId };

            _basketServiceMock.Setup(x => x.GetAsync(basketId, cancellationToken))
                .ReturnsAsync(FSharpResult<Basket, DomainError>.NewError(new NotFoundError("Basket not found")));

            // Act
            var result = await _basketManagmentService.GetById(query, cancellationToken);

            // Assert
            result.IsError.Should().BeTrue();
            result.ErrorValue.Should().BeOfType<NotFoundError>();
        }
        [Test]
        public async Task RemoveAsync_ShouldReturnOk_WhenBasketExists()
        {
            // Arrange
            var cancellationToken = CancellationToken.None;
            var basketId = Guid.NewGuid();
            var command = new DeleteBasketCommand { BasketId = basketId };

            var basket = new Basket
            {
                Id = basketId,
                CreatedAt = DateTime.UtcNow,
                BasketItems = new List<BasketItem>
        {
            new() { Id = Guid.NewGuid(), ItemId = BasicValue.AppleId, Quantity = 2 }
        }
            };

            _basketServiceMock.Setup(x => x.GetAsync(basketId, cancellationToken))
                .ReturnsAsync(FSharpResult<Basket, DomainError>.NewOk(basket));

            _basketItemServiceMock.Setup(x => x.RemoveAsync(It.IsAny<Guid>(), cancellationToken))
                .ReturnsAsync(FSharpResult<Unit, DomainError>.NewOk(default!));

            _basketServiceMock.Setup(x => x.RemoveAsync(It.IsAny<Basket>(), cancellationToken))
                .ReturnsAsync(FSharpResult<Unit, DomainError>.NewOk(default!));

            // Act
            var result = await _basketManagmentService.RemoveAsync(command, cancellationToken);

            // Assert
            result.IsOk.Should().BeTrue();
            _basketServiceMock.Verify(x => x.GetAsync(basketId, cancellationToken), Times.Once);
            _basketItemServiceMock.Verify(x => x.RemoveAsync(It.IsAny<Guid>(), cancellationToken), Times.Once);
            _basketServiceMock.Verify(x => x.RemoveAsync(It.IsAny<Basket>(), cancellationToken), Times.Once);
        }

        [Test]
        public async Task RemoveAsync_ShouldReturnError_WhenBasketNotFound()
        {
            // Arrange
            var cancellationToken = CancellationToken.None;
            var basketId = Guid.NewGuid();
            var command = new DeleteBasketCommand { BasketId = basketId };

            _basketServiceMock.Setup(x => x.GetAsync(basketId, cancellationToken))
                .ReturnsAsync(FSharpResult<Basket, DomainError>.NewError(new NotFoundError("Basket not found")));

            // Act
            var result = await _basketManagmentService.RemoveAsync(command, cancellationToken);

            // Assert
            result.IsError.Should().BeTrue();
            result.ErrorValue.Should().BeOfType<NotFoundError>();
            _basketItemServiceMock.Verify(x => x.RemoveAsync(It.IsAny<Guid>(), cancellationToken), Times.Never);
            _basketServiceMock.Verify(x => x.RemoveAsync(It.IsAny<Basket>(), cancellationToken), Times.Never);
        }
        [Test]
        public async Task RemoveBasketItemAsync_ShouldReturnOk_WhenItemExistsInBasket()
        {
            // Arrange
            var cancellationToken = CancellationToken.None;
            var basketId = Guid.NewGuid();
            var itemId = BasicValue.AppleId;
            var command = new RemoveBasketItemCommand { BasketId = basketId, ItemId = itemId };

            var basket = new Basket
            {
                Id = basketId,
                CreatedAt = DateTime.UtcNow,
                BasketItems = new List<BasketItem>
        {
            new() { Id = Guid.NewGuid(), ItemId = itemId, Quantity = 2 },
            new() { Id = Guid.NewGuid(), ItemId = BasicValue.Cucumber, Quantity = 3 }
        }
            };

            _basketServiceMock.Setup(x => x.GetAsync(basketId, cancellationToken))
                .ReturnsAsync(FSharpResult<Basket, DomainError>.NewOk(basket));

            _basketItemServiceMock.Setup(x => x.RemoveAsync(It.IsAny<Guid>(), cancellationToken))
                .ReturnsAsync(FSharpResult<Unit, DomainError>.NewOk(default!));

            _discountServiceMock.Setup(x => x.GetByItemIdsAsync(It.IsAny<List<Guid>>(), cancellationToken))
                .ReturnsAsync(FSharpResult<IQueryable<Discount>, DomainError>.NewOk(Enumerable.Empty<Discount>().AsQueryable()));

            _basketItemServiceMock.Setup(x => x.CreateAsync(It.IsAny<List<BasketItem>>(), cancellationToken))
                .ReturnsAsync(FSharpResult<Unit, DomainError>.NewOk(default!));

            // Act
            var result = await _basketManagmentService.RemoveBasketItemAsync(command, cancellationToken);

            // Assert
            result.IsOk.Should().BeTrue();
            _basketServiceMock.Verify(x => x.GetAsync(basketId, cancellationToken), Times.Once);
            _basketItemServiceMock.Verify(x => x.RemoveAsync(It.IsAny<Guid>(), cancellationToken), Times.Exactly(2));
            _basketItemServiceMock.Verify(x => x.CreateAsync(It.IsAny<List<BasketItem>>(), cancellationToken), Times.Once);
        }

        [Test]
        public async Task RemoveBasketItemAsync_ShouldReturnError_WhenCommandIsNull()
        {
            // Act
            var result = await _basketManagmentService.RemoveBasketItemAsync(null!, CancellationToken.None);

            // Assert
            result.IsError.Should().BeTrue();
            result.ErrorValue.Should().BeOfType<InvalidObjectError>();
        }

        [Test]
        public async Task RemoveBasketItemAsync_ShouldReturnError_WhenItemNotInBasket()
        {
            // Arrange
            var cancellationToken = CancellationToken.None;
            var basketId = Guid.NewGuid();
            var itemId = Guid.NewGuid();
            var command = new RemoveBasketItemCommand { BasketId = basketId, ItemId = itemId };

            var basket = new Basket
            {
                Id = basketId,
                CreatedAt = DateTime.UtcNow,
                BasketItems = new List<BasketItem>
        {
            new() { Id = Guid.NewGuid(), ItemId = BasicValue.AppleId, Quantity = 2 }
        }
            };

            _basketServiceMock.Setup(x => x.GetAsync(basketId, cancellationToken))
                .ReturnsAsync(FSharpResult<Basket, DomainError>.NewOk(basket));

            // Act
            var result = await _basketManagmentService.RemoveBasketItemAsync(command, cancellationToken);

            // Assert
            result.IsError.Should().BeTrue();
            result.ErrorValue.Should().BeOfType<NotFoundError>();
            _basketItemServiceMock.Verify(x => x.RemoveAsync(It.IsAny<Guid>(), cancellationToken), Times.Never);
        }
        [Test]
        public async Task UpdateItemQuantityAsync_ShouldReturnOk_WhenItemExists()
        {
            // Arrange
            var cancellationToken = CancellationToken.None;
            var basketId = Guid.NewGuid();
            var itemId = BasicValue.AppleId;
            var command = new UpdateBasketItemQuantityCommand
            {
                BasketId = basketId,
                ItemId = itemId,
                Quantity = 5
            };

            var basket = new Basket
            {
                Id = basketId,
                CreatedAt = DateTime.UtcNow,
                BasketItems = new List<BasketItem>
        {
            new() { Id = Guid.NewGuid(), ItemId = itemId, Quantity = 2, UnitPrice = 0.30m }
        }
            };

            _basketServiceMock.Setup(x => x.GetAsync(basketId, cancellationToken))
                .ReturnsAsync(FSharpResult<Basket, DomainError>.NewOk(basket));

            _discountServiceMock.Setup(x => x.GetByItemIdsAsync(It.IsAny<List<Guid>>(), cancellationToken))
                .ReturnsAsync(FSharpResult<IQueryable<Discount>, DomainError>.NewOk(Enumerable.Empty<Discount>().AsQueryable()));

            _basketItemServiceMock.Setup(x => x.UpdateAsync(It.IsAny<List<BasketItem>>(), cancellationToken))
                .ReturnsAsync(FSharpResult<Unit, DomainError>.NewOk(default!));

            // Act
            var result = await _basketManagmentService.UpdateItemQuantityAsync(command, cancellationToken);

            // Assert
            result.IsOk.Should().BeTrue();
            _basketServiceMock.Verify(x => x.GetAsync(basketId, cancellationToken), Times.Once);
            _basketItemServiceMock.Verify(x => x.UpdateAsync(It.Is<List<BasketItem>>(items =>
                items.Count == 1 && items[0].Quantity == 5), cancellationToken), Times.Once);
        }

        [Test]
        public async Task UpdateItemQuantityAsync_ShouldReturnError_WhenItemNotFound()
        {
            // Arrange
            var cancellationToken = CancellationToken.None;
            var basketId = Guid.NewGuid();
            var itemId = Guid.NewGuid();
            var command = new UpdateBasketItemQuantityCommand
            {
                BasketId = basketId,
                ItemId = itemId,
                Quantity = 5
            };

            var basket = new Basket
            {
                Id = basketId,
                CreatedAt = DateTime.UtcNow,
                BasketItems = new List<BasketItem>
        {
            new() { Id = Guid.NewGuid(), ItemId = BasicValue.AppleId, Quantity = 2 }
        }
            };

            _basketServiceMock.Setup(x => x.GetAsync(basketId, cancellationToken))
                .ReturnsAsync(FSharpResult<Basket, DomainError>.NewOk(basket));

            // Act
            var result = await _basketManagmentService.UpdateItemQuantityAsync(command, cancellationToken);

            // Assert
            result.IsError.Should().BeTrue();
            result.ErrorValue.Should().BeOfType<NotFoundError>();
            _basketItemServiceMock.Verify(x => x.UpdateAsync(It.IsAny<List<BasketItem>>(), cancellationToken), Times.Never);
        }

        [Test]
        public async Task UpdateItemQuantityAsync_ShouldReturnError_WhenBasketNotFound()
        {
            // Arrange
            var cancellationToken = CancellationToken.None;
            var basketId = Guid.NewGuid();
            var command = new UpdateBasketItemQuantityCommand
            {
                BasketId = basketId,
                ItemId = BasicValue.AppleId,
                Quantity = 5
            };

            _basketServiceMock.Setup(x => x.GetAsync(basketId, cancellationToken))
                .ReturnsAsync(FSharpResult<Basket, DomainError>.NewError(new NotFoundError("Basket not found")));

            // Act
            var result = await _basketManagmentService.UpdateItemQuantityAsync(command, cancellationToken);

            // Assert
            result.IsError.Should().BeTrue();
            result.ErrorValue.Should().BeOfType<NotFoundError>();
            _basketItemServiceMock.Verify(x => x.UpdateAsync(It.IsAny<List<BasketItem>>(), cancellationToken), Times.Never);
        }
    }
}
