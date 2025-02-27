using MediatR;
using XMailService.Application.Interfaces;

namespace XMailService.Application.MailSignatures.Commands;

public sealed record DeleteMailSignatureCommand : IRequest
{
    public required string Name { get; init; }
    internal sealed class Handler(IUnitOfWork unitOfWork) : IRequestHandler<DeleteMailSignatureCommand>
    {
        public async Task Handle(DeleteMailSignatureCommand request, CancellationToken cancellationToken)
            => await unitOfWork.MailSignatures.Delete(request.Name, cancellationToken);
    }
}
