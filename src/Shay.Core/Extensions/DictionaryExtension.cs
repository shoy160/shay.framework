﻿using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;

namespace Shay.Core.Extensions
{
    public static class DictionaryExtension
    {
        /// <summary>
        /// 添加或更新
        /// </summary>
        /// <param name="dict"></param>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public static void AddOrUpdate(this IDictionary<string, object> dict, string key, object value)
        {
            if (string.IsNullOrWhiteSpace(key))
                return;
            if (dict.ContainsKey(key))
            {
                dict[key] = value;
            }
            else
            {
                dict.Add(key, value);
            }
        }

        /// <summary>
        /// 获取值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dict"></param>
        /// <param name="key"></param>
        /// <param name="def"></param>
        /// <returns></returns>
        public static T GetValue<T>(this IDictionary<string, object> dict, string key, T def = default(T))
        {
            if (dict.TryGetValue(key, out object value) && value != null)
            {
                return value.CastTo(def);
            }
            return def;
        }

        /// <summary>
        /// Url格式
        /// </summary>
        /// <param name="dict"></param>
        /// <param name="encode"></param>
        /// <returns></returns>
        public static string ToUrl(this IDictionary<string, object> dict, bool encode = true)
        {
            var sb = new StringBuilder();
            foreach (var item in dict)
            {
                string value = item.Value.ToString();
                sb.AppendFormat("{0}={1}&", item.Key, encode ? value.UrlEncode() : value);
            }
            return sb.ToString().TrimEnd('&');
        }

        /// <summary>
        /// 将Url格式数据转换为字典
        /// </summary>
        /// <param name="url">url数据</param>
        /// <param name="decode">是否需要url解码</param>
        /// <returns></returns>
        public static void FromUrl(this IDictionary<string, object> dict, string url, bool decode = true, bool clear = true)
        {
            if (clear)
            {
                dict.Clear();
            }
            if (string.IsNullOrEmpty(url))
            {
                return;
            }
            int index = url.IndexOf('?');

            if (index == 0)
            {
                url = url.Substring(index + 1);
            }

            var regex = new Regex(@"(^|&)?(\w+)=([^&]+)(&|$)?", RegexOptions.Compiled);
            var mc = regex.Matches(url);

            foreach (Match item in mc)
            {
                string value = item.Result("$3");

                dict.AddOrUpdate(item.Result("$2"), decode ? value.UrlEncode() : value);
            }
        }

        /// <summary>
        /// 将网关数据转成Xml格式数据
        /// </summary>
        /// <returns></returns>
        public static string ToXml(this IDictionary<string, object> dict)
        {
            if (dict.Count == 0)
            {
                return string.Empty;
            }

            var sb = new StringBuilder();
            sb.Append("<xml>");
            foreach (var item in dict)
            {
                if (item.Value is string)
                {
                    sb.AppendFormat("<{0}><![CDATA[{1}]]></{0}>", item.Key, item.Value);

                }
                else
                {
                    sb.AppendFormat("<{0}>{1}</{0}>", item.Key, item.Value);
                }
            }
            sb.Append("</xml>");

            return sb.ToString();
        }

        /// <summary>
        /// 将Xml格式数据转换为字典
        /// </summary>
        /// <param name="xml">Xml数据</param>
        /// <returns></returns>
        public static void FromXml(this IDictionary<string, object> dict, string xml, bool clear = true)
        {
            if (clear)
            {
                dict.Clear();
            }
            if (string.IsNullOrEmpty(xml))
                return;
            var xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(xml);
            var xmlNode = xmlDoc.FirstChild;
            var nodes = xmlNode.ChildNodes;
            foreach (var item in nodes)
            {
                var xe = (XmlElement)item;
                dict.AddOrUpdate(xe.Name, xe.InnerText);
            }
        }

        /// <summary>
        /// 将键值对转换为字典
        /// </summary>
        /// <param name="dict"></param>
        /// <param name="collection"></param>
        public static void FromNameValueCollection(this IDictionary<string, object> dict, NameValueCollection collection)
        {
            foreach (var item in collection.AllKeys)
            {
                dict.AddOrUpdate(item, collection[item]);
            }
        }

        /// <summary>
        /// 转换为表单数据
        /// </summary>
        /// <param name="url">请求地址</param>
        /// <returns></returns>
        public static string ToForm(this IDictionary<string, object> dict, string url)
        {
            var html = new StringBuilder();
            html.AppendLine("<body>");
            html.AppendLine($"<form name='gateway' method='post' action ='{url}'>");
            foreach (var item in dict)
            {
                html.AppendLine($"<input type='hidden' name='{item.Key}' value='{item.Value}'>");
            }
            html.AppendLine("</form>");
            html.AppendLine("<script language='javascript' type='text/javascript'>");
            html.AppendLine("document.gateway.submit();");
            html.AppendLine("</script>");
            html.AppendLine("</body>");

            return html.ToString();
        }

        /// <summary>
        /// 转成Json格式数据
        /// </summary>
        /// <returns></returns>
        public static string ToJson(this IDictionary<string, object> dict)
        {
            return JsonConvert.SerializeObject(dict);
        }

        /// <summary>
        /// 将Json格式数据转成网关数据
        /// </summary>
        /// <param name="json">json数据</param>
        /// <returns></returns>
        public static void FromJson(this IDictionary<string, object> dict, string json, bool clear = true)
        {
            if (clear)
                dict.Clear();
            if (!string.IsNullOrEmpty(json))
            {
                var jObject = JObject.Parse(json);
                var list = jObject.Children().OfType<JProperty>();
                foreach (var item in list)
                {
                    dict.AddOrUpdate(item.Name, item.Value.ToString());
                }
            }
        }
    }
}
