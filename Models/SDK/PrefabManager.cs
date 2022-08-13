namespace Zaza.SDK
{
    using System.Collections.Generic;

    using Nexd.Reflection;

    using UnityEngine;

    public sealed class PrefabManager
    {
        /// <summary>
        /// Dump prefabs to file.
        /// </summary>
        internal static void Dump(string path)
        {
            PrefabList prefabs = GetPrefabs();

            if(prefabs == null)
            {
                ZazaConsole.Error("Couldn't get prefabs.");
                return;
            }

            prefabs.Dump(path);
        }

#nullable enable
        public static GameObject? GetPrefab(string prefabName)
            => ZNetScene.instance.GetPrefab(prefabName);

        public static GameObject? SpawnPrefab(string prefabName, Vector3 coord, Quaternion rotation)
        {
            GameObject? prefab = GetPrefab(prefabName);

            if(prefab != null)
            {
                return Object.Instantiate<GameObject>(prefab, coord, rotation);
            }

            return null;
        }
#nullable disable

        public static GameObject SpawnPrefab(GameObject prefabObject, Vector3 coord, Quaternion rotation)
            => Object.Instantiate<GameObject>(prefabObject, coord, rotation);

        /// <summary>
        /// Gets every prefab from the game.
        /// </summary>
        /// <exception cref="NotInGameException"></exception>
        /// <returns><see cref="PrefabList"/> with game prefabs.</returns>
        public static PrefabList GetPrefabs()
        {
            if (ZNetScene.instance == null)
            {
                ZazaConsole.Error("You can only dump prefabs in-game.");
                return null;
            }

            return new PrefabList(Pumped.GetFieldValue<Dictionary<int, GameObject>, ZNetScene>(ZNetScene.instance, "m_namedPrefabs"));
        }

        public static bool IsAreaReady(Vector3 coord)
            => ZNetScene.instance.IsAreaReady(coord);
    }
}
