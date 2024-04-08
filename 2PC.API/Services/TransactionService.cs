using Coordinator.Models;
using Coordinator.Models.Context;
using Coordinator.Services.Abstraction;
using Microsoft.EntityFrameworkCore;

namespace Coordinator.Services
{
    public class TransactionService(IHttpClientFactory _httpClientFactory, TwoPhaseCommitContext _context) : ITransactionService
    {

        HttpClient _orderHttpClient = _httpClientFactory.CreateClient("OrderAPI");
        HttpClient _stockHttpClient = _httpClientFactory.CreateClient("StockAPI");
        HttpClient _paymentHttpClient = _httpClientFactory.CreateClient("PaymentAPI");

        public async Task<Guid> CreateTransactionAsync()
        {
            Guid transactionId = Guid.NewGuid();

            var nodes = await _context.Nodes.ToListAsync();
            nodes.ForEach(node => node.NodeStates = new List<NodeState>()
            {
                new(transactionId)
                {
                    IsReady = Enums.ReadyType.Pending,
                    TransactionState = Enums.TransactionState.Pending
                }
            });

            await _context.SaveChangesAsync();

            return transactionId;
        }

        public async Task PrepareServicesAsync(Guid transactionId)
        {
            var transactionNodes = await _context.NodeStates.Include(ns => ns.Node)
                 .Where(ns => ns.TransactionId == transactionId)
                 .ToListAsync();

            foreach (var transactionNode in transactionNodes)
            {
                try
                {
                    var response = await (transactionNode.Node.Name switch
                    {
                        "OrderAPI" => _orderHttpClient.GetAsync("ready"),
                        "StockAPI" => _stockHttpClient.GetAsync("ready"),
                        "PaymentAPI" => _paymentHttpClient.GetAsync("ready"),
                    });

                    var result = bool.Parse(await response.Content.ReadAsStringAsync());
                    transactionNode.IsReady = result ? Enums.ReadyType.Ready : Enums.ReadyType.Unready;
                }
                catch (Exception)
                {
                    transactionNode.IsReady = Enums.ReadyType.Unready;
                }

                await _context.SaveChangesAsync();
            }
        }

        public async Task<bool> CheckReadyServicesAsync(Guid transactionId)
           => (await _context.NodeStates
                .Where(ns => ns.TransactionId == transactionId)
                .ToListAsync()).TrueForAll(ns => ns.IsReady == Enums.ReadyType.Ready);

        public async Task CommitAsync(Guid transactionId)
        {
            var transactionNodes = await _context.NodeStates
                 .Where(ns => ns.TransactionId == transactionId)
                 .Include(ns => ns.Node)
                 .ToListAsync();

            foreach (var transactionNode in transactionNodes)
            {
                try
                {
                    var response = await (transactionNode.Node.Name switch
                    {
                        "OrderAPI" => _orderHttpClient.GetAsync("commit"),
                        "StockAPI" => _stockHttpClient.GetAsync("commit"),
                        "PaymentAPI" => _paymentHttpClient.GetAsync("commit"),
                    });

                    var result = bool.Parse(await response.Content.ReadAsStringAsync());
                    transactionNode.TransactionState = result ? Enums.TransactionState.Done : Enums.TransactionState.Abort;
                }
                catch
                {
                    transactionNode.TransactionState = Enums.TransactionState.Abort;
                }
            }

            await _context.SaveChangesAsync();
        }

        public async Task<bool> CheckTransactionStateServicesAsync(Guid transactionId)
        => (await _context.NodeStates
            .Where(ns => ns.TransactionId == transactionId)
            .ToListAsync()).TrueForAll(ns => ns.TransactionState == Enums.TransactionState.Done);
        public async Task RollbackAsync(Guid transactionId)
        {
            var transactionNodes = await _context.NodeStates
                .Where(ns => ns.TransactionId == transactionId)
                .Include(ns => ns.Node)
                .ToListAsync();

            foreach (var transactionNode in transactionNodes)
            {
                try
                {
                    if (transactionNode.TransactionState == Enums.TransactionState.Done)
                    {
                        _ = await (transactionNode.Node.Name switch
                        {
                            "OrderAPI" => _orderHttpClient.GetAsync("rollback"),
                            "StockAPI" => _stockHttpClient.GetAsync("rollback"),
                            "PaymentAPI" => _paymentHttpClient.GetAsync("rollback"),
                        });

                        transactionNode.TransactionState = Enums.TransactionState.Abort;
                    }
                }
                catch
                {
                    transactionNode.TransactionState = Enums.TransactionState.Abort;
                }
            }

            await _context.SaveChangesAsync();
        }
    }
}
