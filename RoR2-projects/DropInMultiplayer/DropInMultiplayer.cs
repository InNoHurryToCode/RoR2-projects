using System;
using BepInEx;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using RoR2;
using UnityEngine;
using MonoMod.Cil;
using System.Linq;
using System.Collections;
using System.Reflection;
using Mono.Cecil.Cil;
using BepInEx.Configuration;
using UnityEngine.Networking;

namespace DropInMultiplayer {
    [BepInDependency("com.bepis.r2api")]
    [BepInPlugin("com.morris1927.DropInMultiplayer", "DropInMultiplayer", "2.2.1")]
    public class DropInMultiplayer : BaseUnityPlugin {

        private static ConfigWrapper<bool> ImmediateSpawn { get; set; }
        private static ConfigWrapper<bool> NormalSurvivorsOnly { get; set; }
        private static ConfigWrapper<bool> AllowSpawnAsWhileAlive { get; set; }
        private static ConfigWrapper<bool> StartWithItems { get; set; }
        public static ConfigWrapper<bool> SpawnAsEnabled { get; set; }
        public static ConfigWrapper<bool> HostOnlySpawnAs { get; set; }

        private static DropInMultiplayer instance { get; set; }


        public static List<string> survivorList = new List<string>{
            "CommandoBody",
            "HuntressBody",
            "EngiBody",
            "ToolbotBody",
            "MercBody",
            "MageBody",
            "BanditBody",
            "TreebotBody",
        };

        public static List<List<string>> bodyList = new List<List<string>> {
            new List<string> { "AssassinBody", "Assassin"},
            new List<string> { "CommandoBody", "Commando"},
            new List<string> { "HuntressBody", "Huntress"},
            new List<string> { "EngiBody", "Engi", "Engineer"},
            new List<string> { "ToolbotBody", "Toolbot", "MULT", "MUL-T"},
            new List<string> { "MercBody", "Merc", "Mercenary"},
            new List<string> { "MageBody", "Mage", "Artificer"},
            new List<string> { "BanditBody", "Bandit"},
            new List<string> { "SniperBody", "Sniper"},
            new List<string> { "HANDBody", "HAND", "HAN-D"},
            new List<string> { "TreebotBody", "Support", "Rex"},

            new List<string> { "AltarSkeletonBody", "AltarSkeleton"},
            new List<string> { "AncientWispBody", "AncientWisp"},
            new List<string> { "ArchWispBody", "ArchWisp"},
            new List<string> { "BackupDroneBody", "BackupDrone"},
            new List<string> { "BackupDroneOldBody", "BackupDroneOld"},
            new List<string> { "BeetleBody", "Beetle"},
            new List<string> { "BeetleGuardAllyBody", "BeetleGuardAlly"},
            new List<string> { "BeetleGuardBody", "BeetleGuard"},
            new List<string> { "BeetleQueen2Body", "BeetleQueen"},
            new List<string> { "BellBody", "Bell"},
            new List<string> { "BirdsharkBody", "Birdshark"},
            new List<string> { "BisonBody", "Bison"},
            new List<string> { "BomberBody", "Bomber"},
            new List<string> { "ClayBody", "Clay"},
            new List<string> { "ClayBossBody", "ClayBoss"},
            new List<string> { "ClayBruiserBody", "ClayBruiser"},
            new List<string> { "CommandoPerformanceTestBody", "CommandoPerformanceTest"},
            new List<string> { "Drone1Body", "Drone1"},
            new List<string> { "Drone2Body", "Drone2"},
            new List<string> { "ElectricWormBody", "ElectricWorm"},
            new List<string> { "EnforcerBody", "Enforcer"},
            new List<string> { "EngiBeamTurretBody", "EngiBeamTurret"},
            new List<string> { "EngiTurretBody", "EngiTurret"},
            new List<string> { "ExplosivePotDestructibleBody", "ExplosivePotDestructible"},
            new List<string> { "FlameDroneBody", "FlameDrone"},
            new List<string> { "FusionCellDestructibleBody", "FusionCellDestructible"},
            new List<string> { "GolemBody", "Golem"},
            new List<string> { "GolemBodyInvincible", "GolemInvincible"},
            new List<string> { "GravekeeperBody", "Gravekeeper"},
            new List<string> { "GreaterWispBody", "GreaterWisp"},
            new List<string> { "HaulerBody", "Hauler"},
            new List<string> { "HermitCrabBody", "HermitCrab"},
            new List<string> { "ImpBody", "Imp"},
            new List<string> { "ImpBossBody", "ImpBoss"},
            new List<string> { "JellyfishBody", "Jellyfish"},
            new List<string> { "LemurianBody", "Lemurian"},
            new List<string> { "LemurianBruiserBody", "LemurianBruiser"},
            new List<string> { "MagmaWormBody", "MagmaWorm"},
            new List<string> { "MegaDroneBody", "MegaDrone"},
            new List<string> { "MissileDroneBody", "MissileDrone"},
            new List<string> { "PaladinBody", "Paladin"},
            new List<string> { "Pot2Body", "Pot2"},
            new List<string> { "PotMobile2Body", "PotMobile2"},
            new List<string> { "PotMobileBody", "PotMobile"},
            new List<string> { "ShopkeeperBody", "Shopkeeper"},
            new List<string> { "SpectatorBody", "Spectator"},
            new List<string> { "SpectatorSlowBody", "SpectatorSlow"},
            new List<string> { "SquidTurretBody", "SquidTurret"},
            new List<string> { "TimeCrystalBody", "TimeCrystal"},
            new List<string> { "TitanBody", "Titan"},
            new List<string> { "TitanGoldBody", "TitanGold"},
            new List<string> { "Turret1Body", "Turret1"},
            new List<string> { "UrchinTurretBody", "UrchinTurret"},
            new List<string> { "VagrantBody", "Vagrant"},
            new List<string> { "WispBody", "Wisp" }
        };

