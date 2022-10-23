using FluentValidation;
using CardStorageService.Models.Requests;

namespace CardStorageService.Models.Validators
{
    public class CardRequestValidator : AbstractValidator<CreateCardRequest>
    {
        public CardRequestValidator()
        {
            RuleFor(x => x.CVV2)
                .NotNull()
                .Length(1, 3);

            RuleFor(x => x.CardNo)
                .NotNull()
                .Length(16);
        }
    }
}
