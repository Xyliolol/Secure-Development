using FluentValidation;
using CardStorageService.Models.Requests;

namespace CardStorageService.Models.Validators
{
    public class ClientRequestValidator : AbstractValidator<CreateClientRequest>
    {
        public ClientRequestValidator()
        {
            RuleFor(x => x.FirstName)
                .NotNull()
                .Length(3, 255);

            RuleFor(x => x.Surname)
                .NotNull()
                .Length(3, 255);

            RuleFor(x => x.Patronymic)
                .NotNull()
                .Length(3, 255);
        }
    }
}
