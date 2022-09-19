using FluentValidation;
using MediatR;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Driver;
using Prism.ProAssistant.Business;
using Prism.ProAssistant.Business.Behaviors;

var builder = WebApplication.CreateBuilder(args);

// Add Mediatr
var applicationAssembly = typeof(EntryPoint).Assembly;
builder.Services.AddMediatR(new[] { applicationAssembly }, config => config.AsScoped());
builder.Services.AddScoped(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
builder.Services.AddScoped(typeof(IPipelineBehavior<,>), typeof(LogCommandsBehavior<,>));
builder.Services.AddValidatorsFromAssembly(applicationAssembly);

// Add Mongo
var mongoDbConnectionString = EnvironmentConfiguration.GetMandatoryConfiguration("MONGODB_CONNECTION_STRING");

BsonSerializer.RegisterSerializer(new GuidSerializer(GuidRepresentation.Standard).WithRepresentation(BsonType.String));
var client = new MongoClient(mongoDbConnectionString);
builder.Services.AddSingleton<IMongoClient>(client);

var app = builder.Build();

app.MapGet("/", () => "Hello World!");

app.Run();