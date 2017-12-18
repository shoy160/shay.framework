using Microsoft.AspNetCore.Mvc;
using Shay.Core.Extensions;
using Shay.Core.Helper;
using System.Collections.Generic;

namespace Shay.Web.Tests.Controllers
{
    [Route("api/[controller]")]
    public class ValuesController : Controller
    {
        // GET api/values
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
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
            return new { shay = str, uri };
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
