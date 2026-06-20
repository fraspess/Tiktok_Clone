using FluentValidation;

namespace Application.Features.Conversation.Create;

public class CreateConversationValidator : AbstractValidator<CreateConversationCommand>
{
    public CreateConversationValidator()
    {
        RuleFor(c => c.UsersIds)
            .NotNull().WithMessage("Чат має мати хоча б 1 користувача")
            .DependentRules(() =>
            {
                RuleFor(c => c.UsersIds)
                    .Must(p => p.Count > 0).WithMessage("Не може бути чат без людей")
                    .Must(p => p.Count <= 10).WithMessage("Максимальний розмір групового чату є 10 людей.");
            });
    }
}