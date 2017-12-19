using Shay.Core;
using Shay.Core.Extensions;
using Shay.Core.Reflection;
using System;

namespace Shay.Framework
{
    public class DAssemblyFinder : DefaultAssemblyFinder
    {
        private const string FrameworkName = "Shay.";

        private static readonly Func<string, bool> AssemblyFinder =
            t =>
            {
                var frame = t.StartsWith(FrameworkName, StringComparison.CurrentCultureIgnoreCase);
                if (frame)
                    return true;
                var baseAssemblyName = "baseAssembly".Config(string.Empty);
                return !string.IsNullOrWhiteSpace(baseAssemblyName) && t.StartsWith(baseAssemblyName);
            };

        public DAssemblyFinder()
            : base(ass => AssemblyFinder.Invoke(ass.FullName), AssemblyFinder)
        {
        }

        public static DAssemblyFinder Instance => Singleton<DAssemblyFinder>.Instance ??
                                                       (Singleton<DAssemblyFinder>.Instance = new DAssemblyFinder());
    }
}
