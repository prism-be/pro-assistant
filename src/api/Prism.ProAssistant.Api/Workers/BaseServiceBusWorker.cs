// -----------------------------------------------------------------------
//  <copyright file = "BaseServiceBusWorker.cs" company = "Prism">
//  Copyright (c) Prism.All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using System.Text;
using System.Text.Json;
using MediatR;
using Prism.ProAssistant.Business;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Prism.ProAssistant.Api.Workers;

public abstract class BaseServiceBusWorker<T> : BackgroundService
{
    private readonly IConnection? _connection;
    private readonly ILogger _logger;
    private readonly IServiceProvider _serviceProvider;

    private IModel? _channel;
    private string? _consumerTag;

    protected BaseServiceBusWorker(ILogger logger, IServiceProvider serviceProvider, IConnection? connection)
    {
        _logger = logger;
        _serviceProvider = serviceProvider;
        _connection = connection;
    }

    public abstract string Queue { get; }

    public abstract string WorkerName { get; }

    protected virtual ushort PrefetchCount => Convert.ToUInt16(EnvironmentConfiguration.GetConfiguration("RABBITMQ_PREFETCH_DEFAULT") ?? "50");

    public override void Dispose()
    {
        if (_channel != null)
        {
            if (!string.IsNullOrWhiteSpace(_consumerTag))
            {
                _channel.BasicCancel(_consumerTag);
            }

            _channel.Dispose();
        }

        if (_connection != null)
        {
            _connection.Dispose();
        }

        GC.SuppressFinalize(this);
        base.Dispose();
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Start listening on queue {queue} with worker {workerName}", Queue, WorkerName);

        _channel = _connection!.CreateModel();

        _channel.QueueDeclare("workers/" + WorkerName, true, false, false);
        _channel.QueueBind("workers/" + WorkerName, Queue, "*");

        _channel.BasicQos(0, PrefetchCount, false);

        var consumer = new EventingBasicConsumer(_channel);
        consumer.Received += async (_, args) =>
        {
            using var scope = _serviceProvider.CreateScope();

            try
            {
                _logger.LogInformation("Processing message {id} on queue {queue} with worker {workerName}", args.DeliveryTag, Queue, WorkerName);

                var body = args.Body.ToArray();
                var json = Encoding.Default.GetString(body);
                var payload = JsonSerializer.Deserialize<T>(json);

                if (payload != null)
                {
                    var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
                    await ProcessMessageAsync(mediator, payload);
                }

                _channel.BasicAck(args.DeliveryTag, false);
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex, "Cannot process message {id} on queue {queue}", args.DeliveryTag, Queue);
            }
        };

        _consumerTag = _channel.BasicConsume("workers/" + WorkerName, false, consumer);

        return Task.CompletedTask;
    }

    public abstract Task ProcessMessageAsync(IMediator mediator, T payload);
}