namespace Zaza.SDK
{
    using UnityEngine;

    using static MessageHud;

    public static class HUD
    {
        public static void ShowBiomeFoundMsg(string text, bool playStinger = false)
        {
            if (Game.GetLocalPlayer() != null)
            {
                MessageHud.instance.ShowBiomeFoundMsg(text, playStinger);
            }
        }

        public static void ShowMessage(MessageType type, string text, int amount = 0, Sprite icon = null)
        {
            if (Game.GetLocalPlayer() != null)
            {
                MessageHud.instance.ShowMessage(type, text, amount, icon);
            }
        }

        public static void MessageAll(MessageType type, string text)
        {
            if (Game.GetLocalPlayer() != null)
            {
                MessageHud.instance.MessageAll(type, text);
            }
        }
    }
}