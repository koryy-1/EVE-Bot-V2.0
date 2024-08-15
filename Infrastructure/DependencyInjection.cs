using Application.Interfaces.ApiClients;
using Infrastructure.ApiClients;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApiClients(this IServiceCollection services, string baseUri)
        {
            services.AddHttpClient<IGameApiClient, GameApiClient>(client =>
            {
                client.BaseAddress = new Uri(baseUri);
            });
            services.AddHttpClient<IDroneApiClient, DroneApiClient>(client =>
            {
                client.BaseAddress = new Uri(baseUri);
            });
            services.AddHttpClient<IOverviewApiClient, OverviewApiClient>(client =>
            {
                client.BaseAddress = new Uri(baseUri);
            });
            services.AddHttpClient<ISelectItemApiClient, SelectItemApiClient>(client =>
            {
                client.BaseAddress = new Uri(baseUri);
            });
            services.AddHttpClient<IInfoPanelApiClient, InfoPanelApiClient>(client =>
            {
                client.BaseAddress = new Uri(baseUri);
            });
            services.AddHttpClient<IHudInterfaceApiClient, HudInterfaceApiClient>(client =>
            {
                client.BaseAddress = new Uri(baseUri);
            });
            services.AddHttpClient<IInventoryApiClient, InventoryApiClient>(client =>
            {
                client.BaseAddress = new Uri(baseUri);
            });
            services.AddHttpClient<IProbeScannerApiClient, ProbeScannerApiClient>(client =>
            {
                client.BaseAddress = new Uri(baseUri);
            });

            return services;
        }
    }
}
