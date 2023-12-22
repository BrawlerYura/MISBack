using Microsoft.EntityFrameworkCore;
using MISBack.Data;

namespace MISBack.Services.Token;

public class TokenCleanerService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;

    public TokenCleanerService(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            using var scope = _serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            var tokens = await context.Token.Where(t => t.ExpiredDate <= DateTime.UtcNow)
                .ToListAsync(cancellationToken: stoppingToken);

            foreach (var token in tokens)
            {
                context.Remove(token);
            }

            await context.SaveChangesAsync(stoppingToken);
            
            await Task.Delay(30000000, stoppingToken);
        }
    }
}