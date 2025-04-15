using System;

namespace SdWebUiClient.Events
{
    public class RequestCompletedEventArgs : EventArgs
    {
        public RequestCompletedEventArgs(Exception ex)
        {
            Exception = ex;
        }

        public Exception Exception { get; }

        public bool IsSuccess => Exception == null;
    }
}