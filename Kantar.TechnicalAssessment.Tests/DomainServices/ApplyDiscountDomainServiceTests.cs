using FluentAssertions;
using Kantar.TechnicalAssessment.Domain.Enums;
using Kantar.TechnicalAssessment.Domain.Features;
using Kantar.TechnicalAssessment.DomainServices;
using Kantar.TechnicalAssessment.Infra.Data.EntityConfigurations;
using Microsoft.FSharp.Core;
namespace Kantar.TechnicalAssessment.Tests.DomainServices
{
    internal class ApplyDiscountDomainServiceTests
    {
        private readonly ApplyDiscountDomainService _service;

        public ApplyDiscountDomainServiceTests()
        {
            _service = new ApplyDiscountDomainService();
        }

        [Test]
        public void ApplyDiscountsToBasket_ShouldReturnNone_WhenBasketIsNull()
        {
            // Arrange
            IEnumerable<BasketItem> basketItem = null!;
            var discounts = new List<Discount>();

            // Act
            var result = _service.ApplyDiscounts(basketItem, discounts);

            // Assert
            result.Should().BeEmpty();
        }

        [Test]
        public void ApplyDiscountsToBasket_ShouldReturnNone_WhenDiscountsAreNullOrEmpty()
        {
            // Actions
            var resultWithNullDiscounts = _service.ApplyDiscounts([], null!);
            var resultWithEmptyDiscounts = _service.ApplyDiscounts([], []);

            // Verify
            resultWithNullDiscounts.Should().BeEmpty();
        }

        [Test]
        public void ApplyDiscountsToBasket_ShouldApplyDiscounts_ConditionalDivision_ShouldBeOk()
        {
            // Arrange
            var soupId = BasicValue.SoupId;
            var breadId = BasicValue.BreadId;

            IEnumerable<BasketItem> basketItems = [
                new()
                {
                    ItemId = soupId,
                    Quantity = 2,
                    UnitPrice = 3m,
                    Id = Guid.NewGuid(),
                },
                new()
                {
                    ItemId = breadId,
                    Quantity = 1,
                    UnitPrice = 10m,
                    Id = Guid.NewGuid()
                },
            ];

            List<Discount> discounts = [
                new() {
                    Name = "Bread discount",
                    DiscountType = DiscountType.ConditionalDivision,
                    ItemId = breadId,
                    Value = 2,
                    ItemConditionalId = soupId,
                    ConditionalQuantity = 2
                },
            ];

            // Action
            var result = _service.ApplyDiscounts(basketItems, discounts);

            // Verify
            result.Sum(x => x.Discounts).Should().Be(5m);
        }

        [Test]
        public void ApplyDiscountsToBasket_ShouldApplyDiscounts_ConditionalDivisionWith3SoupAnd2Bread_DiscountOnlyOne_ShouldBeOk()
        {
            // Arrange
            var soupId = BasicValue.SoupId;
            var breadId = BasicValue.BreadId;

            IEnumerable<BasketItem> basketItems = [
                    new()
                    {
                        ItemId = soupId,
                        Quantity = 3,
                        UnitPrice = 3m,
                        Id = Guid.NewGuid(),
                    },
                    new()
                    {
                        ItemId = breadId,
                        Quantity = 2,
                        UnitPrice = 10m,
                        Id = Guid.NewGuid()
                    },
                ];

            List<Discount> discounts = [
                new() {
                    Name = "Bread discount",
                    DiscountType = DiscountType.ConditionalDivision,
                    ItemId = breadId,
                    Value = 2,
                    ItemConditionalId = soupId,
                    ConditionalQuantity = 2
                },
            ];

            // Action
            var result = _service.ApplyDiscounts(basketItems, discounts);

            // Verify
            result.Sum(x => x.Discounts).Should().Be(5m);
        }

        [Test]
        public void ApplyDiscountsToBasket_ShouldApplyDiscounts_ConditionalDivisionWith4SoupAnd2Bread_2Discount_ShouldBeOk()
        {
            // Arrange
            var soupId = BasicValue.SoupId;
            var breadId = BasicValue.BreadId;

            IEnumerable<BasketItem> basketItem = [
                    new()
                    {
                        ItemId = soupId,
                        Quantity = 4,
                        UnitPrice = 3m,
                        Id = Guid.NewGuid(),
                    },
                    new()
                    {
                        ItemId = breadId,
                        Quantity = 2,
                        UnitPrice = 10m,
                        Id = Guid.NewGuid()
                    },
                ];

            List<Discount> discounts = [
                new() {
                    Name = "Bread discount",
                    DiscountType = DiscountType.ConditionalDivision,
                    ItemId = breadId,
                    Value = 2,
                    ItemConditionalId = soupId,
                    ConditionalQuantity = 2
                },
            ];

            // Action
            var result = _service.ApplyDiscounts(basketItem, discounts);

            // Verify
            result.Sum(x => x.Discounts).Should().Be(10m);
        }

