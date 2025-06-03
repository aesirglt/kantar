using FluentAssertions;
using Kantar.TechnicalAssessment.Domain.Enums;
using Kantar.TechnicalAssessment.Domain.Features;
using Kantar.TechnicalAssessment.DomainServices;
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
            Basket basket = null!;
            var discounts = new List<Discount>();

            // Act
            var result = _service.ApplyDiscountsToBasket(basket, discounts);

            // Assert
            OptionModule.IsNone(result).Should().BeTrue();
        }

        [Test]
        public void ApplyDiscountsToBasket_ShouldReturnNone_WhenDiscountsAreNullOrEmpty()
        {
            // Arrange
            var basket = new Basket { BasketItems = [] };

            // Actions
            var resultWithNullDiscounts = _service.ApplyDiscountsToBasket(basket, null!);
            var resultWithEmptyDiscounts = _service.ApplyDiscountsToBasket(basket, []);

            // Verify
            OptionModule.IsNone(resultWithNullDiscounts).Should().BeTrue();
            OptionModule.IsNone(resultWithEmptyDiscounts).Should().BeTrue();
        }

        [Test]
        public void ApplyDiscountsToBasket_ShouldApplyDiscounts_WhenValidBasketAndDiscounts_WithItemIds_ShouldApplyOnlyForSpeficItem()
        {
            // Arrange
            var itemId = Guid.NewGuid();
            var basket = new Basket
            {
                BasketItems = [
                    new()
                    {
                        ItemId = itemId,
                        Item = new () {
                            Id = itemId,
                            Price = 100m
                        },
                        Quantity = 2
                    }
                ]
            };
            List<Discount> discounts = [
                new() { Name = "10%", DiscountType = DiscountType.Percentage, Amount = 10, ItemId = Guid.NewGuid() },
                new () { Name = "Fixed 20",  DiscountType = DiscountType.Fixed, Amount = 20, ItemId = itemId }
            ];

            // Action
            var result = _service.ApplyDiscountsToBasket(basket, discounts);

            // Verify
            OptionModule.IsSome(result).Should().BeTrue();
            result.Value.Discounts.Should().Be(40m);
        }

        [Test]
        public void ApplyDiscountsToBasket_ShouldApplyDiscounts_WhenValidBasketAndDiscounts()
        {
            // Arrange
            var basket = new Basket
            {
                BasketItems = [
                    new()
                    {
                        Item = new () { Price = 100m },
                        Quantity = 2
                    }
                ]
            };
            List<Discount> discounts = [
                new() { Name = "10%", DiscountType = DiscountType.Percentage, Amount = 10 },
                new () { Name = "Fixed 20",  DiscountType = DiscountType.Fixed, Amount = 20 }
            ];

            // Action
            var result = _service.ApplyDiscountsToBasket(basket, discounts);

            // Verify
            OptionModule.IsSome(result).Should().BeTrue();
            result.Value.Discounts.Should().Be(60m);
        }

        [Test]
        public void ApplyDiscountsToBasket_ShouldApplyPercentageDiscountCorrectly()
        {
            // Arrange
            var basket = new Basket
            {
                BasketItems = [
                    new()
                    {
                        Item = new () { Price = 200m },
                        Quantity = 1
                    }]
            };
            List<Discount> discounts = [new() { Name = "20%", DiscountType = DiscountType.Percentage, Amount = 20 }];

            // Action
            var result = _service.ApplyDiscountsToBasket(basket, discounts);

            // Verify
            OptionModule.IsSome(result).Should().BeTrue();
            result.Value.Discounts.Should().Be(40m);
        }

        [Test]
        public void ApplyDiscountsToBasket_ShouldApplyFixedDiscountCorrectly()
        {
            // Arrange
            var basket = new Basket
            {
                BasketItems = [new BasketItem
                {
                    Item = new() { Price = 150m },
                    Quantity = 1
                }]
            };
            List<Discount> discounts = [new() { Name = "Fixed", DiscountType = DiscountType.Fixed, Amount = 50 }];

            // Action
            var result = _service.ApplyDiscountsToBasket(basket, discounts);

            // Verify
            OptionModule.IsSome(result).Should().BeTrue();
            result.Value.Discounts.Should().Be(50);
        }

        [Test]
        public void ApplyDiscountsToBasket_ShouldApplyMultipleDiscountsCorrectly()
        {
            // Arrange
            var basket = new Basket
            {
                BasketItems = [
                    new()
                    {
                        Item = new() { Price = 100m },
                        Quantity = 3
                    },
                    new()
                    {
                        Item = new() { Price = 200m },
                        Quantity = 1
                    }]
            };
            List<Discount> discounts = [
                new() { Name = "10%", DiscountType = DiscountType.Percentage, Amount = 10 },
                new() { Name = "Fixed", DiscountType = DiscountType.Fixed, Amount = 50 }];

            // Action
            var result = _service.ApplyDiscountsToBasket(basket, discounts);

            // Verify
            OptionModule.IsSome(result).Should().BeTrue();
            result.Value.Discounts.Should().Be(250m);
        }
    }
}
