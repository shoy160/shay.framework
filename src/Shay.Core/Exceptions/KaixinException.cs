namespace Shay.Core.Exceptions
{
    public class KaixinException : System.Exception
    {
        public int Code { get; }

        public KaixinException(string message, int code = ErrorCode.SystemError)
            : base(message)
        {
            Code = code;
        }
    }
}
