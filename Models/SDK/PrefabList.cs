namespace Zaza.SDK
{
    using System;
    using System.IO;
    using System.Collections.Generic;

    using UnityEngine;

    public class PrefabList : Dictionary<int, GameObject>
    {
        public PrefabList(Dictionary<int, GameObject> prefabCollection)
        {
            foreach (KeyValuePair<int, GameObject> prefab in prefabCollection)
            {
                this[prefab.Key] = prefab.Value;
            }
        }

        internal void Dump(string path)
        {
            try
            {
                using (FileStream fs = File.Open($"{path}\\prefab_dump.txt", FileMode.Create))
                {
                    using (StreamWriter sw = new StreamWriter(fs))
                    {
                        DateTime now = DateTime.Now;

                        sw.WriteLine($"Valheim version: {ZazaAssembly.GetGameVersion()} {now}");
                        sw.WriteLine("HASH\t\tNAME");

                        foreach (var prefab in this)
                        {
                            sw.WriteLine($"0x{prefab.Key.ToString("X")}\t{prefab.Value.name}");
                        }

                        double ms = (DateTime.Now - now).TotalMilliseconds;
                        sw.WriteLine($"Dumped {this.Count} prefab in {ms} ms");
                        ZazaConsole.WriteLine($"Dumped <color=lightgreen>{this.Count}</color> prefab in <color=#5f47d6>{ms}</color> ms");
                    }
                }
            }
            catch (Exception ex)
            {
                ZazaConsole.Exception(ex);
            }
        }
    }
}
