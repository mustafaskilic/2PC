using Microsoft.EntityFrameworkCore;

namespace Coordinator.Models.Context
{
    public class TwoPhaseCommitContext : DbContext
    {
        public TwoPhaseCommitContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<Node> Nodes { get; set; }
        public DbSet<NodeState> NodeStates { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Node>().HasData(
                new Node("OrderAPI") { Id = Guid.NewGuid(), },
                new Node("StockAPI") { Id = Guid.NewGuid(), },
                new Node("PaymentAPI") { Id = Guid.NewGuid(), }
                );
        }
    }
}
