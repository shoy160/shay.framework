using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shay.Core.Helper;
using Shay.Core.Logging;
using Shay.Framework;
using System.Reflection;
using Shay.Core.Extensions;

namespace Shay.Core.Tests
{
    [TestClass]
    public class HelperTest : DTest
    {
        private readonly ILogger _logger = LogManager.Logger<HelperTest>();

        public HelperTest()
            : base(Assembly.GetExecutingAssembly())
        {            
        }

        [TestMethod]
        public void Md5Test()
        {
            var md5 = EncryptHelper.MD5("shay");
            _logger.Info(md5);
            _logger.Info("shay".Config(string.Empty));
            Print(md5);
        }
    }
}
