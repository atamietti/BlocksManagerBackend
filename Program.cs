using BlocksManagerBackend.Blocks.Domain;
using BlocksManagerBackend.Blocks.Domain.Entities;
using BlocksManagerBackend.Blocks.Infrastructure;
using StackExchange.Redis;

namespace BlocksManagerBackend;

public class Program
{
    protected static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddAuthorization();

        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        builder.Services.AddSingleton<IConnectionMultiplexer>(ConnectionMultiplexer.Connect("127.0.0.1:6379,abortConnect=false"));
        builder.Services.AddScoped(typeof(IBlockRepository<>), typeof(BlockRepository<>));

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();

        app.UseAuthorization();

        var mapMethod = typeof(Block).GetMethod(nameof(Block.MapType));
        foreach (var type in Block.RegisteredTypes)
            mapMethod?.MakeGenericMethod(type).Invoke(null, new object?[] { app });

        app.Run();
    }
}
