using Lighthouse.Catalog.Service.Dtos;
using Lighthouse.Catalog.Service.Entities;

namespace Lighthouse.Catalog.Service;

public static class Extensions
{
    public static ItemDto AsDto(this Item item)
    {
        return new ItemDto
        (
            item.Id,
            item.Name,
            item.Description,
            item.Price,
            item.CreatedAt
        );
    }
}