        public static string GetBodyNameFromString(string name) {

            foreach (var bodyLists in bodyList) {
                foreach (var tempName in bodyLists) {
                    if (tempName.Equals(name, StringComparison.OrdinalIgnoreCase)) {
                        return bodyLists[0];
                    }
                }
            }

            return name;
        }

        public void Awake() {

            if (instance == null) {
                instance = this;
            } else
                Destroy(this);

            ImmediateSpawn = Config.Wrap("Enable/Disable", "ImmediateSpawn", "Enables or disables immediate spawning as you join", false);
            NormalSurvivorsOnly = Config.Wrap("Enable/Disable", "NormalSurvivorsOnly", "Changes whether or not spawn_as can only be used to turn into survivors", true);
            StartWithItems = Config.Wrap("Enable/Disable", "StartWithItems", "Enables or disables giving players items if they join mid-game", true);
            AllowSpawnAsWhileAlive = Config.Wrap("Enable/Disable", "AllowSpawnAsWhileAlive", "Enables or disables players using spawn_as while alive", true);
            SpawnAsEnabled = Config.Wrap("Enable/Disable", "SpawnAs", "Enables or disables the spawn_as command", true);
            HostOnlySpawnAs = Config.Wrap("Enable/Disable", "HostOnlySpawnAs", "Changes the spawn_as command to be host only", false);

            On.RoR2.Console.Awake += (orig, self) => {
                Utilities.Generic.CommandHelper.RegisterCommands(self);
                orig(self);
            };

            if (ImmediateSpawn.Value) {
                On.RoR2.Run.Start += (orig, self) => {
                    orig(self);
                    self.SetFieldValue("allowNewParticipants", true);
                };
            }
            On.RoR2.NetworkUser.Start += (orig, self) => {
                orig(self);
                if (NetworkServer.active && Stage.instance != null)
                    AddChatMessage("Join the game by typing 'dim_spawn_as [name]' names are Commando, Huntress, Engi, Mage, Merc, Toolbot, Bandit, Rex", 5f);
            };

            On.RoR2.Run.SetupUserCharacterMaster += SetupUserCharacterMaster;
            On.RoR2.Chat.UserChatMessage.ConstructChatString += UserChatMessage_ConstructChatString;
        }

