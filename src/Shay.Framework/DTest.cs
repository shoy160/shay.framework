using Shay.Core.Logging;
using Shay.Core.Serialize;
using Shay.Framework.Logging;
using System;
using System.Reflection;

namespace Shay.Framework
{
    public abstract class DTest
    {
        protected DBootstrap Bootstrap;

        protected DTest(Assembly executingAssembly)
        {
            Bootstrap = DBootstrap.Instance;
            Bootstrap.Initialize(executingAssembly);
            LogManager.ClearAdapter();
            LogManager.AddAdapter(new ConsoleAdapter());
        }

        protected void Print<T>(T result)
        {
            var type = typeof(T);
            if (type.IsValueType || type == typeof(string))
                Console.WriteLine(result);
            else
                Console.WriteLine(JsonHelper.ToJson(result, NamingType.CamelCase, true));

        }
    }
}
