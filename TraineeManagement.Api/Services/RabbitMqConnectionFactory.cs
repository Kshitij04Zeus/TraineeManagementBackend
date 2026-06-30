using TraineeManagement.Api.DTO;
using TraineeManagement.Api.Models;
using Microsoft.EntityFrameworkCore;
using RabbitMQ.Client;
using TraineeManagement.Api.Data;
using Microsoft.Extensions.Options;
using TraineeManagement.Api.Utilities;

namespace TraineeManagement.Api.Services;

public class RabbitMQConnectionFactory : IRabbitMqConnectionFactory
{
    private readonly RabbitMqSettings _settings;
    public RabbitMQConnectionFactory(IOptions<RabbitMqSettings> options)
    {
        _settings=options.Value;
    }
    public async Task<IConnection> CreateConnectAsync()
    {
        var factory=new ConnectionFactory
        {
            HostName = _settings.Host,  
            Port=_settings.Port,              
            VirtualHost = _settings.VirtualHost,
            UserName = _settings.UserName,
            Password = _settings.Password
        };
        return await factory.CreateConnectionAsync();
    }
}