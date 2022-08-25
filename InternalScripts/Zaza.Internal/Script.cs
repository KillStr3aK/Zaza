namespace Zaza.Internal
{
    using System.Collections.Generic;

    using Zaza;
    using Zaza.SDK;

    public class ZazaInternal : Script
    {
        public ZazaInternal()
        {
            Commands.Register();
        }

        public override void OnUpdate()
        {
            if (!Game.IsInGame())
                return;

            Player localPlayer = Game.GetLocalPlayer();
            localPlayer.SetGodMode(Settings.GodMode);
            localPlayer.SetGhostMode(Settings.GhostMode);
            localPlayer.SetFlyMode(Settings.FlyMode);
            localPlayer.SetAutoPickupRange(Settings.PickupRange);
            localPlayer.SetBaseCameraShake(Settings.CameraShake);
            localPlayer.SetJumpForce(Settings.JumpHeight);
            localPlayer.SetComfortLevel(Settings.ComfortLevel);
            localPlayer.SetUnderRoofState(Settings.RoofState);
            localPlayer.SetNoPlacementCost(Settings.NoCost);
            localPlayer.SetInteractDistance(Settings.InteractDistance);

            Game.SetCameraFOV(Settings.FOV);

            if (Settings.InstantEquip)
            {
                localPlayer.InstantEquipQueue();
            }

            if (Settings.InfiniteStamina)
            {
                localPlayer.AddStamina(localPlayer.GetMaxStamina());
            }

            if (Settings.InstantGuardianPower)
            {
                localPlayer.SetGuardianPowerCooldown(0.0f);
            }

            if (Settings.WalkUnderWater)
            {
                localPlayer.SetCanSwim(false);
                localPlayer.SetSwimDepth(100.0f);

                Game.SetCameraMinWaterDistance(-100.0f);
            } else if (Settings.WalkOnWater)
            {
                localPlayer.SetCanSwim(true);
                localPlayer.SetSwimDepth(-0.5f);
            }

            Inventory inventory = localPlayer.GetInventory();
            List<ItemDrop.ItemData> inventoryItems = inventory.GetAllItems();

            if (Settings.NoDurability)
            {
                foreach(ItemDrop.ItemData item in inventoryItems)
                {
                    item.m_shared.m_useDurability = false;
                }
            }
        }
    }
}
