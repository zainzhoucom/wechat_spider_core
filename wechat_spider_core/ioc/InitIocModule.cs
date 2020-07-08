using Autofac;
using wechat_spider_core.ef;

namespace wechat_spider_core.ioc
{
    public static class InitIocModule
    {
        private static IContainer container = null;

        private static object lock_obj = new object();

        public static IContainer GetContainer()
        {
            if(null == container)
            {
                lock(lock_obj)
                {
                    if(null == container)
                    {
                        var builder = new ContainerBuilder();
                        builder.RegisterType<ISpiderHandler>();
                        builder.RegisterType<SpiderContext>();
                        container = builder.Build();
                    }
                }
            }

            return container;
        }

    }
}
