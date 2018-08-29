using System;

namespace Logic.Server
{
    public class IisServerException : Exception
    {
        public IisServerException(string message) : base(message) { }

        public IisServerException(string message, Exception innerException) : base(message, innerException) { }
    }
}