using Prism.ProAssistant.Api.Extensions;
using Prism.ProAssistant.Api.Healtchecks;
using Prism.ProAssistant.Api.Middlewares;
using Prism.ProAssistant.Api.Models;

var builder = WebApplication.CreateBuilder(args);

builder.AddApplicationInsights();
builder.AddSerilog();

builder.Services.AddDatabase();

builder.Services.AddProAssistant();

builder.Services.AddHttpContextAccessor();
builder.Services.AddBearer();
builder.Services.AddHealthChecks()
    .AddCheck<CheckCache>("Cache")
    .AddCheck<CheckDatabase>("Database");
builder.Services.AddControllers();

builder.Services.AddSwaggerGen(options =>
{
    options.SupportNonNullableReferenceTypes();
    options.SchemaFilter<RequiredNotNullableSchemaFilter>();
});

var app = builder.Build();
app.UseMiddleware<ErrorLoggingMiddleware>();

app.UseHealthChecks("/health/check");
app.UseHealthChecks("/health/ready");
app.UseHealthChecks("/health/startup");
app.MapControllers();

app.UseRouting();
app.UseAuthentication().UseAuthorization();

app.UseDefaultFiles();
app.UseStaticFiles();
app.MapFallbackToFile("index.html");

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
}

app.ReplaceEnvironmentVariables();

app.Run();