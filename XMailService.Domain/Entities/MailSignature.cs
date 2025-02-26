using XMailService.Domain.Entities.Commons;

namespace XMailService.Domain.Entities;

public class MailSignature : EntityBase
{
    public string Name { get; set; } = default!;
    public string FileLocation { get; set; } = default!;
    public string? Description { get; set; }
    public bool IsHtml { get; set; }
    public int Version { get; set; }
}
