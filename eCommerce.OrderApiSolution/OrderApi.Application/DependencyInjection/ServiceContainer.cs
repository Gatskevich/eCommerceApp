﻿using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OrderApi.Application.Services;

namespace OrderApi.Application.DependencyInjection
{
    public static class ServiceContainer
    {
        public static IServiceCollection AddApplicationService(this IServiceCollection services, IConfiguration config)
        {
            services.AddMassTransit(x =>
            {
                x.UsingRabbitMq((context, cfg) =>
                {
                    cfg.AutoStart = true;

                    cfg.Host(config["RabbitMQ:HostName"], h =>
                    {
                        h.Username(config["RabbitMQ:UserName"]);
                        h.Password(config["RabbitMQ:Password"]);
                    });

                    cfg.UseMessageRetry(r =>
                    {
                        r.Interval(3, TimeSpan.FromSeconds(5));
                    });
                });
            });

            services.AddScoped<IOrderService, OrderService>();

            return services;
        }
    }
}
