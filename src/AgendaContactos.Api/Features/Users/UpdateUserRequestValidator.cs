using FluentValidation;

namespace AgendaContactos.Api;

public sealed class UpdateUserRequestValidator : AbstractValidator<UpdateUserRequest>
{
    public UpdateUserRequestValidator()
    {
        RuleFor(user => user.Name).NotEmpty().WithErrorCode("El nombre es obligatorio.").ChildRules(name =>
        {
            name.RuleFor(n => n).MinimumLength(3).WithErrorCode("El nombre debe tener al menos 3 caracteres.");
            name.RuleFor(n => n).MaximumLength(255).WithErrorCode("El nombre no puede exceder los 255 caracteres.");
        }
        );

        RuleFor(user => user.Email).NotEmpty().WithErrorCode("El correo electrónico es obligatorio.").EmailAddress().WithErrorCode("El correo electrónico debe tener un formato válido.");
    }
}