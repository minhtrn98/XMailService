using System.ComponentModel.DataAnnotations;

namespace XMailService.Domain.Entities.Commons;

public interface IEntity
{
    Ulid Id { get; init; }
}

public interface ISoftDelete
{
    bool SoftDeleted { get; set; }
}

public interface IAuditEntity
{
    [MaxLength(200)]
    string CreatedBy { get; set; }
    DateTime CreatedDate { get; set; }
    [MaxLength(200)]
    string? UpdatedBy { get; set; }
    DateTime? UpdatedDate { get; set; }
}

public abstract class EntityBase : IEntity, IAuditEntity, ISoftDelete
{
    public Ulid Id { get; init; }
    public string CreatedBy { get; set; } = string.Empty;
    public DateTime CreatedDate { get; set; }
    public string? UpdatedBy { get; set; }
    public DateTime? UpdatedDate { get; set; }
    public bool SoftDeleted { get; set; }

    protected EntityBase() => Id = Ulid.NewUlid();
}
