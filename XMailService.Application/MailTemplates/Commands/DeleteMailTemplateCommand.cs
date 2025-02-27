using MediatR;
using XMailService.Application.Interfaces;

namespace XMailService.Application.MailTemplates.Commands;

public sealed record DeleteMailTemplateCommand : IRequest
{
    public required string Name { get; init; }

    internal sealed class Handler(IUnitOfWork unitOfWork) : IRequestHandler<DeleteMailTemplateCommand>
    {
        public async Task Handle(DeleteMailTemplateCommand request, CancellationToken cancellationToken)
            => await unitOfWork.MailTemplates.Delete(request.Name, cancellationToken);
    }
}
