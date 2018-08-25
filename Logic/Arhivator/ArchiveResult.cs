using System;

namespace Logic.Arhivator
{
    public class ArchiveResult
    {
        public static ArchiveResult Success()
        {
            return new ArchiveResult(true, null);
        }

        public static ArchiveResult Fail(Exception exception)
        {
            return new ArchiveResult(false, exception);
        }

        private ArchiveResult(bool result, Exception exception)
        {
            Result = result;
            Exception = exception;
        }

        public bool Result { get; }

        public Exception Exception { get; }
    }
}