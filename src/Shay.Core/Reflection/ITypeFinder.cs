using Shay.Core.Dependency;
using System;

namespace Shay.Core.Reflection
{
    /// <summary> 类型查找器 </summary>
    public interface ITypeFinder : ILifetimeDependency
    {
        Type[] Find(Func<Type, bool> expression);

        Type[] FindAll();
    }
}
