﻿using FluentValidation;
using CardStorageService.Models.Requests;

namespace CardStorageService.Models.Validators
{
    public class AuthenticationRequestValidator : AbstractValidator<AuthenticationRequest>
    {
        public AuthenticationRequestValidator()
        {
            RuleFor(x => x.Login)
                .NotNull()
                .Length(5, 255)
                .EmailAddress();


            RuleFor(x => x.Password)
                .NotNull()
                .Length(5, 50);

        }
    }
}
