namespace Zaza
{
    using System;

    public class NotInGameException : Exception
    {
        public NotInGameException()
        {
        }

        public NotInGameException(string message)
            : base(message)
        {
        }

        public NotInGameException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}