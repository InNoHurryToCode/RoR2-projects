using RoR2;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SavedGames.Data {
    [Serializable]
    public class PlayerData {

        //public static FieldInfo deployablesList = typeof(CharacterMaster).GetField()


        public string username;

        public SerializableTransform transform;

        public int money;
        public int health;
        public int shields;
        public int infusion;

        public int[] items;
        public List<DeployableData> deployables;

        public int equipItem0;
        public int equipItem1;
        public int equipItemCount;

        public string characterBodyName;

        public bool alive;

        public PlayerData(NetworkUser player) {
            var inventory = player.master.inventory;
            var healthComponent = player.GetCurrentBody().GetComponent<HealthComponent>();


            deployables = new List<DeployableData>();

            transform = new SerializableTransform(player.GetCurrentBody().transform);
            username = player.userName;
            alive = player.master.alive;
            money = (int)player.master.money;
            health = (int)healthComponent.health;
            shields = (int)healthComponent.shield;
            infusion = (int)inventory.infusionBonus;

            items = new int[(int)ItemIndex.Count];
            for (int i = 0; i < (int)ItemIndex.Count; i++) {
                items[i] = inventory.GetItemCount((ItemIndex)i);
            }

            equipItem0 = (int)inventory.GetEquipment(0).equipmentIndex;
            equipItem1 = (int)inventory.GetEquipment(1).equipmentIndex;
            equipItemCount = inventory.GetEquipmentSlotCount();

            characterBodyName = player.master.bodyPrefab.name;

            var deployablesList = player.master.GetFieldValue<List<DeployableInfo>>("deployablesList");
            if (deployablesList != null) {
                foreach (var item in deployablesList) {
                    deployables.Add(new DeployableData(item.deployable));
                }
            }
            
        }


        public void LoadPlayer() {
            var player = SavedGames.GetPlayerFromUsername(username);
            if (player == null) {
                Debug.Log("Could not find player: " + username);
                return;
            }

            var inventory = player.master.inventory;
            var healthComponent = player.GetCurrentBody().GetComponent<HealthComponent>();
            var bodyPrefab = BodyCatalog.FindBodyPrefab(characterBodyName);

            player.master.bodyPrefab = bodyPrefab;
            if (alive) {
                player.master.Respawn(transform.position.GetVector3(), transform.rotation.GetQuaternion());
            } else {
                healthComponent?.Suicide();
            }

            for (int i = 0; i < items.Length; i++) {
                inventory.RemoveItem((ItemIndex)i, int.MaxValue);
                inventory.GiveItem((ItemIndex)i, items[i]);
            }

            inventory.SetEquipmentIndex((EquipmentIndex)equipItem0);
            if (equipItemCount == 2) {
                inventory.SetActiveEquipmentSlot((byte)1);
                inventory.SetEquipmentIndex((EquipmentIndex)equipItem1);
            }
            
            inventory.AddInfusionBonus((uint)-inventory.infusionBonus);
            inventory.AddInfusionBonus((uint)infusion);

            player.master.money = (uint)money;


            foreach (var item in deployables) {
                item.LoadDeployable(player.master);
            }

            SavedGames.instance.StartCoroutine(WaitForStart(player));
        }

        IEnumerator WaitForStart(NetworkUser player) {
            yield return null;
            var healthComponent = player.GetCurrentBody().healthComponent;

            healthComponent.health = health;
            healthComponent.shield = shields;
        }
    }
}
