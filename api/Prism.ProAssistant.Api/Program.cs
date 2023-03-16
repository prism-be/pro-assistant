using Prism.ProAssistant.Api.Extensions;
using Prism.ProAssistant.Api.Middlewares;
using Prism.ProAssistant.Api.Models;

var builder = WebApplication.CreateBuilder(args);

builder.AddApplicationInsights();
builder.AddSerilog();

builder.Services.AddDatabase();

builder.Services.AddProAssistant();

builder.Services.AddHttpContextAccessor();
builder.Services.AddBearer();
builder.Services.AddHealthChecks();
builder.Services.AddControllers();

builder.Services.AddSwaggerGen(options =>
{
    options.SupportNonNullableReferenceTypes();
    options.SchemaFilter<RequiredNotNullableSchemaFilter>();
});

var app = builder.Build();
app.UseMiddleware<ErrorLoggingMiddleware>();

app.UseHealthChecks("/api/health");
app.MapControllers();

app.UseRouting();
app.UseAuthentication().UseAuthorization();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
}

app.Run();