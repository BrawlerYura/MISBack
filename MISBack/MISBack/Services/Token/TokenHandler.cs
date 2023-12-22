using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Text.Json;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Net.Http.Headers;
using MISBack.Data;
using MISBack.Data.Entities;

namespace BlogApi.Services.Token;

public class ValidateTokenRequirement : IAuthorizationRequirement
{
    public ValidateTokenRequirement()
    {
    }
}

public class ValidateTokenHandler : AuthorizationHandler<ValidateTokenRequirement>
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IServiceScopeFactory _serviceScopeFactory;

    public ValidateTokenHandler(IHttpContextAccessor httpContextAccessor, IServiceScopeFactory serviceScopeFactory)
    {
        _httpContextAccessor = httpContextAccessor;
        _serviceScopeFactory = serviceScopeFactory;
    }

    protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, ValidateTokenRequirement requirement)
    {
        try
        {
            if (_httpContextAccessor.HttpContext != null)
            {
                string? authorizationString = _httpContextAccessor.HttpContext.Request.Headers[HeaderNames.Authorization];
                if (string.IsNullOrEmpty(authorizationString) || !authorizationString.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
                {
                    throw new UnauthorizedAccessException("Invalid token");
                }

                var token = authorizationString.Substring("Bearer ".Length).Trim();

                using var scope = _serviceScopeFactory.CreateScope();
                var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                
                var handler = new JwtSecurityTokenHandler();
                var jsonToken = handler.ReadToken(token) as JwtSecurityToken;
                
                if (jsonToken != null)
                {

                    var tokenEntity = await dbContext.Token
                        .FirstOrDefaultAsync(x => x.InvalidToken == token);
                
                    if (tokenEntity != null)
                    {
                        throw new UnauthorizedAccessException("Token expired");
                    }
                    
                    if (jsonToken.ValidTo < DateTime.UtcNow)
                    {
                        dbContext.Token.Add(new MISBack.Data.Entities.Token
                        {
                            InvalidToken = token,
                            ExpiredDate = jsonToken.ValidTo
                        });

                        await dbContext.SaveChangesAsync();
                        
                        throw new UnauthorizedAccessException("Token expired");
                    }
                }
            }

            context.Succeed(requirement);
        }
        catch (UnauthorizedAccessException ex)
        {
            var response = _httpContextAccessor.HttpContext?.Response;
            if (response != null)
            {
                response.StatusCode = (int)HttpStatusCode.Unauthorized;
                response.ContentType = "application/json";

                ProblemDetails problem = new()
                {
                    Status = (int)HttpStatusCode.Unauthorized,
                    Type = "Unauthorized",
                    Title = "Unauthorized",
                    Detail = ex.Message
                };

                string json = JsonSerializer.Serialize(problem);
                
                await response.WriteAsync(json);
            }
        }
    }
}

