using System;

namespace UDPConnection
{
    public sealed class SocketException : Exception
    {
        public SocketException(Exception innerException) : base(innerException.Message, innerException)
        {

        }
    }
}