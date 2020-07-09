using Autofac;
using System.Reflection;

namespace wechat_spider_core.ioc
{
    public static class InitIocModule
    {
        private static IContainer container = null;

        private readonly static object lock_obj = new object();

        public static void Init()
        {
            if (null == container)
            {
                lock (lock_obj)
                {
                    if (null == container)
                    {
                        var builder = new ContainerBuilder();
                        builder.RegisterAssemblyTypes(Assembly.GetExecutingAssembly()).AsImplementedInterfaces().AsSelf();

                        container = builder.Build();
                    }
                }
            }
        }
        public static T GetFromFac<T>()
        {
            if(null == container)
            {
                lock(lock_obj)
                {
                    Init();
                }
            }

            return container.Resolve<T>();
        }

    }
}
