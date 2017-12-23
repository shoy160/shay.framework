using Microsoft.AspNetCore.Mvc;
using Shay.Core.Cache;
using Shay.Core.Extensions;
using Shay.Core.Helper;
using Shay.Core.Logging;
using System;
using System.Collections.Generic;

namespace Shay.Web.Tests.Controllers
{
    [Route("api/[controller]")]
    public class ValuesController : Controller
    {
        private readonly ILogger _logger = LogManager.Logger(typeof(ValuesController));
        private readonly ICache _cache = CacheManager.GetCacher(typeof(ValuesController));

        [HttpGet, Route("~/")]
        public IActionResult Home()
        {
            return RedirectToAction("Get", "Values");
        }

        // GET api/values
        [HttpGet]
        public IEnumerable<string> Get()
        {
            _logger.Info("test");
            var set = "set".Query(false);
            if (set)
            {
                _cache.Set("test", new string[] { "value1", "value2" }, TimeSpan.FromSeconds(50));
            }
            var list = _cache.Get<List<string>>("test");
            _logger.Debug(list);
            return list ?? new List<string>();
        }

        [HttpGet, Route("config/{value}")]
        public string Config(string value)
        {
            return value.Config(string.Empty);
        }

        [HttpGet, Route("qs")]
        public object Query()
        {
            var str = "shay".Query(string.Empty);
            var uri = "test".SetQuery("nice");
            var context = Shay.Core.Web.HttpContext.Current;
            return new
            {
                shay = str,
                uri,
                headers = HttpContext.Request.Headers,
                localIp = Core.Web.HttpContext.LocalIpAddress,
                remoteIp = Core.Web.HttpContext.RemoteIpAddress
            };
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            var md5 = EncryptHelper.Hash($"value:{id}", EncryptHelper.HashFormat.SHA256);
            //return md5;
            return string.Concat(md5, ",", $"value:{id}".Md5());
        }

        // POST api/values
        [HttpPost]
        public void Post([FromBody]string value)
        {
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
