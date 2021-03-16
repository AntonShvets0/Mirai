using System;

namespace Mirai.Exceptions
{
    public class PacketException : Exception
    {
        public PacketException(string message) : base(message)
        {
        }
    }
}