namespace Zaza
{
    using Nexd.Reflection;
    using System.Collections.Generic;

    using static Terminal;

    internal sealed class ZazaConsole : ZazaTerminal
    {
        /// <summary>
        /// Dictionary that contains commands that has been registered by the cheat.
        /// </summary>
        public static Dictionary<string, ConsoleCommand> Commands { get; set; } = new Dictionary<string, ConsoleCommand>();

        /// <summary>
        /// Dictionary that contains every built-in command.
        /// <para>Might be used to restore them.</para>
        /// </summary>
        public readonly static Dictionary<string, ConsoleCommand> BuiltInCommands = Pumped.GetFieldValue<Dictionary<string, ConsoleCommand>, Terminal>("commands");

        static ZazaConsole()
            => Terminal = GetTerminal();

        /// <summary>
        /// Register a new command to the terminal.
        /// </summary>
        /// <param name="command">Command name</param>
        /// <param name="description">Description</param>
        /// <param name="callback">Callback that will be executed when the command is used.</param>
        public static void RegisterCommand(string command, string description, ConsoleEvent callback)
            => Commands[command] = new ConsoleCommand(command, description, callback);

        /// <summary>
        /// Remove single command from the terminal.
        /// </summary>
        /// <param name="command">Command you want to remove.</param>
        public static bool RemoveCommand(string command)
        {
            Dictionary<string, ConsoleCommand> commands = Pumped.GetFieldValue<Dictionary<string, ConsoleCommand>, Terminal>("commands");
            if (commands.Remove(command))
            {
                WriteLine($"Removed command <color=yellow>{command}</color>");
                Pumped.SetFieldValue<Terminal, Dictionary<string, ConsoleCommand>>("commands", commands);
                return true;
            }

            WriteLine($"Failed to remove command <color=red>{command}</color>");
            return false;
        }

        /// <summary>
        /// Remove every command that has been registered by <see langword="Zaza"/>
        /// </summary>
        public static void RemoveCommands()
        {
            foreach (KeyValuePair<string, ConsoleCommand> command in Commands)
            {
                RemoveCommand(command.Key);
            }
        }

        /// <summary>
        /// Remove every built-in command.
        /// </summary>
        public static void RemoveBuiltInCommands()
        {
            Dictionary<string, ConsoleCommand> commands = Pumped.GetFieldValue<Dictionary<string, ConsoleCommand>, Terminal>("commands");
            List<string> commandsToBeRemoved = new List<string>();

            foreach (KeyValuePair<string, ConsoleCommand> command in commands)
            {
                // Not built-in command
                if (Commands.ContainsKey(command.Key))
                    continue;

                commandsToBeRemoved.Add(command.Key);
            }

            foreach (string command in commandsToBeRemoved)
            {
                RemoveCommand(command);
            }
        }

        /// <summary>
        /// Gets the given command <see cref="ConsoleCommand"/> instance.
        /// </summary>
        /// <param name="command">Command name.</param>
        /// <returns><see cref="ConsoleCommand"/> instance for the given command, if exists.</returns>
#nullable enable
        public static ConsoleCommand? GetCommand(string command)
#nullable disable
        {
            Dictionary<string, ConsoleCommand> commands = Pumped.GetFieldValue<Dictionary<string, ConsoleCommand>, Terminal>("commands");
            if (commands.TryGetValue(command, out ConsoleCommand cmd))
            {
                return cmd;
            }

            return null;
        }

        /// <summary>
        /// Checks whether the given command is registered.
        /// </summary>
        /// <param name="command">Command name.</param>
        /// <returns><see langword="true"/> if the command is registered, otherwise <see langword="false"/></returns>
        public static bool IsCommandRegistered(string command)
            => Pumped.GetFieldValue<Dictionary<string, ConsoleCommand>, Terminal>("commands").ContainsKey(command);

        /// <summary>
        /// Print to game console.
        /// </summary>
        /// <param name="text">
        /// Text that you want to send.
        /// <para>You can use <see langword="HTML"/> colors using <![CDATA[ <color=[COLOR NAME|HEX CODE]>text</color> ]]></para> 
        /// <para>Examples:</para>
        /// <para><![CDATA[ <color=orange>text</color> ]]></para>
        /// <para><![CDATA[ <color=#FC0303>text</color> ]]></para>
        /// </param>
        public static void WriteLine(string text)
            => Console.instance.Print($"<color=#73fc03>[ZAZA]</color> {text}");

        /// <inheritdoc cref="ZazaConsole.WriteLine"/>
        public static void Error(string text)
            => Console.instance.Print($"<color=#73fc03>[ZAZA]</color> <color=red>[ERROR]</color> {text}");

        /// <summary>
        /// Prints exception to console.
        /// </summary>
        /// <param name="ex">Thrown exception.</param>
        public static void Exception(System.Exception ex)
            => Error($"{ex.Message}\n{ex.StackTrace}");

        /// <summary>
        /// Checks whether the console is enabled or not.
        /// </summary>
        /// <returns><see langword="true"/> if console is enabled, otherwise <see langword="false"/></returns>
        public static bool IsConsoleEnabled()
            => Console.instance.IsConsoleEnabled();

        /// <summary>
        /// Checks whether the console is visible or not.
        /// </summary>
        /// <returns><see langword="true"/> if the console is visible, otherwise <see langword="false"/></returns>
        public static bool IsVisible()
            => Console.IsVisible();

        /// <summary>
        /// Gets the <see cref="Console"/> instance.
        /// <para>It should be the same as <see cref="Console.instance"/></para>
        /// </summary>
        /// <returns><see cref="Console"/> instance</returns>
        public static Terminal GetTerminal()
            => Pumped.GetPropertyValue<Terminal, Console>(Console.instance, "m_terminalInstance");
    }
}