        private static void AddChatMessage(string message, float time = 0.1f) {
            instance.StartCoroutine(AddHelperMessage(message, time));
        }

        private static IEnumerator AddHelperMessage(string message, float time) {
            yield return new WaitForSeconds(time);
            var chatMessage = new Chat.SimpleChatMessage { baseToken = message };
            Chat.SendBroadcastChat(chatMessage);

        }

        private string UserChatMessage_ConstructChatString(On.RoR2.Chat.UserChatMessage.orig_ConstructChatString orig, Chat.UserChatMessage self) {

            if (!NetworkServer.active) {
                return orig(self);
            }

            List<string> split = new List<string>(self.text.Split(Char.Parse(" ")));
            string commandName = ArgsHelper.GetValue(split, 0);

            if (commandName.Equals("dim_spawn_as", StringComparison.OrdinalIgnoreCase)) {


                string bodyString = ArgsHelper.GetValue(split, 1);
                string userString = ArgsHelper.GetValue(split, 2);


                SpawnAs(self.sender.GetComponent<NetworkUser>(), bodyString, userString);
            }
            return orig(self);
        }

        [Server]
        private static void SpawnAs(NetworkUser user, string bodyString, string userString) {

            if (!SpawnAsEnabled.Value) {
                return;
            }

            if (HostOnlySpawnAs.Value) {
                if (NetworkUser.readOnlyInstancesList[0].netId != user.netId) {
                    return;
                }
            }

            bodyString = GetBodyNameFromString(bodyString);

            GameObject bodyPrefab = BodyCatalog.FindBodyPrefab(bodyString);
            if (bodyPrefab == null) {
                AddChatMessage("Could not find " + bodyString);
                return;
            }


            if (NormalSurvivorsOnly.Value) {
                if (!survivorList.Contains(bodyString)) {
                    AddChatMessage("Can only spawn as normal survivors");
                    return;
                }
            }

            NetworkUser player = GetNetUserFromString(userString);
            player = player ?? user;
            CharacterMaster master = player.master;

            if (master) {
                if (AllowSpawnAsWhileAlive.Value && master.alive) {
                    master.bodyPrefab = bodyPrefab;
                    master.Respawn(master.GetBody().transform.position, master.GetBody().transform.rotation);
                    AddChatMessage(player.userName + " respawning as " + bodyString);
                } else if (!master.alive) {
                    AddChatMessage("Cannot use dim_spawn_as while dead");
                } else if (!AllowSpawnAsWhileAlive.Value && master.alive) {
                    AddChatMessage("Cannot use dim_spawn_as while alive");
                }
            } else {
                Run.instance.SetFieldValue("allowNewParticipants", true);
                Run.instance.OnUserAdded(user);

                user.master.bodyPrefab = bodyPrefab;

                Transform spawnTransform = Stage.instance.GetPlayerSpawnTransform();
                CharacterBody body = user.master.SpawnBody(bodyPrefab, spawnTransform.position, spawnTransform.rotation);

                Run.instance.HandlePlayerFirstEntryAnimation(body, spawnTransform.position, spawnTransform.rotation);
                AddChatMessage(player.userName + " spawning as " + bodyString);
                if (!ImmediateSpawn.Value)
                    Run.instance.SetFieldValue("allowNewParticipants", false);
            }

        }

        private static NetworkUser GetNetUserFromString(string playerString) {
            int result = 0;
            if (playerString != "") {
                if (int.TryParse(playerString, out result)) {
                    if (result < NetworkUser.readOnlyInstancesList.Count && result >= 0) {

                        return NetworkUser.readOnlyInstancesList[result];
                    }
                    Debug.Log("Specified player index does not exist");
                    return null;
                } else {
                    foreach (NetworkUser n in NetworkUser.readOnlyInstancesList) {
                        if (n.userName.Equals(playerString, StringComparison.CurrentCultureIgnoreCase)) {
                            return n;
                        }
                    }
                    return null;
                }
            }

            return null;
        }


