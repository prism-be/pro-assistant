using FluentValidation;
using MediatR;
using Prism.ProAssistant.Business;
using Prism.ProAssistant.Business.Behaviors;

var builder = WebApplication.CreateBuilder(args);

// Add Mediatr
var applicationAssembly = typeof(EntryPoint).Assembly;
builder.Services.AddMediatR(new[] { applicationAssembly }, config => config.AsScoped());
builder.Services.AddScoped(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
builder.Services.AddScoped(typeof(IPipelineBehavior<,>), typeof(LogCommandsBehavior<,>));
builder.Services.AddValidatorsFromAssembly(applicationAssembly);

var app = builder.Build();

app.MapGet("/", () => "Hello World!");

app.Run();