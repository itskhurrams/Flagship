using Flagship.Application.Interfaces;
using Flagship.Application.Services;
using Flagship.Core.Interfaces;
using Flagship.Infrastructure.Caching;
using Flagship.Infrastructure.Logging.Applogger;
using Flagship.Infrastructure.Persistance.Repositories;
using Microsoft.Extensions.DependencyInjection;

namespace Flagship.Infrastructure.Extension.Container {
    public class DependencyContainer {
        public static void RegisterServices(IServiceCollection services) {
            ServicesRegistration(services);
            RepositoryRegistration(services);
            ApploggerRegistration(services);
        }
        private static void ServicesRegistration(IServiceCollection services) {
            services.AddTransient<IUserService, UserService>();
            services.AddTransient<ILoginLogService, LoginLogService>();
            services.AddTransient<ITerritoryService, TerritoryService>();
        }
        private static void RepositoryRegistration(IServiceCollection services) {
            services.AddTransient<IBaseRepository, BaseRepository>();
            services.AddTransient<IUserRepository, UserRepository>();
            services.AddTransient<ILoginLogRepository, LoginLogRepository>();
            services.AddTransient<ITerritoryRepository, TerritoryRepository>();
            services.AddScoped<IMemoryCacheProvider, MemoryCacheProvider>();
        }

        private static void ApploggerRegistration(IServiceCollection services) {
            services.AddSingleton<IApplogger, Applogger>();
        }
    }
}
