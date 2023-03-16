using Prism.ProAssistant.Api.Extensions;
using Prism.ProAssistant.Api.Middlewares;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHttpContextAccessor();
builder.Services.AddBearer();
builder.Services.AddHealthChecks();
builder.Services.AddControllers();

var app = builder.Build();
app.UseMiddleware<ErrorLoggingMiddleware>();

app.UseHealthChecks("/api/health");
app.MapControllers();

app.UseRouting();
app.UseAuthentication().UseAuthorization();

app.Run();
