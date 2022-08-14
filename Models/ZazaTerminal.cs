namespace Zaza
{
    using System.Collections.Generic;

    using Nexd.Reflection;

    using UnityEngine;

    using static Terminal;

    public abstract class ZazaTerminal
    {
        /// <summary>
        /// <see cref="Terminal"/> instance.
        /// </summary>
        protected Terminal Terminal { get; set; }

        /// <summary>
        /// Checks whether the given command is allowed or not.
        /// </summary>
        /// <param name="command">Command you want to check.</param>
        /// <returns>Hardcoded to return <see langword="true"/></returns>
        internal bool IsAllowedCommandInternal(ConsoleCommand command)
            => Pumped.Invoke<bool, Terminal>(this.Terminal, "isAllowedCommand", command);

        /// <summary>
        /// Checks whether the built-in cheats are enabled or not.
        /// </summary>
        /// <returns><see langword="true"/> if cheats are enabled, otherwise <see langword="false"/></returns>
        internal bool IsCheatsEnabledInternal()
            => this.Terminal.IsCheatsEnabled();

        internal void SetFontSizeInternal(int size)
            => this.Terminal.m_output.fontSize = size;

        internal int GetFontSizeInternal()
            => this.Terminal.m_output.fontSize;

        internal void SetFontColorInternal(Color color)
            => this.Terminal.m_output.color = color;

        internal Color GetFontColorInternal()
            => this.Terminal.m_output.color;

        internal void OpenInternal()
            => this.Terminal.m_chatWindow.gameObject.SetActive(true);

        internal void CloseInternal()
            => this.Terminal.m_chatWindow.gameObject.SetActive(false);

        internal void ClearInternal()
        {
            List<string> m_chatBuffer = Pumped.GetFieldValue<List<string>, Terminal>(this.Terminal, "m_chatBuffer");

            if(m_chatBuffer != null)
            {
                m_chatBuffer.Clear();
            }

            this.Terminal.m_output.text = string.Empty;
        }
    }
}
