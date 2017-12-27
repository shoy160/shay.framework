using Autofac;
using Shay.Core;
using Shay.Core.Cache;
using Shay.Core.Dependency;
using Shay.Core.Logging;
using Shay.Framework.Logging;
using System;
using System.Linq;
using System.Reflection;

namespace Shay.Framework
{
    public class DBootstrap : Bootstrap
    {
        private DBootstrap() { }

        public static DBootstrap Instance
            => Singleton<DBootstrap>.Instance ??
               (Singleton<DBootstrap>.Instance = new DBootstrap());

        public ContainerBuilder Builder { get; private set; }

        private IContainer _container;

        public IContainer Container => _container ?? (_container = Builder.Build());

        public delegate void BuilderAction(ContainerBuilder builderAction);

        public event BuilderAction BuilderHandler;

        public override void Initialize(Assembly executingAssembly = null)
        {
            LoggerInit();
            CacheInit();

            if (executingAssembly == null)
                executingAssembly = Assembly.GetExecutingAssembly();
            IocRegisters(executingAssembly);
            BuilderHandler?.Invoke(Builder);
            _container = Builder.Build();
            IocManager = _container.Resolve<IIocManager>();
            DatabaseInit();
            ModulesInstaller();
            //AppDomain.CurrentDomain.UnhandledException += (sender, e) =>
            //{
            //    var ex = (Exception)e.ExceptionObject;
            //    LogManager.Logger(typeof(DBootstrap)).Error(ex.Message, ex);
            //};
        }

        public override void IocRegisters(Assembly executingAssembly)
        {
            Builder = new ContainerBuilder();

            var assemblies = DAssemblyFinder.Instance.FindAll().Union(new[] { executingAssembly }).ToArray();
            Builder.RegisterAssemblyTypes(assemblies)
                .Where(type => typeof(ILifetimeDependency).IsAssignableFrom(type) && !type.IsAbstract)
                .AsSelf() //自身服务，用于没有接口的类
                .AsImplementedInterfaces() //接口服务
                .PropertiesAutowired()//属性注入
                .InstancePerLifetimeScope(); //保证生命周期基于请求

            Builder.RegisterAssemblyTypes(assemblies)
                .Where(type => typeof(IDependency).IsAssignableFrom(type) && !type.IsAbstract)
                .AsSelf() //自身服务，用于没有接口的类
                .AsImplementedInterfaces() //接口服务
                .PropertiesAutowired(); //属性注入

            Builder.RegisterAssemblyTypes(assemblies)
                .Where(type => typeof(ISingleDependency).IsAssignableFrom(type) && !type.IsAbstract)
                .AsSelf() //自身服务，用于没有接口的类
                .AsImplementedInterfaces() //接口服务
                .PropertiesAutowired()//属性注入
                .SingleInstance(); //保证单例注入
        }

        public override void CacheInit()
        {
            CacheManager.SetProvider(CacheLevel.First, new RuntimeMemoryCacheProvider());
        }

        public override void LoggerInit()
        {
            LogManager.AddAdapter(new Log4NetAdapter());
        }

        public override void DatabaseInit()
        {
        }
    }
}
