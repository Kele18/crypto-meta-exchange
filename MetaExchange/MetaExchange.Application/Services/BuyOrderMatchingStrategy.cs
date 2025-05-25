using MetaExchange.Application.DTOs;
using MetaExchange.Application.DTOs.MetaExchange.Application.Models;
using MetaExchange.Application.Interfaces.Strategies;
using MetaExchange.Domain;

namespace MetaExchange.Application.Services
{
    public class BuyOrderMatchingStrategy : IOrderMatchingStrategy
    {
        public List<MatchedOrder> Match(List<OrderBook> books, decimal targetAmount)
        {
            var candidates = CreateBuyOrderCandidates(books)
                             .Where(i => i.IsViable)
                             .OrderBy(j => j.Order.Price)
                             .ToList();

            var results = new List<MatchedOrder>();
            decimal remaining = targetAmount;

            foreach (var candidate in candidates)
            {
                if (remaining <= 0) break;

                decimal toUse = Math.Min(remaining, candidate.MaxBuyableAmount);
                results.Add(new MatchedOrder(candidate.ExchangeName, candidate.Order, toUse));
                remaining -= toUse;
            }

            return remaining > 0 ? [] : results;
        }

        private IEnumerable<BuyOrderCandidate> CreateBuyOrderCandidates(List<OrderBook> books)
        {
            foreach (var book in books)
            {
                foreach (var ask in book.Asks)
                {
                    decimal maxBuyable = CalculateMaxBuyableAmount(ask, book);
                    yield return new BuyOrderCandidate(book.ExchangeName, ask, maxBuyable);
                }
            }
        }

        private decimal CalculateMaxBuyableAmount(Order order, OrderBook book)
        {
            return Math.Min(order.Amount, book.AvailableEur / order.Price);
        }
    }
}