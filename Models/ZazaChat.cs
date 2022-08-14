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

        public static void SetFontSize(int size)
            => _instance.SetFontSizeInternal(size);

        public static int GetFontSize()
            => _instance.GetFontSizeInternal();

        public static void SetFontColor(Color color)
            => _instance.SetFontColorInternal(color);

        public static Color GetFontColor()
            => _instance.GetFontColorInternal();

        public static void Open()
            => _instance.OpenInternal();

        public static void Close()
            => _instance.CloseInternal();

        public static void Clear()
            => _instance.ClearInternal();

        public static bool IsCheatsEnabled()
            => _instance.IsCheatsEnabledInternal();

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
