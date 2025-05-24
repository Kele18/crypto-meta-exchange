using FluentValidation;
using MetaExchange.Application.DTOs;
using MetaExchange.Domain;

namespace MetaExchange.WebAPI.Validator
{
    public class OrderRequestValidator : AbstractValidator<OrderRequest>
    {
        public OrderRequestValidator()
        {
            RuleFor(x => x.Amount)
                .GreaterThan(0).WithMessage("Amount must be greater than 0.");

            RuleFor(x => x.Type).Custom((value, context) =>
             {
                 if (!Enum.IsDefined(typeof(OrderType), value))
                 {
                     context.AddFailure("Type", "Invalid order type. Allowed values: Buy, Sell.");
                 }
             });
        }
    }
}