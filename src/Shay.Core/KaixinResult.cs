using System;
using System.Collections.Generic;
using System.Linq;

namespace Shay.Core
{
    /// <summary> 基础数据结果类 </summary>
    [Serializable]
    public class KaixinResult
    {
        public bool Status { get; set; }

        public int Code { get; set; }

        public string Message { get; set; }

        public KaixinResult(bool status, string message, int code = 0)
        {
            Status = status;
            Message = message;
            Code = code;
        }

        public KaixinResult(string message, int code = -1)
            : this(false, message, code)
        {
        }

        public static KaixinResult Success
        {
            get { return new KaixinResult(true, string.Empty); }
        }

        public static KaixinResult Error(string message, int code = -1)
        {
            return new KaixinResult(false, message, code);
        }

        public static KaixinResult<T> Succ<T>(T data)
        {
            return new KaixinResult<T>(true, data);
        }

        public static KaixinResult<T> Error<T>(string message, int code = -1)
        {
            return new KaixinResult<T>(message, code);
        }

        public static KaixinResults<T> Succ<T>(IEnumerable<T> data, int count = -1)
        {
            return count < 0 ? new KaixinResults<T>(data) : new KaixinResults<T>(data, count);
        }

        public static KaixinResults<T> Errors<T>(string message, int code = -1)
        {
            return new KaixinResults<T>(message, code);
        }
    }

    [Serializable]
    public class KaixinResult<T> : KaixinResult
    {
        public T Data { get; set; }

        public KaixinResult(bool status, T data, int code = 0)
            : base(status, string.Empty, code)
        {
            Data = data;
        }

        public KaixinResult(string message, int code = -1)
            : base(false, message, code)
        {
        }
    }

    [Serializable]
    public class KaixinResults<T> : KaixinResult
    {
        public IEnumerable<T> Data { get; set; }

        public int TotalCount { get; set; }

        public KaixinResults(string message, int code = -1)
            : base(false, message, code)
        {
        }

        public KaixinResults(IEnumerable<T> list)
            : base(true, string.Empty)
        {
            var data = list as T[] ?? list.ToArray();
            Data = data;
            TotalCount = data.Length;
        }

        public KaixinResults(IEnumerable<T> list, int totalCount)
            : base(true, string.Empty)
        {
            Data = list;
            TotalCount = totalCount;
        }
    }
}