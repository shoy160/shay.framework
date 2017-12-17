using Shay.Core.Dependency;
using Shay.Core.Modules;

namespace Shay.Core
{
    /// <summary> 初始化模块 </summary>
    public class CoreModule : DModule
    {
        public override void Initialize()
        {
            CurrentIocManager.IocManager = IocManager;
            base.Initialize();
        }
    }
}
