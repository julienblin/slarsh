namespace Slarsh
{
    using System;

    public class SlarshException : Exception
    {
        public SlarshException()
        {
        }

        public SlarshException(string message)
            : base(message)
        {
        }

        public SlarshException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}
