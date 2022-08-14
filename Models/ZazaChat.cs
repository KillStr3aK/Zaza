namespace Zaza
{
    using UnityEngine;

    using Nexd.Reflection;
    using static Terminal;

    public sealed class ZazaChat : ZazaTerminal
    {
        private static readonly ZazaChat _instance = new ZazaChat();

        /// <summary>
        /// Gets the in-game <see cref="Chat.instance"/>.
        /// </summary>
        /// <exception cref="NotInGameException">If not in-game.</exception>
        public static Chat Instance
        {
            get
            {
                if(Chat.instance == null)
                {
                    throw new NotInGameException("Chat is only available in-game");
                }

                // it should set the Terminal when we're in-game
                if(_instance.Terminal == null)
                {
                    _instance.Terminal = GetTerminal();
                }

                return Chat.instance;
            }
        }

        /// <summary>
        /// Print to game chat.
        /// </summary>
        /// <param name="text">
        /// Text that you want to send.
        /// <para>You can use <see langword="HTML"/> colors using <![CDATA[ <color=[COLOR NAME|HEX CODE]>text</color> ]]></para> 
        /// <para>Examples:</para>
        /// <para><![CDATA[ <color=orange>text</color> ]]></para>
        /// <para><![CDATA[ <color=#FC0303>text</color> ]]></para>
        /// </param>
        public static void WriteLine(string text)
            => Instance.AddString($"<color=#73fc03>[ZAZA]</color> {text}");

        /// <summary>
        /// Checks whether the chat dialog is visible or not.
        /// </summary>
        /// <returns><see langword="true"/> if chat dialog is visible, otherwise <see langword="false"/></returns>
        public static bool IsChatDialogWindowVisible()
            => Instance.IsChatDialogWindowVisible();

        /// <summary>
        /// Checks whether the dialog is visible or not.
        /// </summary>
        /// <returns><see langword="true"/> if dialog is visible, otherwise <see langword="false"/></returns>
        public static bool IsDialogVisible(GameObject talker)
            => Instance.IsDialogVisible(talker);

        /// <summary>
        /// Sets font size for this <see cref="Terminal"/>
        /// </summary>
        /// <param name="size">Font size you want to use</param>
        public static void SetFontSize(int size)
            => _instance.SetFontSizeInternal(size);

        /// <summary>
        /// Gets the font size of this <see cref="Terminal"/>
        /// </summary>
        /// <returns>Font size</returns>
        public static int GetFontSize()
            => _instance.GetFontSizeInternal();

        /// <summary>
        /// Sets font color for this <see cref="Terminal"/>
        /// </summary>
        /// <param name="color"></param>
        public static void SetFontColor(Color color)
            => _instance.SetFontColorInternal(color);

        /// <summary>
        /// Gets the font color of this <see cref="Terminal"/>
        /// </summary>
        /// <returns></returns>
        public static Color GetFontColor()
            => _instance.GetFontColorInternal();

        /// <summary>
        /// Opens this <see cref="Terminal"/>
        /// </summary>
        public static void Open()
            => _instance.OpenInternal();

        /// <summary>
        /// Closes this <see cref="Terminal"/>
        /// </summary>
        public static void Close()
            => _instance.CloseInternal();

        /// <summary>
        /// Clear <see cref="Terminal"/> buffer
        /// </summary>
        public static void Clear()
            => _instance.ClearInternal();

        /// <summary>
        /// Checks whether the cheats are enabled or not.
        /// </summary>
        /// <returns><see langword="true"/> if the cheats are enabled, <see langword="false"/> otherwise</returns>
        public static bool IsCheatsEnabled()
            => _instance.IsCheatsEnabledInternal();

        /// <summary>
        /// Checks whether the given command is allowed or not.
        /// </summary>
        /// <param name="command">Command you want to check</param>
        /// <returns><see langword="true"/> if the command is allowed, <see langword="false"/> otherwise</returns>
        public static bool IsAllowedCommand(ConsoleCommand command)
            => _instance.IsAllowedCommandInternal(command);

        /// <summary>
        /// Gets the <see cref="Chat"/> instance.
        /// <para>It should be the same as <see cref="Chat.instance"/></para>
        /// </summary>
        /// <returns><see cref="Chat"/> instance</returns>
        public static Terminal GetTerminal()
            => Pumped.GetPropertyValue<Terminal, Chat>(Instance, "m_terminalInstance");
    }
}
