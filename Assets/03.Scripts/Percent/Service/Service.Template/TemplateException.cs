using System;
using Service.Net;

namespace Service.Template
{
    public class TemplateException : Exception
    {
        public readonly int ErrorCode = -1;
        public readonly ISerializer Serializer = null;

        public TemplateException(int errorCode, string format, params object[] param) 
            : base(string.Format(format, param))
		{
            ErrorCode = errorCode;
        }

        public TemplateException(ISerializer serializer, string format, params object[] param) 
            : base(string.Format(format, param))
		{
            Serializer = serializer;
        }
    }
}