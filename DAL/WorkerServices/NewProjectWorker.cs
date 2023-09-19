using DAL.MainDB.Entities;
using DAL.MainDB.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Linq.Expressions;
using System.Text.Json;

namespace DAL.WorkerServices;

public class NewProjectWorker : BackgroundService
{
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private readonly ILogger<InitDatabaseWorker> _logger;
    public NewProjectWorker(IServiceScopeFactory serviceScopeFactory, ILogger<InitDatabaseWorker> logger)
    {
        _serviceScopeFactory = serviceScopeFactory;
        _logger = logger;
    }

    protected override Task ExecuteAsync(CancellationToken token)
    {
        try
        {
            // Süper admine yada geliştiriciye sormadan burayaı çalıştırmayınız.
            // bu worker sadece 1 kere çalışacak.
            // yeni projeye geçiş sürecinde olan ana tablolarda uid Değişikliği gerçekleştirmektedir.
            token.ThrowIfCancellationRequested();
            //Task.Run(async () => await DoWork(token), token);
        }
        catch (Exception) { }

        return Task.CompletedTask;

    }

    private async Task DoWork(CancellationToken token)
    {
        try
        {
            await using var scope = _serviceScopeFactory.CreateAsyncScope();
            var _uow = scope.ServiceProvider.GetService<IUnitOfWork>();
            var db = _uow.DbContext;


            var super_admin_role = await db.Roles.FirstOrDefaultAsync(i => i.ParentUid == null, token);

            Expression<Func<Role, bool>> predicate1 = i => i.Uid != super_admin_role.Uid;
            _ = await db.Roles.Where(predicate1).ExecuteDeleteAsync(token);

            Expression<Func<Permission, bool>> predicate2 = i => i.RoleUid != super_admin_role.Uid;
            _ = await db.Permissions.Where(predicate2).ExecuteDeleteAsync(token);

            Expression<Func<Admin, bool>> predicate3 = i => i.RoleUid != super_admin_role.Uid;
            _ = await db.Admins.Where(predicate3).ExecuteDeleteAsync(token);

            var old_roles = await db.Roles.ToListAsync(token);
            var old_admins = await db.Admins.ToListAsync(token);
            var old_modules = await db.Modules.ToListAsync(token);
            var old_permissions = await db.Permissions.ToListAsync(token);


            foreach (var item in old_roles)
            {
                var new_guid = Guid.NewGuid();
                Expression<Func<Role, bool>> predicate4 = i => i.Uid == item.Uid;
                Expression<Func<SetPropertyCalls<Role>, SetPropertyCalls<Role>>> props1 = i => i
                    .SetProperty(_ => _.Uid, new_guid)
                    .SetProperty(_ => _.AddDate, DateTime.Now)
                    .SetProperty(_ => _.UpdateDate, DateTime.Now);
                _ = await db.Roles.Where(predicate4).ExecuteUpdateAsync(props1, token);
            }
            var new_super_admin_role = await db.Roles.FirstOrDefaultAsync(token);

            foreach (var item in old_admins)
            {
                var connection_keys = JsonSerializer.Serialize(new List<object>() { });
                var new_guid = Guid.NewGuid();
                Expression<Func<Admin, bool>> predicate5 = i => i.Uid == item.Uid;
                Expression<Func<SetPropertyCalls<Admin>, SetPropertyCalls<Admin>>> props2 = i => i
                    .SetProperty(_ => _.Uid, new_guid)
                    .SetProperty(_ => _.RoleUid, new_super_admin_role.Uid)
                    .SetProperty(_ => _.AddDate, DateTime.Now)
                    .SetProperty(_ => _.UpdateDate, DateTime.Now)
                    .SetProperty(_ => _.ConnectionKeys, connection_keys);
                _ = await db.Admins.Where(predicate5).ExecuteUpdateAsync(props2, token);
            }
            var new_super_admin = await db.Admins.FirstOrDefaultAsync(token);

            foreach (var item in old_permissions)
            {
                var ignored = JsonSerializer.Serialize(new List<string>() { });
                Guid new_guid = Guid.NewGuid();
                Expression<Func<Permission, bool>> predicate6 = i => i.Uid == item.Uid;
                Expression<Func<SetPropertyCalls<Permission>, SetPropertyCalls<Permission>>> props3 = i => i
                    .SetProperty(_ => _.RoleUid, new_super_admin_role.Uid)
                    .SetProperty(_ => _.ByAdmin, new_super_admin.Uid)
                    .SetProperty(_ => _.Uid, new_guid)
                    .SetProperty(_ => _.AddDate, DateTime.Now)
                    .SetProperty(_ => _.UpdateDate, DateTime.Now)
                    .SetProperty(_ => _.IgnoredSections, ignored);
                _ = await db.Permissions.Where(predicate6).ExecuteUpdateAsync(props3, token);
            }

            foreach (var item in old_modules)
            {
                Guid new_guid = Guid.NewGuid();

                Expression<Func<Permission, bool>> predicate7 = i => i.ModuleUid == item.Uid;
                Expression<Func<SetPropertyCalls<Permission>, SetPropertyCalls<Permission>>> props4 = i => i
                    .SetProperty(_ => _.ModuleUid, new_guid);
                _ = await db.Permissions.Where(predicate7).ExecuteUpdateAsync(props4, token);

                Expression<Func<Module, bool>> predicate8 = i => i.ParentUid == item.Uid;
                Expression<Func<SetPropertyCalls<Module>, SetPropertyCalls<Module>>> props5 = i => i
                    .SetProperty(_ => _.ParentUid, new_guid);
                _ = await db.Modules.Where(predicate8).ExecuteUpdateAsync(props5, token);

                Expression<Func<Module, bool>> predicate9 = i => i.Uid == item.Uid;
                Expression<Func<SetPropertyCalls<Module>, SetPropertyCalls<Module>>> props6 = i => i
                    .SetProperty(_ => _.Uid, new_guid)
                    .SetProperty(_ => _.AddDate, DateTime.Now)
                    .SetProperty(_ => _.UpdateDate, DateTime.Now);
                _ = await db.Modules.Where(predicate9).ExecuteUpdateAsync(props6, token);
            }


        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
        }


        //return Task.CompletedTask;

    }
}