        private void SetupUserCharacterMaster(On.RoR2.Run.orig_SetupUserCharacterMaster orig, Run self, NetworkUser user) {
            orig(self, user);
            if (!StartWithItems.Value || Run.instance.fixedTime < 5f) {
                return;
            }

            int averageItemCountT1 = 0;
            int averageItemCountT2 = 0;
            int averageItemCountT3 = 0;
            ReadOnlyCollection<NetworkUser> readOnlyInstancesList = NetworkUser.readOnlyInstancesList;

            int playerCount = PlayerCharacterMasterController.instances.Count;

            if (playerCount <= 1)
                return;
            else
                playerCount--;

            for (int i = 0; i < readOnlyInstancesList.Count; i++) {
                if (readOnlyInstancesList[i].id.Equals(user.id))
                    continue;
                CharacterMaster cm = readOnlyInstancesList[i].master;
                averageItemCountT1 += cm.inventory.GetTotalItemCountOfTier(ItemTier.Tier1);
                averageItemCountT2 += cm.inventory.GetTotalItemCountOfTier(ItemTier.Tier2);
                averageItemCountT3 += cm.inventory.GetTotalItemCountOfTier(ItemTier.Tier3);
            }

            averageItemCountT1 /= playerCount;
            averageItemCountT2 /= playerCount;
            averageItemCountT3 /= playerCount;

            CharacterMaster characterMaster = user.master;
            int itemCountT1 = averageItemCountT1 - characterMaster.inventory.GetTotalItemCountOfTier(ItemTier.Tier1);
            int itemCountT2 = averageItemCountT2 - characterMaster.inventory.GetTotalItemCountOfTier(ItemTier.Tier2);
            int itemCountT3 = averageItemCountT3 - characterMaster.inventory.GetTotalItemCountOfTier(ItemTier.Tier3);


            itemCountT1 = itemCountT1 < 0 ? 0 : itemCountT1;
            itemCountT2 = itemCountT2 < 0 ? 0 : itemCountT2;
            itemCountT3 = itemCountT3 < 0 ? 0 : itemCountT3;
            Debug.Log(itemCountT1 + " " + itemCountT2 + " " + itemCountT3 + " itemcount to add");
            Debug.Log(averageItemCountT1 + " " + averageItemCountT2 + " " + averageItemCountT3 + " average");
            for (int i = 0; i < itemCountT1; i++) {
                characterMaster.inventory.GiveItem(GetRandomItem(ItemCatalog.tier1ItemList), 1);
            }
            for (int i = 0; i < itemCountT2; i++) {
                characterMaster.inventory.GiveItem(GetRandomItem(ItemCatalog.tier2ItemList), 1);
            }
            for (int i = 0; i < itemCountT3; i++) {
                characterMaster.inventory.GiveItem(GetRandomItem(ItemCatalog.tier3ItemList), 1);
            }
        }

        private ItemIndex GetRandomItem(List<ItemIndex> items) {
            int itemID = UnityEngine.Random.Range(0, items.Count);

            return items[itemID];
        }




        [ConCommand(commandName = "dim_spawn_as", flags = ConVarFlags.ExecuteOnServer, helpText = "Spawn as a new character. Type body_list for a full list of characters")]
        private static void CCSpawnAs(ConCommandArgs args) {
            if (args.Count == 0) {
                return;
            }

            string bodyString = ArgsHelper.GetValue(args.userArgs, 0);
            string playerString = ArgsHelper.GetValue(args.userArgs, 1);

            SpawnAs(args.sender, bodyString, playerString);

        }

        [ConCommand(commandName = "player_list", flags = ConVarFlags.ExecuteOnServer, helpText = "Shows list of players with their ID")]
        private static void CCPlayerList(ConCommandArgs args) {
            NetworkUser n;
            for (int i = 0; i < NetworkUser.readOnlyInstancesList.Count; i++) {
                n = NetworkUser.readOnlyInstancesList[i];
                Debug.Log(i + ": " + n.userName);
            }
        }
    }


    public class ArgsHelper {

        public static string GetValue(List<string> args, int index) {
            if (index < args.Count && index >= 0) {
                return args[index];
            }

            return "";
        }
    }


}
