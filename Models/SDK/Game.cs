namespace Zaza.SDK
{
    using System.Collections.Generic;

    using UnityEngine;

    public static class Game
    {
        /// <summary>
        /// Get local <see cref="Player"/> instance.
        /// </summary>
        /// <returns>Local <see cref="Player"/> instance</returns>
#nullable enable
        public static Player? GetLocalPlayer()
            => Player.m_localPlayer;
#nullable disable

        public static bool IsInGame()
            => GetLocalPlayer() != null;

        public static bool InLoadingScreen()
        {
            Player localPlayer = GetLocalPlayer();
            return localPlayer == null || localPlayer.IsTeleporting();
        } 

        /// <summary>
        /// Kill mobs in the given distance.
        /// </summary>
        /// <param name="maxDistance">Maximum distance between the mob and the player</param>
        /// <param name="maxAmount">Maximum amount of mobs to be killed</param>
        /// <returns>Amount of mobs killed</returns>
        public static int KillMobs(float maxDistance, int maxAmount = -1)
        {
            Player localPlayer = Game.GetLocalPlayer();

            int amount = 0;
            List<Character> characters = Character.GetAllCharacters();
            HitData hit = new HitData();
            hit.m_damage.m_damage = 9999.9f;

            foreach (Character character in characters)
            {
                if (character.IsPlayer())
                    continue;

                if (Vector3.Distance(localPlayer.transform.position, character.transform.position) > maxDistance)
                    continue;

                localPlayer.Attack(character, hit);
                ++amount;

                if (maxAmount != -1 && amount == maxAmount)
                    break;
            }

            return amount;
        }

        public static void SetDebugMode(bool state)
            => Player.m_debugMode = state;

        public static void SetCameraFOV(float fov)
        {
            Camera mainCamera = Utils.GetMainCamera();
            Camera[] components = mainCamera.GetComponentsInChildren<Camera>();

            mainCamera.fieldOfView = fov;
            foreach (var i in components)
            {
                i.fieldOfView = fov;
            }
        }
    }
}