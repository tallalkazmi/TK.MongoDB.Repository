using Autofac;
using TK.MongoDB.Data;

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
