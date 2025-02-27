using XMailService.Domain.Entities.Commons;

namespace XMailService.Domain.Entities;

public class MailTemplate : EntityBase
{
    public string Name { get; set; } = default!;
    public string Subject { get; set; } = default!;
    public string? Description { get; set; }
    public int Version { get; set; }
}
