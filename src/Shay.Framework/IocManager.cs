﻿using Autofac;
using Shay.Core.Dependency;
using System;

namespace Shay.Framework
{
    public class IocManager : IIocManager
    {
        private readonly DBootstrap _bootstrap;

        public IocManager()
        {
            _bootstrap = DBootstrap.Instance;
        }
        public T Resolve<T>()
        {
            return _bootstrap.Container.Resolve<T>();
        }

        public object Resolve(Type type)
        {
            return _bootstrap.Container.Resolve(type);
        }
    }
}
