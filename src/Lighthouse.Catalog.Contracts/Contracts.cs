namespace Lighthouse.Catalog.Contracts;

public record CatalogItemCreated
(
    Guid ItemId,
    string Name,
    string Description,
    int Price
);

public record CatalogItemUpdated
(
    Guid ItemId,
    string Name,
    string Description,
    int Price
);

public record CatalogItemDeleted(Guid ItemId);
