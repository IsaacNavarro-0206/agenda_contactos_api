using FluentValidation;

namespace AgendaContactos.Api;

public sealed class RegisterUserRequestValidator : AbstractValidator<RegisterUserRequest>
{
    public RegisterUserRequestValidator()
    {
        RuleFor(user => user.Name)
            .NotEmpty().WithErrorCode("El nombre es obligatorio.").ChildRules(name =>
                    {
                        name.RuleFor(n => n)
                        .MinimumLength(3).WithErrorCode("El nombre debe tener al menos 3 caracteres.");
                        name.RuleFor(n => n)
                        .MaximumLength(255).WithErrorCode("El nombre no puede exceder los 255 caracteres.");
                    }
                );

        RuleFor(user => user.Email)
            .NotEmpty().WithErrorCode("El correo electrónico es obligatorio.")
            .EmailAddress().WithErrorCode("El correo electrónico debe tener un formato válido.");

        RuleFor(user => user.Password)
            .NotEmpty().WithMessage("La contraseña es obligatoria.")
            .MinimumLength(12).WithMessage("La contraseña debe tener al menos 12 caracteres.")
            .MaximumLength(128).WithMessage("La contraseña no puede exceder los 128 caracteres.");
    }
}