using Shay.Core;
using Shay.Core.Cache;
using Shay.Core.Modules;

namespace Shay.Redis
{
    [DependsOn(typeof(CoreModule))]
    public class RedisModule : DModule
    {
        public override void Initialize()
        {
            CacheManager.SetProvider(CacheLevel.Second, new RedisCacheProvider());
            base.Initialize();
        }

        public override void Shutdown()
        {
            RedisManager.Instance.Dispose();
            base.Shutdown();
        }
    }
}
