﻿using System;
using System.Collections;
using System.Collections.Generic;
using BepInEx;
using RoR2;
using RoR2.Networking;
using UnityEngine;
using UnityEngine.Networking;
using Utilities;

using ArgsHelper = Utilities.Generic.ArgsHelper;

namespace ContinueGames
{

    [BepInPlugin("com.morris1927.ContinueGames", "ContinueGames", "1.0.0")]
    public class SavedGames : BaseUnityPlugin
    {

        public static SavedGames instance { get; set; }

        public void Awake() {
            if (instance == null) {
                instance = this;
            } else {
                Destroy(this);
            }
            On.RoR2.Console.Awake += (orig, self) => {
                Generic.CommandHelper.RegisterCommands(self);
                orig(self);
            };

            On.RoR2.Run.AdvanceStage += (orig, self, stage) => {
                orig(self, stage);
            };
        }

        [ConCommand(commandName = "load", flags = ConVarFlags.Engine, helpText = "Load game")]
        private static void CCLoad(ConCommandArgs args) {
            if (!NetworkServer.active) return;
            if (args.Count != 1) {
                Debug.Log("Command failed, requires 1 argument: load <filename>");
                return;
            }

            string saveString = PlayerPrefs.GetString("Save" + ArgsHelper.GetValue(args.userArgs, 0));
            if (saveString == "") {
                Debug.Log("Save does not exist.");
                return;
            }
            SaveData save = TinyJson.JSONParser.FromJson<SaveData>(saveString);
            instance.StartCoroutine(instance.StartLoading(save));
        }
        [ConCommand(commandName = "save", flags = ConVarFlags.Engine, helpText = "Save game")]
        private static void CCSave(ConCommandArgs args) {
            if (!NetworkServer.active) return;
            if (args.Count != 1) {
                Debug.Log("Command failed, requires 1 argument: save <filename>");
                return;
            }
            SaveGame(ArgsHelper.GetValue(args.userArgs, 0));
        }

        private IEnumerator StartLoading(SaveData save) {
            //yield return new WaitUntil(() => PreGameController.instance);
            if (Run.instance == null) {
                GameNetworkManager.singleton.desiredHost = new GameNetworkManager.HostDescription(new GameNetworkManager.HostDescription.HostingParameters {
                    listen = false,
                    maxPlayers = 1
                });
                yield return new WaitForSeconds(1f);
                PreGameController.instance?.StartLaunch();
                yield return new WaitForSeconds(1f);
            }

            //yield return new WaitUntil(() => Run.instance);

            
            LoadGame(save);
        }

        private static void SaveGame(string saveFile) {
            SaveData save = new SaveData();
            save.playerList = new List<PlayerData>();

            foreach (var item in NetworkUser.readOnlyInstancesList) {
                SavePlayer(item, ref save);
            }
            SaveRun(ref save);

            string json = TinyJson.JSONWriter.ToJson(save);
            Debug.Log(json);
            PlayerPrefs.SetString("Save" + saveFile, json);
        }

        private static void SaveRun(ref SaveData save) {
            save.teamExp = (int)TeamManager.instance.GetTeamExperience(TeamIndex.Player);

            save.difficulty = (int)Run.instance.selectedDifficulty;
            save.fixedTime = Run.instance.fixedTime;
            save.stageClearCount = Run.instance.stageClearCount;
            save.sceneName = Stage.instance.sceneDef.sceneName;
        }

        private static void SavePlayer(NetworkUser player, ref SaveData save) {
            PlayerData playerData = new PlayerData();

            Inventory inventory = player.master.inventory;
            playerData.username = player.userName;

            playerData.items = new int[(int)ItemIndex.Count - 1];
            for (int i = 0; i < (int)ItemIndex.Count -1; i++) {
                playerData.items[i] = inventory.GetItemCount((ItemIndex)i);
            }

            playerData.equipItem0 = (int) inventory.GetEquipment(0).equipmentIndex;
            playerData.equipItem1 = (int) inventory.GetEquipment(1).equipmentIndex;
            playerData.equipItemCount = inventory.GetEquipmentSlotCount();

            playerData.characterBodyName = player.master.bodyPrefab.name;//.Replace(" (Clone)", "");

            save.playerList.Add(playerData);

        }

        private static void LoadGame(SaveData save) {
            foreach (var item in save.playerList) {
                LoadPlayer(item);
            }

            LoadRun(save);



            //TODO: Get SceneDef to load variation
        }

        private static void LoadRun(SaveData save) {
            TeamManager.instance.GiveTeamExperience(TeamIndex.Player, (ulong)save.teamExp);

            Run.instance.selectedDifficulty = (DifficultyIndex)save.difficulty;
            Run.instance.fixedTime = save.fixedTime;
            Run.instance.stageClearCount = save.stageClearCount - 1;
            Run.instance.AdvanceStage(save.sceneName);
        }

        private static void LoadPlayer(PlayerData playerData) {
            NetworkUser player = GetPlayerFromUsername(playerData.username);
            if (player == null) {
                Debug.Log("Could not find player: " + playerData.username);
                return;
            }
            //CharacterBody body = player?.master?.GetBody();
            Inventory inventory = player.master?.inventory;

            GameObject bodyPrefab = BodyCatalog.FindBodyPrefab(playerData.characterBodyName);
            player.master.bodyPrefab = bodyPrefab;
            player.master.Respawn(Vector3.zero, Quaternion.identity);

            for (int i = 0; i < playerData.items.Length -1; i++) {
                inventory.RemoveItem((ItemIndex)i, int.MaxValue);
                inventory.GiveItem((ItemIndex)i, playerData.items[i]);
            }

            inventory.SetEquipmentIndex((EquipmentIndex)playerData.equipItem0);
            if (playerData.equipItemCount == 2) {
                inventory.SetActiveEquipmentSlot((byte)1);
                inventory.SetEquipmentIndex((EquipmentIndex)playerData.equipItem1);
            }
            player.master.money = 15;
        }

        public static NetworkUser GetPlayerFromUsername(string username) {
            foreach (var item in NetworkUser.readOnlyInstancesList) {
                if (username == item.userName) {
                    return item;
                }
            }

            return null;
        }
    }
}
