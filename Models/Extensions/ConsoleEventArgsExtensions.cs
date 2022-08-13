namespace Zaza
{
    using System;
    using System.ComponentModel;

    using static Terminal;

    internal static class ConsoleEventArgsExtensions
    {
        /// <inheritdoc cref="ZazaConsole.WriteLine"/>
        public static void Reply(this ConsoleEventArgs args, string text)
            => ZazaConsole.WriteLine(text);

        /// <inheritdoc cref="ZazaConsole.WriteLine"/>
        public static void ReplyError(this ConsoleEventArgs args, string text)
            => args.Reply($"<color=red>{text}</color>");

        /// <summary>
        /// Try to convert <see cref="string"/> to <typeparamref name="Type"/>
        /// </summary>
        /// <typeparam name="Type">Type you want to convert to</typeparam>
        /// <param name="args"></param>
        /// <param name="parameterIndex">Command parameter index.</param>
        /// <param name="defaultValue">Default value if convert fails.</param>
        /// <exception cref="NotSupportedException"></exception>
        /// <returns>Converted argument to <typeparamref name="Type"/></returns>
        public static Type TryParameter<Type>(this ConsoleEventArgs args, int parameterIndex, Type defaultValue)
        {
            static bool Convert<InnerType>(string input, out InnerType result)
            {
                result = default(InnerType);

                try
                {
                    TypeConverter converter = TypeDescriptor.GetConverter(typeof(InnerType));
                    if (converter != null)
                    {
                        result = (InnerType)converter.ConvertFromString(input);
                        return true;
                    }

                    return false;
                }
                catch (NotSupportedException)
                {
                    return false;
                }
            }

            if (args.Length <= parameterIndex || !Convert<Type>(args[parameterIndex], out var result))
            {
                return defaultValue;
            }

            return result;
        }

        public static bool TryParameterBool(this ConsoleEventArgs args, int parameterIndex, bool defaultValue = false)
            => args.TryParameter<bool>(parameterIndex, defaultValue);
    }
}
