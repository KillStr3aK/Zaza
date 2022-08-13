namespace Zaza
{
    using System;

    public class PluginException : Exception
    {
        public PluginException()
        {
        }

        public PluginException(string message)
            : base(message)
        {
        }

        public PluginException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}