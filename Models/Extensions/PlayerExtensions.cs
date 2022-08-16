namespace Zaza
{
    using System.Collections.Generic;

    using Nexd.Reflection;

    public static class PlayerExtensions
    {
        public static void SetComfortLevel(this Player player, int comfortLevel)
            => Pumped.SetFieldValue<Player, int>(player, "m_comfortLevel", comfortLevel);

        public static void SetUnderRoofState(this Player player, bool underRoofState)
            => Pumped.SetFieldValue<Player, bool>(player, "m_underRoof", underRoofState);

        public static void SetNoPlacementCost(this Player player, bool noPlacementCost)
            => Pumped.SetFieldValue<Player, bool>(player, "m_noPlacementCost", noPlacementCost);

        public static void SetMaxCarryWeight(this Player player, float carryWeight)
            => player.m_maxCarryWeight = carryWeight;

        public static void SetBaseCameraShake(this Player player, float cameraShake)
            => player.m_baseCameraShake = cameraShake;

        public static void SetJumpForce(this Player player, float jumpForce)
            => player.m_jumpForce = jumpForce;

        public static void SetJumpStaminaUsage(this Player player, float jumpStaminaUsage)
            => player.m_jumpStaminaUsage = jumpStaminaUsage;

        public static void SetAutoPickupRange(this Player player, float pickupRange)
            => player.m_autoPickupRange = pickupRange;

        public static void SetFlyMode(this Player player, bool flyMode)
            => player.m_flying = flyMode;

        public static void SetGuardianPowerCooldown(this Player player, float cooldown)
            => Pumped.SetFieldValue<Player, float>(player, "m_guardianPowerCooldown", cooldown);

        public static void SetCanSwim(this Player player, bool state)
            => player.m_canSwim = state;

        public static void SetSwimDepth(this Player player, float depth)
            => player.m_swimDepth = depth;

        public static void SetSwimSpeed(this Player player, float speed)
            => player.m_swimSpeed = speed;

        public static void SetInteractDistance(this Player player, float distance)
            => player.m_maxInteractDistance = distance;

        public static void Attack(this Player player, Character target, HitData hitData)
        {
            hitData.SetAttacker(player);
            target.ApplyDamage(hitData, false, false);
        }

        public static void InstantEquipQueue(this Player player)
        {
            List<Player.EquipQueueData> queue = Pumped.GetFieldValue<List<Player.EquipQueueData>, Player>(player, "m_equipQueue");

            foreach(Player.EquipQueueData queueData in queue)
            {
                queueData.m_duration = 0.0f;
                queueData.m_time = 0.0f;
            }
        }
    }
}
