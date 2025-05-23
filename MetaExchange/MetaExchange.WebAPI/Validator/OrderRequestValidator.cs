using FluentValidation;
using MetaExchange.Application.DTOs;

namespace MetaExchange.WebAPI.Validator
{
    public class OrderRequestValidator : AbstractValidator<OrderRequest>
    {
        public OrderRequestValidator()
        {
            RuleFor(x => x.Amount)
                .GreaterThan(0).WithMessage("Amount must be greater than 0.");

            RuleFor(x => x.Type)
                .IsInEnum().WithMessage("OrderType must be Buy or Sell.");
        }
    }
}