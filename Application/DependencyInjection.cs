using Application.Interfaces;
using Application.Services;
using Application.Strategies;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddBotServices(this IServiceCollection services)
        {
            services.AddSingleton<IBotService, BotService>();
            services.AddSingleton<ICoordinator, Coordinator>();
            services.AddSingleton<IGameService, GameService>();

            return services;
        }

        public static IServiceCollection AddBotWorkers(this IServiceCollection services)
        {
            services.AddHostedService<CmdExecChecker>();
            services.AddHostedService<CombatService>();
            services.AddHostedService<NavigationService>();
            services.AddHostedService<TargetService>();
            services.AddHostedService<DroneService>();
            services.AddHostedService<MonitoringService>();
            services.AddHostedService<LootingService>();
            services.AddHostedService<Executor>();

            return services;
        }

        public static IServiceCollection AddBotStrategies(this IServiceCollection services)
        {
            services.AddSingleton<Autopilot>();
            services.AddSingleton<FarmingStrategy>();
            services.AddSingleton<DestroyerStrategy>();

            return services;
        }
    }
}
