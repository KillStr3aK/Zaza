namespace Zaza
{
    using UnityEngine;

    using Nexd.Reflection;
    using System.Reflection;

    public sealed class ZazaGUI
    {
        private static Rect MainWindow = new Rect(60.0f, 250.0f, 300.0f, 230.0f);

        private static Assembly IMGUIModule;

        static ZazaGUI()
            => IMGUIModule = Pumped.GetAssembly("UnityEngine.IMGUIModule");

        public static void Render()
            => MainWindow = ZazaGUI.Window(0, MainWindow, new GUI.WindowFunction(MainWindowCallback), "Zaza");

        public static void DragWindow(Rect A_0)
            => Pumped.InvokeInternalVoid(IMGUIModule, "GUI", "DragWindow", A_0);

        public static Rect Window(int A_0, Rect A_1, GUI.WindowFunction A_2, string A_3)
            => Pumped.InvokeInternal<Rect>(IMGUIModule, "GUI", "Window", A_0, A_1, A_2, A_3);

        public static void Label(Rect A_0, string A_1)
            => Pumped.InvokeInternalVoid(IMGUIModule, "GUI", "Label", A_0, A_1);

        public static void Label(Rect A_0, string A_1, GUIStyle A_2)
            => Pumped.InvokeInternalVoid(IMGUIModule, "GUI", "Label", A_0, A_1, A_2);

        public static void Button(Rect A_0, GUIContent A_1)
            => Pumped.InvokeInternal<bool>(IMGUIModule, "GUI", "Button", A_0, A_1);

        public static void RepeatButton(Rect A_0, GUIContent A_1)
            => Pumped.InvokeInternal<bool>(IMGUIModule, "GUI", "RepeatButton", A_0, A_1);

        private static void MainWindowCallback(int window)
        {
            ZazaGUI.Label(new Rect(100.0f, 126.0f, 100.0f, 100.0f), "Test");
        }
    }
}
