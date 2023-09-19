using DAL.MainDB.Context;
using DAL.MainDB.Repositories.Concretes;
using DAL.MainDB.Repositories.Interfaces;
using DAL.MainSysDB.Context;
using DAL.WorkerServices;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DAL;
public static class DalRegistration
{
    /*
     * 
     * 
     * */
    public static IServiceCollection AddDalServices(this IServiceCollection services, IConfiguration configuration)
    {

        // ======== Add Mediatr Service ========
        services.AddMediatR(configuration =>
        {
            configuration.RegisterServicesFromAssembly(typeof(DalRegistration).Assembly);
        });

        // ======== Add Repository Service For DB1 ========
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<IAdminRepository, AdminRepository>();
        services.AddScoped<IModuleRepository, ModuleRepository>();
        services.AddScoped<IPermissionRepository, PermissionRepository>();
        services.AddScoped<IRoleRepository, RoleRepository>();


        // ======== Add Hosted Services ========
        //services.AddHostedService<NewProjectWorker>();
        services.AddHostedService<InitDatabaseWorker>();
        services.AddHostedService<InitDeletedRecordTableWorker>();

        // ======== Add Interceptor For DB1 ========
        services.AddSingleton<TrackeRecordsInterceptor>();

        // ======== Init DB1(MainDB) ========
        services.AddDbContext<MDbContext>((sp, options) =>
        {
            options.UseNpgsql(configuration["ConnectionStrings:MDbContext"]);
            var interceptor = sp.GetService<TrackeRecordsInterceptor>();
            options.AddInterceptors(interceptor);
        }, ServiceLifetime.Scoped);

        // ======== Init DB2(MainSysDB) ========
        services.AddDbContext<SysDbContext>(options =>
        {
            options.UseNpgsql(configuration["ConnectionStrings:SysDbContext"]);
        }, ServiceLifetime.Singleton);


        return services;
    }
}
