using System;

namespace Shay.Core.Dependency
{
    public interface IIocManager : ILifetimeDependency
    {
        T Resolve<T>();
        object Resolve(Type type);
    }
}
