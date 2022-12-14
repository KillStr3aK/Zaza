namespace Zaza
{
    using System;

    using UnityEngine;

    internal sealed class Zaza : MonoBehaviour
    {
        void Awake()
        {
            ZazaConsole.SetFontSize(12);

            ZazaConsole.WriteLine($"Injected version: <color=yellow>{ZazaAssembly.GetCheatVersion()}</color>");
            ZazaConsole.WriteLine("Developers: <color=purple>Nexd @ Eternar</color>");

            try
            {
                // Any code that may throw
                ScriptHandler.LoadScriptsFromDirectory("Zaza/Scripts");
            } catch (Exception ex)
            {
                ZazaConsole.Exception(ex);
            } finally
            {
                // Code that is unlikely to throw

                Commands.Register();
                ZazaConsole.WriteLine($"Registered <color=yellow>{ZazaConsole.Commands.Count}</color> commands.");
            }
        }

        void Update()
            => ScriptRuntime.Update();

        void FixedUpdate()
            => ScriptRuntime.FixedUpdate();

        void LateUpdate()
            => ScriptRuntime.LateUpdate();

        void OnGUI()
            => ZazaGUI.Render();
    }
}
