using System.ComponentModel.DataAnnotations;

namespace Lighthouse.Catalog.Service.Dtos;

public record ItemDto
(
    Guid Id,
    string Name,
    string Description,
    int Price,
    DateTimeOffset CreatedDate
);

public record CreateItemDto
(
    [Required] string Name,
    string Description,
    [Range(0, 1000)] int Price
);

public record UpdateItemDto
(
    [Required] string Name,
    string Description,
    [Range(0, 1000)] int Price
);