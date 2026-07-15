using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;

namespace BingehOS.Infrastructure.Messaging;

public sealed class RabbitMqEventPublisher : IEventPublisher, IHostedService, IDisposable
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<RabbitMqEventPublisher> _logger;
    private IConnection? _connection;
    private IChannel? _channel;

    public RabbitMqEventPublisher(IConfiguration configuration, ILogger<RabbitMqEventPublisher> logger)
    {
        _configuration = configuration;
        _logger = logger;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        try
        {
            var connectionString = _configuration["RabbitMq:ConnectionString"];
            var factory = string.IsNullOrWhiteSpace(connectionString)
                ? new ConnectionFactory
                {
                    HostName = _configuration["RabbitMq:Host"] ?? "localhost",
                    Port = int.Parse(_configuration["RabbitMq:Port"] ?? "5672"),
                    UserName = _configuration["RabbitMq:Username"] ?? "guest",
                    Password = _configuration["RabbitMq:Password"] ?? "guest"
                }
                : new ConnectionFactory { Uri = new Uri(connectionString) };

            _connection = await factory.CreateConnectionAsync(cancellationToken);
            _channel = await _connection.CreateChannelAsync(null, cancellationToken);
            await _channel.ExchangeDeclareAsync("bingehos.events", ExchangeType.Topic, durable: true, cancellationToken: cancellationToken);
            _logger.LogInformation("Connected to RabbitMQ");
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to connect to RabbitMQ");
            throw;
        }
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        Dispose();
        return Task.CompletedTask;
    }

    public async Task Publish<TEvent>(TEvent @event, CancellationToken ct = default) where TEvent : notnull
    {
        if (_channel == null)
            throw new EventPublishingException(
                $"RabbitMQ channel is unavailable. Event {typeof(TEvent).Name} was not published.");

        try
        {
            var body = System.Text.Json.JsonSerializer.SerializeToUtf8Bytes(@event);
            var routingKey = typeof(TEvent).Name;

            await _channel.BasicPublishAsync(exchange: "bingehos.events", routingKey: routingKey, mandatory: false, body: body, cancellationToken: ct);
            _logger.LogInformation("Published event {Event}", typeof(TEvent).Name);
        }
        catch (OperationCanceledException) when (ct.IsCancellationRequested)
        {
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to publish event {Event}", typeof(TEvent).Name);
            throw;
        }
    }

    public void Dispose()
    {
        _channel?.Dispose();
        _connection?.Dispose();
    }
}

public sealed class EventPublishingException : Exception
{
    public EventPublishingException(string message) : base(message)
    {
    }
}
