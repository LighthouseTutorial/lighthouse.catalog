using Lighthouse.Common;

namespace Lighthouse.Catalog.Service.Entities;

public class Item : IEntity
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int Price { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
}