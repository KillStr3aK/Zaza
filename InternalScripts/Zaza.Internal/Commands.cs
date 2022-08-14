namespace Zaza.Internal
{
    using System;
    using System.Collections.Generic;

    using UnityEngine;

    using Zaza.SDK;
    using static Terminal;

    internal sealed class Commands
    {
        internal static void Register()
        {
            ZazaConsole.RegisterCommand("unlockdlc", "[(Optional)dlcname] Soft-Unlock the given DLC.", Commands.UnlockDLC);

            ZazaConsole.RegisterCommand("prefab_spawn", "[name] [amount] [level] Spawn any game prefab. (Items, mobs, etc.)", Commands.PrefabSpawn);

            ZazaConsole.RegisterCommand("kill_mobs", "[distance] [(Optional)maxamount] Kill entities", Commands.KillMobs);

            ZazaConsole.RegisterCommand("teleport_coord", "[x] [y] [z] Teleport to coord.", Commands.TeleportToCoord);

            ZazaConsole.RegisterCommand("heal", "Heal self to maxhealth.", Commands.Heal);

            ZazaConsole.RegisterCommand("map_reveal", "Reveals the whole map.", Commands.RevealMap);

            ZazaConsole.RegisterCommand("map_reset", "Resets the whole map.", Commands.ResetMap);

            ZazaConsole.RegisterCommand("map_merchant", "Discover closest vendor.", Commands.DiscoverMerchant);

            ZazaConsole.RegisterCommand("pickuprange", "[range] Set the pickup range. (aka loot magnet)", Commands.PickupRange);

            ZazaConsole.RegisterCommand("nodurability", "[true | false | 1 | 0] Toggle items durability.", Commands.NoDurability);

            ZazaConsole.RegisterCommand("camera_shake", "[number] Set the camera chake value.", Commands.SetCameraShake);

            ZazaConsole.RegisterCommand("place_distance", "[distance] Set the place distance value.", Commands.SetPlaceDistance);

            ZazaConsole.RegisterCommand("interact_distance", "[distance] Set the interact distance value.", Commands.SetInteractDistance);

            ZazaConsole.RegisterCommand("set_fov", "[number] Change the player fov.", Commands.SetFOV);

            ZazaConsole.RegisterCommand("comfort_level", "[number] Set the comfort level.", Commands.SetComfortLevel);

            ZazaConsole.RegisterCommand("jumpheight", "[number] Set the jump force.", Commands.SetJumpHeight);

            ZazaConsole.RegisterCommand("teleport_restrict", "[true | false | 1 | 0] Toggle no teleport item restriction.", Commands.ToggleTeleportRestrict);

            ZazaConsole.RegisterCommand("god_toggle", "[true | false | 1 | 0] Toggle god mode.", Commands.ToggleGodMode);

            ZazaConsole.RegisterCommand("ghost_toggle", "[true | false | 1 | 0] Toggle ghost mode.", Commands.ToggleGhostMode);

            ZazaConsole.RegisterCommand("equip_toggle", "[true | false | 1 | 0] Toggle instant equip.", Commands.ToggleInstantEquip);

            ZazaConsole.RegisterCommand("stamina_toggle", "[true | false | 1 | 0] Toggle infinite stamina.", Commands.ToggleInfiniteStamina);

            ZazaConsole.RegisterCommand("roof_state", "[true | false | 1 | 0] Set roof state.", Commands.SetRoofState);

            ZazaConsole.RegisterCommand("nocost_toggle", "[true | false | 1 | 0] Toggle no cost.", Commands.ToggleNoCost);

            ZazaConsole.RegisterCommand("fly_toggle", "[true | false | 1 | 0] Toggle fly mode.", Commands.ToggleFlyMode);

            ZazaConsole.RegisterCommand("debug_toggle", "[true | false | 1 | 0] Toggle debug mode.", Commands.ToggleNoCost);

            ZazaConsole.RegisterCommand("guardian_cooldown", "[true | false | 1 | 0] Toggle instant guardian cooldown.", Commands.ToggleGuardianCooldown);

            ZazaConsole.RegisterCommand("guardian_start", "Force start guardian power.", Commands.ForceStartGuardianPower);

            ZazaConsole.RegisterCommand("food_puke", "Clear foods.", Commands.ClearFoods);
        }

        public static void UnlockDLC(ConsoleEventArgs args)
        {
            if (args.Length == 1)
            {
                List<DLCMan.DLCInfo> dlcs = DLCManager.GetDLCs();
                string text = "";

                System.Random random = new System.Random();

                foreach (DLCMan.DLCInfo dlc in dlcs)
                {
                    text += $"<color={random.Next(0x1000000).ToString("X6")}>{dlc.m_name}</color> ";
                }

                args.Reply($"Available DLCs: {text}");
                return;
            } else if (args.Length != 2)
            {
                args.ReplyError("Syntax Error: /unlockdlc [(Optional)dlcname]");
                return;
            }

            string dlcName = args[1];
            DLCMan.DLCInfo dlcInfo = DLCManager.GetDLC(dlcName);

            if (dlcInfo == null)
            {
                args.ReplyError($"The given DLC ({dlcName}) does not exists.");
                return;
            }

            if (DLCManager.IsDLCInstalled(dlcName))
            {
                args.Reply($"The given DLC ({dlcName}) is already unlocked.");
                return;
            }

            DLCManager.UnlockDLC(dlcInfo);

            if (DLCManager.IsDLCInstalled(dlcName))
            {
                args.Reply($"Successfully unlocked DLC {dlcName}");
            }
            else
            {
                args.ReplyError($"Failed to unlock DLC \"{dlcName}\"");
            }
        }

        public static void PrefabSpawn(ConsoleEventArgs args)
        {
            if (!Game.IsInGame())
            {
                args.ReplyError("You can only spawn prefabs in-game.");
                return;
            }

            if (args.Length != 4)
            {
                args.ReplyError("Syntax Error: /prefab_spawn [name] [amount] [level]");
                return;
            }

            string prefabName = args[1];
            GameObject prefabObject = PrefabManager.GetPrefab(prefabName);

            if (prefabObject == null)
            {
                args.ReplyError("Invalid prefab! For prefab names, use command 'prefab_dump'");
                return;
            }

            try
            {
                if (int.TryParse(args[2], out int amount) && amount < 1 || amount > 100)
                {
                    args.ReplyError("Amount must be number and should be between 1 - 100");
                    return;
                }

                if (int.TryParse(args[3], out int level) && level < 1 || level > 5)
                {
                    args.ReplyError("Level must be number and should be between 1 - 5");
                    return;
                }

                Player localPlayer = Game.GetLocalPlayer();
                DateTime now = DateTime.Now;

                for (int i = 0; i < amount; i++)
                {
                    GameObject spawnedPrefab = PrefabManager.SpawnPrefab(prefabObject, localPlayer.transform.position + localPlayer.transform.forward * 2.0f + Vector3.up + (UnityEngine.Random.insideUnitSphere * 0.5f), Quaternion.identity);
                    
                    if (spawnedPrefab != null && spawnedPrefab.TryGetComponent<Character>(out Character characterComponent))
                    {
                        if (characterComponent & level > 1)
                        {
                            characterComponent.SetLevel(level);
                        }
                    }
                }

                string log = $"Spawned x<color=yellow>{amount}</color> of <color=cyan>{prefabObject.name}</color> (Level <color=orange>{level}</color>) in <color=#5f47d6>{(DateTime.Now - now).TotalMilliseconds}</color> ms";
                args.Reply(log);
                ZazaChat.WriteLine(log);
            }
            catch (Exception ex)
            {
                ZazaConsole.Exception(ex);
            }
        }

        public static void KillMobs(ConsoleEventArgs args)
        {
            if (Game.IsInGame())
            {
                args.ReplyError("You can only kill animals in-game.");
                return;
            }

            if (args.Length < 2 || args.Length > 3)
            {
                args.ReplyError("Syntax Error: /kill_mobs [distance] [(Optional)maxamount]");
                return;
            }

            if (float.TryParse(args[1], out float distance) && distance < 0.0f)
            {
                args.ReplyError("Distance must be greater than zero.");
                return;
            }

            if (int.TryParse(args[2], out int maxAmount) && maxAmount < 0)
            {
                args.ReplyError("Max Amount must be greater than zero.");
                return;
            }

            int result = 0;

            try
            {
                result = Game.KillMobs(distance, maxAmount);
            }
            catch (Exception ex)
            {
                ZazaConsole.Exception(ex);
            }

            args.Reply($"Killed <color=yellow>{result}</color> animals.");
        }

        public static void TeleportToCoord(ConsoleEventArgs args)
        {
            if (!Game.IsInGame())
            {
                args.ReplyError("You can only teleport in-game.");
                return;
            }

            if (args.Length != 4)
            {
                args.ReplyError("Syntax Error: /teleport_coord [x] [y] [z]");
                return;
            }

            float[] coords = { 0.0f, 0.0f, 0.0f };
            for (int i = 1; i < 4; i++)
            {
                if (!float.TryParse(args[i], out coords[i - 1]))
                {
                    args.ReplyError($"Parameter {i} is invalid!");
                    return;
                }
            }

            Player localPlayer = Game.GetLocalPlayer();
            
            if(!localPlayer.IsTeleporting() && localPlayer.IsTeleportable())
            {
                localPlayer.TeleportTo(new Vector3(coords[0], coords[1], coords[2]), localPlayer.transform.rotation, true);
            } else
            {
                args.ReplyError("Unable to teleport.");
            }
        }

        public static void Heal(ConsoleEventArgs args)
        {
            if (!Game.IsInGame())
            {
                args.ReplyError("You must be in-game.");
                return;
            }

            Player localPlayer = Game.GetLocalPlayer();
            localPlayer.Heal(localPlayer.GetMaxHealth(), true);

            args.Reply("You have been healed up!");
        }

        public static void RevealMap(ConsoleEventArgs args)
        {
            if (!Game.IsInGame())
            {
                args.ReplyError("You must be in-game.");
                return;
            }

            Minimap.instance.ExploreAll();
            args.Reply("Revealed the whole map!");
        }

        public static void ResetMap(ConsoleEventArgs args)
        {
            if (!Game.IsInGame())
            {
                args.ReplyError("You must be in-game.");
                return;
            }

            args.Reply("Reseted the whole map!");
            Minimap.instance.Reset();
        }

        public static void DiscoverMerchant(ConsoleEventArgs args)
        {
            if (!Game.IsInGame())
            {
                args.ReplyError("You must be in-game.");
                return;
            }

            Player localPlayer = Game.GetLocalPlayer();
            ZoneSystem.LocationInstance locationInstance;
            ZoneSystem.instance.FindClosestLocation("Vendor_BlackForest", localPlayer.transform.position, out locationInstance);
            Minimap.instance.DiscoverLocation(locationInstance.m_position, Minimap.PinType.Icon3, "Merchant", true);
            
            args.Reply($"Merchant position: X: {locationInstance.m_position.x} Y: {locationInstance.m_position.y} Z: {locationInstance.m_position.z}");
        }

        public static void PickupRange(ConsoleEventArgs args)
        {
            if (args.Length != 2)
            {
                args.ReplyError($"Usage: /pickuprange [range] Current value: {Settings.PickupRange}");
                return;
            }

            if (!float.TryParse(args[1], out Settings.PickupRange))
            {
                args.ReplyError("Parameter 1 is invalid!");
                return;
            }

            args.Reply($"Pickup range has been changed to <color=yellow>{Settings.PickupRange}</color>");
        }

        public static void NoDurability(ConsoleEventArgs args)
        {
            if (args.Length != 2)
            {
                args.ReplyError($"Usage: /nodurability [true | false | 1 | 0] Current value: {Settings.NoDurability}");
                return;
            }

            if (!bool.TryParse(args[1], out Settings.NoDurability))
            {
                args.ReplyError("Parameter 1 is invalid!");
                return;
            }

            args.Reply($"No durability has been changed to <color=yellow>{Settings.NoDurability}</color>");
        }

        public static void SetJumpHeight(ConsoleEventArgs args)
        {
            if (args.Length != 2)
            {
                args.ReplyError($"Usage: /jumpheight [number] Current value: {Settings.JumpHeight}");
                return;
            }

            if (!float.TryParse(args[1], out Settings.JumpHeight))
            {
                args.ReplyError("Parameter 1 is invalid!");
                return;
            }

            args.Reply($"Jump height has been changed to <color=yellow>{Settings.JumpHeight}</color>");
        }

        public static void SetPlaceDistance(ConsoleEventArgs args)
        {
            if (args.Length != 2)
            {
                args.ReplyError($"Usage: /placedistance [distance] Current value: {Settings.PlaceDistance}");
                return;
            }

            if (!float.TryParse(args[1], out Settings.PlaceDistance))
            {
                args.ReplyError("Parameter 1 is invalid!");
                return;
            }

            args.Reply($"Place distance has been changed to <color=yellow>{Settings.PlaceDistance}</color>");
        }
        
        public static void SetInteractDistance(ConsoleEventArgs args)
        {
            if (args.Length != 2)
            {
                args.ReplyError($"Usage: /interact_distance [distance] Current value: {Settings.InteractDistance}");
                return;
            }

            if (!float.TryParse(args[1], out Settings.InteractDistance))
            {
                args.ReplyError("Parameter 1 is invalid!");
                return;
            }

            args.Reply($"Interact distance has been changed to <color=yellow>{Settings.InteractDistance}</color>");
        }
        
        public static void SetCameraShake(ConsoleEventArgs args)
        {
            if (args.Length != 2)
            {
                args.ReplyError($"Usage: /interact_distance [distance] Current value: {Settings.CameraShake}");
                return;
            }

            if (!float.TryParse(args[1], out Settings.CameraShake))
            {
                args.ReplyError("Parameter 1 is invalid!");
                return;
            }

            args.Reply($"Camera shake has been changed to <color=yellow>{Settings.CameraShake}</color>");
        }
        
        public static void SetFOV(ConsoleEventArgs args)
        {
            if (args.Length != 2)
            {
                args.ReplyError($"Usage: /set_fov [number] Current value: {Settings.FOV}");
                return;
            }

            if (!float.TryParse(args[1], out Settings.FOV))
            {
                args.ReplyError("Parameter 1 is invalid!");
                return;
            }

            args.Reply($"Player FOV has been changed to <color=yellow>{Settings.FOV}</color>");
        }
        
        public static void ToggleTeleportRestrict(ConsoleEventArgs args)
        {
            if (args.Length != 2)
            {
                args.ReplyError($"Usage: /teleport_restrict [true | false | 1 | 0] Current value: {Settings.TeleportRestrict}");
                return;
            }

            if (!bool.TryParse(args[1], out Settings.TeleportRestrict))
            {
                args.ReplyError("Parameter 1 is invalid!");
                return;
            }

            args.Reply($"No teleport restriction has been changed to <color=yellow>{Settings.TeleportRestrict}</color>");
        }
        
        public static void ToggleGodMode(ConsoleEventArgs args)
        {
            if (args.Length != 2)
            {
                args.ReplyError($"Usage: /god_toggle [true | false | 1 | 0] Current value: {Settings.GodMode}");
                return;
            }

            if (!bool.TryParse(args[1], out Settings.GodMode))
            {
                args.ReplyError("Parameter 1 is invalid!");
                return;
            }

            args.Reply($"God mode has been changed to <color=yellow>{Settings.GodMode}</color>");
        }
        
        public static void ToggleGhostMode(ConsoleEventArgs args)
        {
            if (args.Length != 2)
            {
                args.ReplyError($"Usage: /ghost_toggle [true | false | 1 | 0] Current value: {Settings.GhostMode}");
                return;
            }

            if (!bool.TryParse(args[1], out Settings.GhostMode))
            {
                args.ReplyError("Parameter 1 is invalid!");
                return;
            }

            args.Reply($"Ghost mode has been changed to <color=yellow>{Settings.GhostMode}</color>");
        }

        public static void ToggleInstantEquip(ConsoleEventArgs args)
        {
            if (args.Length != 2)
            {
                args.ReplyError($"Usage: /equip_toggle [true | false | 1 | 0] Current value: {Settings.InstantEquip}");
                return;
            }

            if (!bool.TryParse(args[1], out Settings.InstantEquip))
            {
                args.ReplyError("Parameter 1 is invalid!");
                return;
            }

            args.Reply($"Instant equip has been changed to <color=yellow>{Settings.InstantEquip}</color>");
        }

        public static void ToggleInfiniteStamina(ConsoleEventArgs args)
        {
            if (args.Length != 2)
            {
                args.ReplyError($"Usage: /stamina_toggle [true | false | 1 | 0] Current value: {Settings.InfiniteStamina}");
                return;
            }

            if (!bool.TryParse(args[1], out Settings.InfiniteStamina))
            {
                args.ReplyError("Parameter 1 is invalid!");
                return;
            }

            args.Reply($"Infinite stamina has been changed to <color=yellow>{Settings.InfiniteStamina}</color>");
        }

        public static void SetComfortLevel(ConsoleEventArgs args)
        {
            if (args.Length != 2)
            {
                args.ReplyError($"Usage: /comfort_level [number] Current value: {Settings.ComfortLevel}");
                return;
            }

            if (!int.TryParse(args[1], out Settings.ComfortLevel))
            {
                args.ReplyError("Parameter 1 is invalid!");
                return;
            }

            args.Reply($"Comfort level has been changed to <color=yellow>{Settings.ComfortLevel}</color>");
        }
        
        public static void SetRoofState(ConsoleEventArgs args)
        {
            if (args.Length != 2)
            {
                args.ReplyError($"Usage: /roof_state [true | false | 1 | 0] Current value: {Settings.RoofState}");
                return;
            }

            if (!bool.TryParse(args[1], out Settings.RoofState))
            {
                args.ReplyError("Parameter 1 is invalid!");
                return;
            }

            args.Reply($"Roof state has been changed to <color=yellow>{Settings.RoofState}</color>");
        }
        
        public static void ToggleNoCost(ConsoleEventArgs args)
        {
            if (args.Length != 2)
            {
                args.ReplyError($"Usage: /nocost_toggle [true | false | 1 | 0] Current value: {Settings.NoCost}");
                return;
            }

            if (!bool.TryParse(args[1], out Settings.NoCost))
            {
                args.ReplyError("Parameter 1 is invalid!");
                return;
            }

            args.Reply($"No cost has been changed to <color=yellow>{Settings.NoCost}</color>");
        }

        public static void ToggleDebugMode(ConsoleEventArgs args)
        {
            if (args.Length != 2)
            {
                args.ReplyError($"Usage: /debug_toggle [true | false | 1 | 0] Current value: {Settings.DebugMode}");
                return;
            }

            if (!bool.TryParse(args[1], out Settings.DebugMode))
            {
                args.ReplyError("Parameter 1 is invalid!");
                return;
            }

            args.Reply($"Debug mode has been changed to <color=yellow>{Settings.DebugMode}</color>");
        }
        
        public static void ToggleFlyMode(ConsoleEventArgs args)
        {
            if (args.Length != 2)
            {
                args.ReplyError($"Usage: /fly_toggle [true | false | 1 | 0] Current value: {Settings.FlyMode}");
                return;
            }

            if (!bool.TryParse(args[1], out Settings.FlyMode))
            {
                args.ReplyError("Parameter 1 is invalid!");
                return;
            }

            args.Reply($"Fly mode has been changed to <color=yellow>{Settings.FlyMode}</color>");
        }

        public static void ClearFoods(ConsoleEventArgs args)
        {
            if (!Game.IsInGame())
            {
                args.ReplyError("You must be in-game.");
                return;
            }

            Player localPlayer = Game.GetLocalPlayer();
            localPlayer.ClearFood();
            ZazaConsole.WriteLine("Your foods has been cleared!");
        }

        public static void ToggleGuardianCooldown(ConsoleEventArgs args)
        {
            if (args.Length != 2)
            {
                args.ReplyError($"Usage: /guardian_cooldown [true | false | 1 | 0] Current value: {Settings.InstantGuardianPower}");
                return;
            }

            if (!bool.TryParse(args[1], out Settings.InstantGuardianPower))
            {
                args.ReplyError("Parameter 1 is invalid!");
                return;
            }

            args.Reply($"Instant guardian power has been changed to <color=yellow>{Settings.InstantGuardianPower}</color>");
        }

        public static void ForceStartGuardianPower(ConsoleEventArgs args)
        {
            if (!Game.IsInGame())
            {
                args.ReplyError("You must be in-game.");
                return;
            }

            Player localPlayer = Game.GetLocalPlayer();
            if (localPlayer.StartGuardianPower())
            {
                args.Reply("Force started guardian power.");
            } else
            {
                args.ReplyError("No guardian power is equipped or couldn't force start it!");
            }
        }
    }
}