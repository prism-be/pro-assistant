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

app.UseHealthChecks("/health");
app.UseHealthChecks("/ready");
app.UseHealthChecks("/startup");
app.MapControllers();

app.UseRouting();
app.UseAuthentication().UseAuthorization();
app.UseDefaultFiles();
app.UseStaticFiles();

app.UseMiddleware<NextJsRouterMiddleWare>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
}

app.ReplaceEnvironmentVariables();

app.Run();