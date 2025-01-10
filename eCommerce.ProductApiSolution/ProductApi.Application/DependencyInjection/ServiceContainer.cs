using eCommerce.SharedLibrary.DTOs.Requests;
using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ProductApi.Application.Consumers;

namespace ProductApi.Application.DependencyInjection
{
    public static class ServiceContainer
    {
        public static IServiceCollection AddApplicationService(this IServiceCollection services, IConfiguration config)
        {
            services.AddMassTransit(x =>
            {
                x.AddConsumer<GetProductRequestConsumer>();
   
                x.UsingRabbitMq((context, cfg) =>
                {
                    cfg.Host(config["RabbitMQ:HostName"], h =>
                    {
                        h.Username(config["RabbitMQ:UserName"]);
                        h.Password(config["RabbitMQ:Password"]);
                    });

                    cfg.ReceiveEndpoint("get-product-queue", e =>
                    {
                        e.ConfigureConsumer<GetProductRequestConsumer>(context);
                    });
                });
            });

            return services;
        }
    }
}
