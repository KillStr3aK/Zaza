/*
namespace Zaza
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Collections.Generic;

    using Microsoft.CodeAnalysis.Scripting;
    using Microsoft.CodeAnalysis.CSharp.Scripting;

    internal class ScriptContext<T>
    {
#nullable enable
        public Script<T>? Script { get; set; } = null;

        public ScriptState<T>? State { get; set; } = null;
#nullable disable

        public void Compile()
            => this.Script.Compile();

        public void RunAsync(object globals = null)
            => this.State = this.Script.RunAsync(globals).Result;
    }

    internal static class ScriptingEnvironment
    {
        public static Dictionary<string, ScriptContext<object>> Scripts { get; set; } = new Dictionary<string, ScriptContext<object>>();

#nullable enable
        public static ScriptContext<object>? Create(string name, string path)
#nullable disable
        {
            if(!File.Exists(path))
            {
                ZazaConsole.Error($"Script file not found at path \"{path}\"");
                return null;
            }

            string code = File.ReadAllText(path);

            ScriptOptions options = ScriptOptions.Default
                .WithImports
                (
                    "System",
                    "System.Linq",
                    "System.Threading.Tasks",
                    "System.Collections.Generic",
                    "Zaza"
                )
                .WithReferences(AppDomain.CurrentDomain.GetAssemblies()
                    .Where(x => !x.IsDynamic && !string.IsNullOrWhiteSpace(x.Location)));

            ScriptContext<object> ctx = new ScriptContext<object>
            {
                Script = CSharpScript.Create(code, options)
            };

            Scripts[name] = ctx;
            return ctx;
        }
    }
}
*/
