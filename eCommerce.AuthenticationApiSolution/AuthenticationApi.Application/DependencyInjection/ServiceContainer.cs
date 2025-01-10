using AuthenticationApi.Application.Consumers;
using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace AuthenticationApi.Application.DependencyInjection
{
    public static class ServiceContainer
    {
        public static IServiceCollection AddApplicationService(this IServiceCollection services, IConfiguration config)
        {
            services.AddMassTransit(x =>
            {
                x.AddConsumer<GetUserRequestConsumer>();

                x.UsingRabbitMq((context, cfg) =>
                {
                    cfg.Host(config["RabbitMQ:HostName"], h =>
                    {
                        h.Username(config["RabbitMQ:UserName"]);
                        h.Password(config["RabbitMQ:Password"]);
                    });

                    cfg.ReceiveEndpoint("get-user-queue", e =>
                    {
                        e.ConfigureConsumer<GetUserRequestConsumer>(context);
                    });
                });
            });

            return services;
        }
    }
}
