using FluentValidation;

namespace AgendaContactos.Api;

public sealed class ContactRequestValidator : AbstractValidator<ContactRequest>
{
    public ContactRequestValidator()
    {
        RuleFor(contact => contact.Name).NotEmpty().WithMessage("El nombre es obligatorio.").ChildRules(name =>
        {
            name.RuleFor(n => n).MinimumLength(3).WithMessage("El nombre debe tener al menos 3 caracteres.");
            name.RuleFor(n => n).MaximumLength(255).WithMessage("El nombre no puede exceder los 50 caracteres.");
        });

        RuleFor(contact => contact.Phone).NotEmpty().WithMessage("El teléfono es obligatorio.")
            .Matches(@"^\+?\d{10,15}$").WithMessage("El teléfono debe tener un formato válido.");

        RuleFor(contact => contact.Email).NotEmpty().WithMessage("El correo electrónico es obligatorio.")
            .EmailAddress().WithMessage("El correo electrónico debe tener un formato válido.");
    }
}