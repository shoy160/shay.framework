using Shay.Core.Serialize;
using System;

namespace Shay.Payment.Attributes
{
    /// <summary>
    /// 字段属性名
    /// </summary>
    public class PropNameAttribute : Attribute
    {
        public string Name { get; }
        public bool Ignore { get; }
        public NamingType NamingType { get; }

        public PropNameAttribute(string name)
        {
            Name = name;
        }

        public PropNameAttribute(bool ignore)
        {
            Ignore = ignore;
        }

        public PropNameAttribute(NamingType namingType)
        {
            NamingType = namingType;
        }
    }
}
