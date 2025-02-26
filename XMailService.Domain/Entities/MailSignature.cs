using XMailService.Domain.Entities.Commons;

namespace XMailService.Domain.Entities;

public class MailSignature : EntityBase
{
    public string Name { get; set; } = default!;
    public string Signature { get; set; } = default!;
    public bool IsHtml { get; set; }
    public int Version { get; set; }
}