        [Test]
        public void ApplyDiscountsToBasket_ShouldApplyDiscounts_WhenValidBasketAndDiscounts_WithItemIds_ShouldApplyOnlyForSpeficItem()
        {
            // Arrange
            var itemId = Guid.NewGuid();
            IEnumerable<BasketItem> basketItems = [
                    new()
                    {
                        ItemId = itemId,
                        UnitPrice = 100m,
                        Quantity = 2
                    }
                ];
            List<Discount> discounts = [
                new() { Name = "10%", DiscountType = DiscountType.Percentage, Value = 10, ItemId = itemId },
                new () { Name = "Fixed 20",  DiscountType = DiscountType.Fixed, Value = 20, ItemId = itemId }
            ];

            // Action
            var result = _service.ApplyDiscounts(basketItems, discounts);

            // Verify
            result.Sum(x => x.Discounts).Should().Be(60m);
        }

        [Test]
        public void ApplyDiscountsToBasket_ShouldApplyDiscounts_WhenValidBasketAndDiscounts()
        {
            // Arrange
            var itemId = Guid.NewGuid();
            IEnumerable<BasketItem> basketItems = [
                    new()
                    {
                        Quantity = 2,
                        UnitPrice = 100m,
                        BasketId = Guid.NewGuid(),
                        ItemId = itemId,
                    }
                ];
            List<Discount> discounts = [
                new() { Name = "10%", DiscountType = DiscountType.Percentage, Value = 10, ItemId = itemId },
                new () { Name = "Fixed 20",  DiscountType = DiscountType.Fixed, Value = 20, ItemId = itemId }
            ];

            // Action
            var result = _service.ApplyDiscounts(basketItems, discounts);

            // Verify
            result.Sum(x => x.Discounts).Should().Be(60m);
        }

        [Test]
        public void ApplyDiscountsToBasket_ShouldApplyPercentageDiscountCorrectly()
        {
            // Arrange
            var itemId = Guid.NewGuid();
            IEnumerable<BasketItem> basketItems = [
                    new()
                    {
                        Quantity = 1,
                        UnitPrice = 200m,
                        ItemId = itemId
                    }];
            List<Discount> discounts = [new() {
                Name = "20%", DiscountType = DiscountType.Percentage, Value = 20 , ItemId = itemId
            }];

            // Action
            var result = _service.ApplyDiscounts(basketItems, discounts);

            // Verify
            result.Sum(x => x.Discounts).Should().Be(40m);
        }

        [Test]
        public void ApplyDiscountsToBasket_ShouldApplyFixedDiscountCorrectly()
        {
            // Arrange
            var itemId = Guid.NewGuid();
            IEnumerable<BasketItem> basketItems = [
                new ()
                {
                    Item = new() { Price = 150m },
                    Quantity = 1,
                    UnitPrice = 150m,
                    ItemId = itemId
                }
            ];
            List<Discount> discounts = [new() { Name = "Fixed", DiscountType = DiscountType.Fixed, Value = 50, ItemId = itemId }];

            // Action
            var result = _service.ApplyDiscounts(basketItems, discounts);

            // Verify
            result.Sum(x => x.Discounts).Should().Be(50m);
        }

        [Test]
        public void ApplyDiscountsToBasket_ShouldApplyMultipleDiscountsCorrectly()
        {
            // Arrange
            var itemId= Guid.NewGuid();
            IEnumerable<BasketItem> basketItems = [
                    new()
                    {
                        Quantity = 3,
                        ItemId = itemId,
                        UnitPrice = 100m
                    },
                    new()
                    {
                        Quantity = 1,
                        ItemId = itemId,
                        UnitPrice = 200m
                    }];
            List<Discount> discounts = [
                new() { Name = "10%", DiscountType = DiscountType.Percentage, Value = 10 , ItemId = itemId},
                new() { Name = "Fixed", DiscountType = DiscountType.Fixed, Value = 50 , ItemId = itemId }];

            // Action
            var result = _service.ApplyDiscounts(basketItems, discounts);

            // Verify
            result.Sum(x => x.Discounts).Should().Be(250m);
        }
    }
}
