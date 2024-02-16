using BlocksManagerBackend.Blocks.Domain.Entities;

namespace BlocksManagerBackend.Blocks.Domain;
public interface IBlockRepository<T> where T : Block
{
    Task<T> GetByKey(string key);
    Task<bool> CreateBlock(string key, T block);
    Task<bool> UpdateBlock(string key, string sectionID, T block);
    Task<bool> DeleteBlock(string key, string sectionID);


}