using Autofac;
using TK.MongoDB.Data;
using TK.MongoDB.Interfaces;

namespace TK.MongoDB.Test
{
    public class BaseTest
    {
        private IContainer autofacContainer;
        protected IContainer AutofacContainer
        {
            get
            {
                if (autofacContainer == null)
                {
                    var builder = new ContainerBuilder();

                    builder.RegisterGeneric(typeof(Repository<>))
                        .As(typeof(IRepository<>))
                        .InstancePerLifetimeScope();

                    builder.RegisterGeneric(typeof(OverrideRepository<>))
                        .As(typeof(IRepository<>))
                        .InstancePerLifetimeScope();

                    //Register if Dependency Tracking is required
                    builder.RegisterType(typeof(DependencyTracker))
                        .As(typeof(IDependencyTracker))
                        .InstancePerLifetimeScope();

                    var container = builder.Build();
                    autofacContainer = container;
                }

                return autofacContainer;
            }
        }

        protected IRepository<Activity> ActivityRepository
        {
            get
            {
                return AutofacContainer.Resolve<IRepository<Activity>>();
            }
        }
    }
}
