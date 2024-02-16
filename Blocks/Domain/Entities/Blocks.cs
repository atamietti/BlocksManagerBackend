using System.Reflection;
using Microsoft.AspNetCore.Mvc;

namespace BlocksManagerBackend.Blocks.Domain.Entities;

public class Block
{
    public virtual string ID { get; }
    public int BlockOrder { get; set; }

    public static Type[] RegisteredTypes => Assembly.GetExecutingAssembly().GetTypes()?
      .Where(t => t.IsSubclassOf(typeof(Block)))?.ToArray() ?? Array.Empty<Type>();


    public static void MapType<T>(WebApplication app) where T : Block
    {
        Type type = typeof(T);
        app.MapGet($"/{type.Name.ToLower()}" + "{key}", async (string key, IBlockRepository<T> service) =>
        {
            var result = await service.GetByKey(key);
            return string.IsNullOrEmpty(result.ID) ? Results.NotFound(key) : Results.Ok(result);
        })
        .WithTags($"{type.Name}");

        app.MapPost($"/{type.Name.ToLower()}/", async (string key, T block, IBlockRepository<T> service) =>
        {
           if(HasErrors(key))
                return Results.StatusCode(500);

            var result = await service.CreateBlock(key, block);
            return result ? Results.Ok(result) : Results.StatusCode(500);
        })
        .WithTags($"{type.Name}");

        app.MapPut($"/{type.Name.ToLower()}/", async (string key, string sectionID, T updatedBlock, IBlockRepository<T> service) =>
        {
            var result = await service.UpdateBlock(key, sectionID, updatedBlock);
            return result ? Results.Ok("Success") : Results.StatusCode(500);
        })
        .WithTags($"{type.Name}");

        app.MapDelete($"/{type.Name.ToLower()}/", async (string key, string sectionID, IBlockRepository<T> service) =>
        {
            var result = await service.DeleteBlock(key, sectionID);
            return result ? Results.Ok("Success") : Results.StatusCode(500);
        })
       .WithTags($"{type.Name}");
    }

    private static bool HasErrors(string key)=>
        string.IsNullOrWhiteSpace(key) || key.Contains(" ");
}