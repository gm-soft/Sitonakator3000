using System;

namespace Logic.Arhivator
{
    public class AsyncActionResult
    {
        public static AsyncActionResult Success()
        {
            return new AsyncActionResult(true, null);
        }

        public static AsyncActionResult Fail(Exception exception)
        {
            return new AsyncActionResult(false, exception);
        }

        private AsyncActionResult(bool result, Exception exception)
        {
            Result = result;
            Exception = exception;
        }

        public bool Result { get; }

        public Exception Exception { get; }
    }
}