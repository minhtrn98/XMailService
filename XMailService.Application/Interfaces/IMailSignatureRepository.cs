﻿using XMailService.Domain.Entities;

namespace XMailService.Application.Interfaces;

public interface IMailSignatureRepository : IRepository<MailSignature>
{
    Task<int> GetCurrentVersion(string name, CancellationToken cancellationToken = default);
    Task<bool> Delete(string name, CancellationToken cancellationToken = default);
}
