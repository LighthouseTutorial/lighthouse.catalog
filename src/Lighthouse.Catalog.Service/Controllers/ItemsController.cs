using Microsoft.AspNetCore.Mvc;
using Lighthouse.Catalog.Service.Dtos;
using Lighthouse.Catalog.Service.Entities;
using Lighthouse.Common;
using MassTransit;
using Lighthouse.Catalog.Contracts;
using Microsoft.AspNetCore.Authorization;

namespace Lighthouse.Catalog.Service.Controllers;

[ApiController]
[Route("items")]
public class ItemsController : ControllerBase
{
    private const string AdminRole = "Admin";
    private readonly IRepository<Item> _itemsRepository;
    private readonly IPublishEndpoint _publishEndpoint;

    public ItemsController(IRepository<Item> itemsRepository, IPublishEndpoint publishEndpoint)
    {
        _itemsRepository = itemsRepository;
        _publishEndpoint = publishEndpoint;
    }

    [HttpGet]
    [Authorize(Policies.Read)]
    public async Task<ActionResult<IEnumerable<ItemDto>>> GetAllAsync()
    {
        var items = await _itemsRepository.GetAllAsync();
        return Ok(items.Select(item => item.AsDto()));
    }

    [HttpGet("{id}")]
    [Authorize(Policies.Read)]
    public async Task<ActionResult<ItemDto>> GetByIdAsync(Guid id)
    {
        var item = await _itemsRepository.GetAsync(id);

        if (item is null)
        {
            return NotFound();
        }

        return Ok(item.AsDto());
    }

    [HttpPost]
    [Authorize(Policies.Write)]
    public async Task<ActionResult<ItemDto>> CreateItemAsync(CreateItemDto newItemDto)
    {
        var item = new Item
        {
            Id = Guid.NewGuid(),
            Name = newItemDto.Name,
            Description = newItemDto.Description,
            Price = newItemDto.Price,
            CreatedAt = DateTimeOffset.UtcNow
        };

        await _itemsRepository.CreateAsync(item);

        await _publishEndpoint.Publish(new CatalogItemCreated(item.Id, item.Name, item.Description, item.Price));

        return CreatedAtAction(nameof(GetByIdAsync), new { id = item.Id }, item);
    }

    [HttpPut("{id}")]
    [Authorize(Policies.Write)]
    public async Task<IActionResult> UpdateItem(Guid id, UpdateItemDto updatedItemDto)
    {
        var existingItem = await _itemsRepository.GetAsync(id);

        if (existingItem is null)
        {
            return NotFound();
        }

        existingItem.Name = updatedItemDto.Name;
        existingItem.Description = updatedItemDto.Description;
        existingItem.Price = updatedItemDto.Price;

        await _itemsRepository.UpdateAsync(existingItem);

        await _publishEndpoint.Publish(new CatalogItemUpdated(existingItem.Id, existingItem.Name, existingItem.Description, existingItem.Price));

        return NoContent();
    }

    [HttpDelete("{id}")]
    [Authorize(Policies.Write)]
    public async Task<IActionResult> DeleteItem(Guid id)
    {
        var existingItem = await _itemsRepository.GetAsync(id);

        if (existingItem is null)
        {
            return NotFound();
        }

        await _itemsRepository.RemoveAsync(existingItem.Id);

        await _publishEndpoint.Publish(new CatalogItemDeleted(id));

        return NoContent();
    }
}