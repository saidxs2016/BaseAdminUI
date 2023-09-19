using Autofac;

namespace Application.DependencyResolver.Autofac;

public class AutofacBusinessModule : Module
{
    protected override void Load(ContainerBuilder container)
    {

        //container.RegisterType<PersonRepository>().As<IPersonRepository>().InstancePerLifetimeScope(); // singleton == SingleInstance, Scoped == InstancePerLifetimeScope, transiit == InstancePerDependency
        //container.RegisterType<DepartmentRefEditRepository>().As<IDepartmentRefEditRepository>().InstancePerLifetimeScope();

        // ============ Register Mediator ============
        //container.RegisterType<Mediator>().As<IMediator>().InstancePerLifetimeScope();
        //container.RegisterMediatR(assembly);

        // ============ Register Auto Mapper ============
        //var config = new MapperConfiguration(cfg =>
        //{
        //    cfg.AddProfile(new GlobalMapper());
        //});
        //var mapper = config.CreateMapper();
        //container.RegisterInstance<IMapper>(mapper).SingleInstance();
        //container.RegisterAutoMapper(assembly);




        // ============ Register AspectInterceptor ============ 
        //var assembly = Assembly.GetExecutingAssembly();
        //container.RegisterAssemblyTypes(assembly).AsImplementedInterfaces()
        //    .EnableInterfaceInterceptors(new ProxyGenerationOptions()
        //    {
        //        Selector = new AspectInterceptorSelector()
        //    }).SingleInstance();
    }
}
