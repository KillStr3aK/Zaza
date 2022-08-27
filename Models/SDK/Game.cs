namespace Zaza.SDK
{
    using System.Collections.Generic;

    using UnityEngine;

    using Nexd.Reflection;

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
            => GetLocalPlayer() != null && !InLoadingScreen();

        public static bool InLoadingScreen()
            => Pumped.Invoke<bool, ZNetScene>(ZNetScene.instance, "InLoadingScreen");

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
            => GameCamera.instance.m_fov = fov;

        public static void SetCameraMinWaterDistance(float distance)
            => GameCamera.instance.m_minWaterDistance = distance;

        public static void SetCameraMaxDistance(float distance)
            => GameCamera.instance.m_maxDistance = distance;

        public static void SetCameraMaxDistanceBoat(float distance)
            => GameCamera.instance.m_maxDistanceBoat = distance;

        public static void AddStatusEffect(string name, float range = 10.0f)
        {
            if (!Game.IsInGame())
                return;

            Player localPlayer = Game.GetLocalPlayer();
            List<Player> playersInRange = new List<Player>();
            Player.GetPlayersInRange(localPlayer.transform.position, range, playersInRange);
            
            foreach(Player player in playersInRange)
            {
                player.AddStatusEffect(name, true);
            }
        }
    }
}