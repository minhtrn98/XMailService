using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using XMailService.Domain.Entities;

namespace XMailService.Infrastructure.MailTemplates;

public sealed class MailTemplateConfiguration : IEntityTypeConfiguration<MailTemplate>
{
    public void Configure(EntityTypeBuilder<MailTemplate> builder)
    {
        builder.Property(w => w.Name).HasMaxLength(100);
        builder.Property(w => w.Subject).HasMaxLength(500);
        builder.Property(w => w.Description).HasMaxLength(500);

        builder.Property<uint>("RowVersion").IsRowVersion();
        builder.HasIndex(w => w.Name).IsUnique();
        builder.HasQueryFilter(x => !x.SoftDeleted);
    }
}
