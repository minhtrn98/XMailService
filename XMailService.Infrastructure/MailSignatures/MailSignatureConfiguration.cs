using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using XMailService.Domain.Entities;

namespace XMailService.Infrastructure.MailSignatures;

public sealed class MailSignatureConfiguration : IEntityTypeConfiguration<MailSignature>
{
    public void Configure(EntityTypeBuilder<MailSignature> builder) => throw new NotImplementedException();
}
