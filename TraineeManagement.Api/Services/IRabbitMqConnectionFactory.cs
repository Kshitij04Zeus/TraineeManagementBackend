using TraineeManagement.Api.DTO;
using TraineeManagement.Api.Models;
using RabbitMQ.Client;
using Microsoft.AspNetCore.Connections;
using System.Threading;      
using System.Threading.Tasks;

namespace TraineeManagement.Api.Services;
public interface IRabbitMqConnectionFactory
{
    Task<IConnection> CreateConnectAsync();
}
