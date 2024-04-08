using Coordinator.Enums;

namespace Coordinator.Models
{
    public record NodeState(Guid TransactionId)
    {
        public Guid Id { get; set; }
        /// <summary>
        /// 1. asamanin durumunu ifade eder
        /// </summary>
        public ReadyType IsReady { get; set; }
        /// <summary>
        /// 2. asamanin neticesinde islemin basarili tamamlanip, tamamlanmadigini ifade eder.
        /// </summary>
        public TransactionState TransactionState { get; set; }
        public Node Node { get; set; }
    }
}

