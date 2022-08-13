namespace Zaza
{
    using Nexd.Reflection;

    using static Terminal;

    public abstract class ZazaTerminal
    {
        /// <summary>
        /// <see cref="Terminal"/> instance.
        /// </summary>
        protected static Terminal Terminal { get; set; }

        /// <summary>
        /// Checks whether the given command is allowed or not.
        /// </summary>
        /// <param name="command">Command you want to check.</param>
        /// <returns>Hardcoded to return <see langword="true"/></returns>
        public static bool IsAllowedCommand(ConsoleCommand command)
            => Pumped.Invoke<bool, Terminal>(Terminal, "isAllowedCommand", command);

        /// <summary>
        /// Checks whether the built-in cheats are enabled or not.
        /// </summary>
        /// <returns><see langword="true"/> if cheats are enabled, otherwise <see langword="false"/></returns>
        public static bool IsCheatsEnabled()
            => Terminal.IsCheatsEnabled();
    }
}
