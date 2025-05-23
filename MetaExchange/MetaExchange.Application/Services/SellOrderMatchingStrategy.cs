using MetaExchange.Application.DTOs;
using MetaExchange.Application.Interfaces.Strategies;
using MetaExchange.Domain;

namespace MetaExchange.Application.Services
{
    public class SellOrderMatchingStrategy : IOrderMatchingStrategy
    {
        public List<MatchedOrder> Match(List<OrderBook> books, decimal targetAmount)
        {
            var candidates = CreateSellOrderCandidates(books)
                             .Where(c => c.IsViable)
                             .OrderByDescending(c => c.Order.Price)
                             .ToList();

            return MatchSellOrders(candidates, targetAmount);
        }

        private IEnumerable<SellOrderCandidate> CreateSellOrderCandidates(List<OrderBook> books)
        {
            foreach (var book in books)
            {
                foreach (var bid in book.Bids)
                {
                    decimal maxSellable = CalculateMaxSellableAmount(bid, book);
                    yield return new SellOrderCandidate(book.ExchangeName, bid, maxSellable);
                }
            }
        }

        private decimal CalculateMaxSellableAmount(Order order, OrderBook book)
        {
            return Math.Min(order.Amount, book.AvailableBtc);
        }

        private List<MatchedOrder> MatchSellOrders(IEnumerable<SellOrderCandidate> candidates, decimal targetAmount)
        {
            var results = new List<MatchedOrder>();
            decimal remaining = targetAmount;

            foreach (var candidate in candidates)
            {
                if (remaining <= 0) break;

                var toUse = Math.Min(remaining, candidate.MaxSellableAmount);
                results.Add(new MatchedOrder(candidate.ExchangeName, candidate.Order, toUse));
                remaining -= toUse;
            }

            return remaining > 0 ? new List<MatchedOrder>() : results;
        }
    }
}