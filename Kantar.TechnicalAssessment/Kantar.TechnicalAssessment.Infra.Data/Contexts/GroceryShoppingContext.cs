using Kantar.TechnicalAssessment.Domain.Features;
using Kantar.TechnicalAssessment.Infra.Data.EntityConfigurations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Kantar.TechnicalAssessment.Infra.Data.Contexts
{
    public class GroceryShoppingContext : DbContext
    {
        public virtual DbSet<Basket> Baskets { get; set; }

        public GroceryShoppingContext(DbContextOptions<GroceryShoppingContext> options) : base(options)
        {
            ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
            => base.OnModelCreating(
                    modelBuilder.ApplyConfiguration(new BasketEntityConfiguration()));
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseLazyLoadingProxies();
            optionsBuilder.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
            optionsBuilder.LogTo(Console.WriteLine);
            base.OnConfiguring(optionsBuilder);
        }
    }
}
