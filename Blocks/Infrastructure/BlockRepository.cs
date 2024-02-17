using BlocksManagerBackend.Blocks.Domain;
using BlocksManagerBackend.Blocks.Domain.Entities;
using StackExchange.Redis;
using System.Text.Json;

namespace BlocksManagerBackend.Blocks.Infrastructure;

public class BlockRepository<T> : IBlockRepository<T> where T : Block
{
    private readonly IDatabase _redis;

    public BlockRepository(IConnectionMultiplexer redis)
    {
        _redis = redis.GetDatabase(1);
    }
    public async Task<bool> CreateBlock(string key, T block)
    {
        var serializedModel = JsonSerializer.Serialize(block);

        if (block is ServicesBlock servicesBlock && servicesBlock is not null)
            foreach (var card in servicesBlock.ServiceCards)
                card.SectionID = Guid.NewGuid();

        return await _redis.StringSetAsync(key, serializedModel);
    }
    public async Task<T> GetByKey(string key)
    {
        var block = await _redis.StringGetAsync(key);

        if (!block.HasValue) return Activator.CreateInstance<T>();

        return JsonSerializer.Deserialize<T>(block);
    }

    public async Task<bool> DeleteBlock(string key, string sectionID)
    {
        var existingModel = await _redis.StringGetAsync(key);

        if (!existingModel.HasValue) return false;

        var block = JsonSerializer.Deserialize<T>(existingModel);
        if (block == null) return false;


        if (!string.IsNullOrEmpty(sectionID) && (block is ServicesBlock servicesBlock && servicesBlock is not null))
        {
            var section = servicesBlock.ServiceCards.Find(f => f.SectionID.ToString() == sectionID);
            if (section == null)
                return false;
            servicesBlock.ServiceCards.Remove(section);
            return await UpdateBlock(key, string.Empty, block);
        }


        return await _redis.KeyDeleteAsync(key);
    }

    public async Task<bool> UpdateBlock(string key, string sectionID, T block)
    {
        var existingModel = await _redis.StringGetAsync(key);

        if (!existingModel.HasValue) return false;

        var model = JsonSerializer.Deserialize<T>(existingModel);

        if (!string.IsNullOrEmpty(sectionID) && (model is ServicesBlock servicesBlock && servicesBlock is not null))
        {
            if (block is ServicesBlock serviceCardBlock && serviceCardBlock is not null)
            {
                var section = servicesBlock.ServiceCards.Find(f => f.SectionID.ToString() == sectionID);
                if (section == null)
                    return false;
                servicesBlock.ServiceCards.Remove(section);
                if (serviceCardBlock.ServiceCards.Any())
                    foreach (var serviceCard in serviceCardBlock.ServiceCards)
                        servicesBlock.ServiceCards.Add(serviceCard);

                var serializedservicesBlock = JsonSerializer.Serialize(servicesBlock);
                return await _redis.StringSetAsync(key, serializedservicesBlock);
            }
        }
        var serializedBlock = JsonSerializer.Serialize(block);

        return await _redis.StringSetAsync(key, serializedBlock);
    }
      
}
