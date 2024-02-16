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
        _redis = redis.GetDatabase();
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
        var serializedModel = JsonSerializer.Serialize(model);

        if (!string.IsNullOrEmpty(sectionID) && (model is ServicesBlock servicesBlock && servicesBlock is not null))
        {
            if (block is ServiceCard serviceCard && serviceCard is not null)
            {
                var section = servicesBlock.ServiceCards.Find(f => f.SectionID.ToString() == sectionID);
                if (section == null)
                    return false;
                servicesBlock.ServiceCards.Remove(section);
                servicesBlock.ServiceCards.Add(serviceCard);
                return await UpdateBlock(key, string.Empty, block);
            }
        }

        return await _redis.StringSetAsync(key, serializedModel);
    }

    // Method to get a list of values by key
    private async Task<List<string>> GetListByKeyAsync(string key)
    {
        var values = await _redis.ListRangeAsync(key);
        var result = new List<string>();

        foreach (var value in values)
        {
            result.Add(value.ToString());
        }

        return result;
    }

}
